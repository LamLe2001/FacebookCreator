using System.ComponentModel;

namespace FacebookCreator.MuiltiTask
{
    public class BaseViewModel : ThreadController, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ThreadController
    {
        public delegate void ThreadAction();
        public Task? _Task;
        CancellationTokenSource? src;
        PauseTokenSource? pauseSource;

        public bool StopTask()
        {
            if (_Task == null)
                return true;

            try
            {
                if (src == null)
                    return true;

                src.Cancel();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void TaskWait(CancellationTokenSource tokenSource, PauseTokenSource pauseSource)
        {
            var ct = StartTask(async () =>
            {
                while (true)
                {
                    try
                    {
                        tokenSource.Token.ThrowIfCancellationRequested();
                    }
                    catch
                    {
                        return;
                    }

                    await pauseSource.Token.PauseIfRequestedAsync();

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }, tokenSource, pauseSource);
        }

        public bool StartTask(ThreadAction action, CancellationTokenSource? tokenSource, PauseTokenSource? pauseSource)
        {
            if (_Task != null)
            {
                StopTask();
                _Task = null;
            }

            try
            {
                src = tokenSource;
                this.pauseSource = pauseSource;

                if (tokenSource == null)
                {
                    _Task = Task.Run(() => { action(); });
                }
                else
                {
                    _Task = Task.Run(() => { action(); }, tokenSource.Token);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> PauseTask()
        {
            if (_Task == null)
                return true;

            try
            {
                if (pauseSource != null)
                {
                    await pauseSource.PauseAsync();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ResumeTask()
        {
            if (_Task == null)
                return true;

            try
            {
                if (pauseSource != null)
                {
                    await pauseSource.ResumeAsync();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class PauseTokenSource
    {
        private bool _paused = false;
        private bool _pauseRequested = false;

        private TaskCompletionSource<bool> _resumeRequestTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        private TaskCompletionSource<bool> _pauseConfirmationTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        private readonly SemaphoreSlim _stateAsyncLock = new SemaphoreSlim(1);
        private readonly SemaphoreSlim _pauseRequestAsyncLock = new SemaphoreSlim(1);

        public PauseToken Token => new PauseToken(this);

        public async Task<bool> IsPaused(CancellationToken token = default)
        {
            await _stateAsyncLock.WaitAsync(token);

            try
            {
                return _paused;
            }
            finally
            {
                _stateAsyncLock.Release();
            }
        }

        public async Task ResumeAsync(CancellationToken token = default)
        {
            await _stateAsyncLock.WaitAsync(token);

            try
            {
                if (!_paused)
                    return;

                await _pauseRequestAsyncLock.WaitAsync(token);

                try
                {
                    var resumeRequestTcs = _resumeRequestTcs;
                    _paused = false;
                    _pauseRequested = false;
                    _resumeRequestTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    _pauseConfirmationTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                    resumeRequestTcs.TrySetResult(true);
                }
                finally
                {
                    _pauseRequestAsyncLock.Release();
                }
            }
            finally
            {
                _stateAsyncLock.Release();
            }
        }

        public async Task PauseAsync(CancellationToken token = default)
        {
            await _stateAsyncLock.WaitAsync(token);

            try
            {
                if (_paused)
                    return;

                Task pauseConfirmationTask;

                await _pauseRequestAsyncLock.WaitAsync(token);

                try
                {
                    _pauseRequested = true;
                    pauseConfirmationTask = WaitForPauseConfirmationAsync(token);
                }
                finally
                {
                    _pauseRequestAsyncLock.Release();
                }

                await pauseConfirmationTask;

                _paused = true;
            }
            finally
            {
                _stateAsyncLock.Release();
            }
        }

        private async Task WaitForPauseConfirmationAsync(CancellationToken token)
        {
            await _pauseConfirmationTcs.Task.WaitAsync(token);
        }

        internal async Task PauseIfRequestedAsync(CancellationToken token = default)
        {
            await _pauseRequestAsyncLock.WaitAsync(token);

            try
            {
                if (!_pauseRequested)
                    return;

                var resumeRequestTask = WaitForResumeRequestAsync(token);
                _pauseConfirmationTcs.TrySetResult(true);

                await resumeRequestTask;
            }
            finally
            {
                _pauseRequestAsyncLock.Release();
            }
        }

        private async Task WaitForResumeRequestAsync(CancellationToken token)
        {
            await _resumeRequestTcs.Task.WaitAsync(token);
        }
    }

    public struct PauseToken
    {
        private readonly PauseTokenSource _source;

        public PauseToken(PauseTokenSource source)
        {
            _source = source;
        }

        public Task<bool> IsPaused()
        {
            return _source.IsPaused();
        }

        public Task PauseIfRequestedAsync(CancellationToken token = default)
        {
            return _source.PauseIfRequestedAsync(token);
        }
    }

    public class RelayCommand<T>
    {
        private readonly Predicate<T>? _canExecute;
        private readonly Action<T> _execute;

        public RelayCommand(Predicate<T>? canExecute, Action<T> execute)
        {
            _canExecute = canExecute;
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke((T)parameter!) ?? true;
        }

        public void Execute(object? parameter)
        {
            _execute((T)parameter!);
        }
    }
}
