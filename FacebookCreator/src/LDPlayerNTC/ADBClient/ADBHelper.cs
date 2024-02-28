using Serilog;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LDPlayerAndADBController.ADBClient
{
    public class ADBHelper
    {
        public static string PathFolderADB { get; set; }
        /// <summary>
        /// /////////////
        /// </summary>
        /// <param name="cmdCommand"></param>
        /// <returns></returns>
        public static string ExecuteADB_Result(string cmdCommand, int timeoutMilliseconds = 10000)
        {
            string result;
            int retryCount = 0;
            const int maxRetryCount = 3;
            const int retryIntervalMilliseconds = 2000; // 20 seconds
            try
            {
                do
                {
                    var process = new Process();
                    using (process)
                    {
                        process.StartInfo = new ProcessStartInfo
                        {
                            FileName = $"{PathFolderADB}\\adb.exe",
                            Arguments = cmdCommand,
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            RedirectStandardInput = true,
                            RedirectStandardOutput = true
                        };

                        using (var processCancellationTokenSource = new CancellationTokenSource())
                        using (var timer = new Timer(state => processCancellationTokenSource.Cancel(), null, timeoutMilliseconds, Timeout.Infinite))
                        {
                            process.Start();
                            var waitForExitTask = Task.Run(() =>
                            {
                                process.WaitForExit();
                            });

                            var timeoutTask = Task.Delay(timeoutMilliseconds);

                            // Chờ cho một trong hai Task kết thúc
                            var completedTask = Task.WhenAny(waitForExitTask, timeoutTask).Result;

                            // Kiểm tra xem quá trình đã kết thúc hay chưa
                            if (completedTask == waitForExitTask)
                            {
                                result = process.StandardOutput.ReadToEnd();
                                process.Close();
                                process.Dispose();
                                return result;
                            }
                            else
                            {
                                // Handle timeout
                                result = "Timeout occurred";
                                process.Kill();
                                process.Close();
                                process.Dispose();
                            }
                        }
                    }
                    if (result == null || string.IsNullOrEmpty(result.Trim()) || result == "Timeout occurred")
                    {
                        // Retry after 20 seconds
                        retryCount++;
                        if (retryCount <= maxRetryCount)
                        {
                            Thread.Sleep(retryIntervalMilliseconds);
                        }
                    }
                    else
                    {
                        break; // Break the loop if the result is not null or empty
                    }
                } while (retryCount <= maxRetryCount);
            }
            catch (Exception ex)
            {
                result = null;
                Log.Error($"End {nameof(ADBHelper)}, params; {nameof(ExecuteADB_Result)}, cmd; {cmdCommand}, Error; {ex.Message}, Exception; {ex}");
            }
            return result;
        }
        /// <summary>
        /// /////////////
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static void ExecuteADB(string cmd)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = $"{PathFolderADB}\\adb.exe";
                p.StartInfo.Arguments = cmd;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.EnableRaisingEvents = true;
                p.Start();
                p.WaitForExit();
                p.Close();
                p.Dispose();
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBHelper)}, params; {nameof(ExecuteADB)}, cmd; {cmd}, Error; {ex.Message}, Exception; {ex}");
            }

        }
        /// <summary>
        /// ADB
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrId"></param>
        /// <param name="cmd">cmd adb</param>
        /// <returns>Result cmd</returns>
        public static string ADB(string deviceId, string cmd)
        {
            try
            {
                return ExecuteADB_Result(string.Format("-s {0} {1} ", deviceId, cmd));
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBHelper)}, params; {nameof(ADB)},deviceId; {deviceId}, cmd; {cmd}, Error; {ex.Message}, Exception; {ex}");
            }
            return null;
        }
        private static string TAP_DEVICES = "shell input tap {0} {1}";
        private static string SWIPE_DEVICES = "shell input swipe {0} {1} {2} {3} {4}";
        private static string KEY_DEVICES = "shell input keyevent {0}";
        private static string INPUT_TEXT_DEVICES = "shell input text \"{0}\"";
        private static string CAPTURE_SCREEN_TO_DEVICES = "shell screencap -p \"{0}\"";
        private static string PULL_SCREEN_FROM_DEVICES = "pull \"{0}\"";
        private static string REMOVE_SCREEN_FROM_DEVICES = "shell rm -f \"{0}\"";
        private static string GET_SCREEN_RESOLUTION = "shell dumpsys display | grep mCurrentDisplayRect";
        private static string CLEARCACHES = "shell pm clear {0}";
        /// <summary>
        /// Lấy kích thước màn hình
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <returns></returns>
        public static Point GetScreenResolution(string deviceId)
        {
            try
            {
                string cmdCommand = string.Format(GET_SCREEN_RESOLUTION);
                string result = ADBHelper.ADB(deviceId, cmdCommand);
                result = result.Substring(result.IndexOf("- "));
                result = result.Substring(result.IndexOf(' '), result.IndexOf(')') - result.IndexOf(' '));
                string[] temp = result.Split(new char[]
                {
                ','
                });
                int x = Convert.ToInt32(temp[0].Trim());
                int y = Convert.ToInt32(temp[1].Trim());
                return new Point(x, y);
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBHelper)}, params; {nameof(GetScreenResolution)},deviceId; {deviceId}, Error; {ex.Message}, Exception; {ex}");
                return new Point(0, 0);
            }
        }
        /// <summary>
        /// Click tọa độ phần trăm
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="count"></param>
        public static void TapByPercent(string deviceId, double x, double y, int count = 1)
        {
            int j = 5;
        Start:
            try
            {
                if (j > 0)
                {
                    Point resolution = ADBHelper.GetScreenResolution(deviceId);
                    int X = (int)(x * ((double)resolution.X * 1.0 / 100.0));
                    int Y = (int)(y * ((double)resolution.Y * 1.0 / 100.0));
                    string cmdCommand = string.Format(ADBHelper.TAP_DEVICES, X, Y);
                    for (int i = 1; i < count; i++)
                    {
                        cmdCommand = cmdCommand + " && " + string.Format(ADBHelper.TAP_DEVICES, X, Y);
                    }
                    string result = ADBHelper.ADB(deviceId, cmdCommand);
                }

            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBHelper)}, params; {nameof(TapByPercent)},deviceId; {deviceId}, Error; {ex.Message}, Exception; {ex}");
                j--;
                goto Start;
            }
        }

        /// <summary>
        /// Click theo tọa độ
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="count"></param>
        public static void Tap(string deviceId, int x, int y, int count = 1)
        {
            try
            {
                string cmdCommand = string.Format(ADBHelper.TAP_DEVICES, x, y);
                for (int i = 1; i < count; i++)
                {
                    cmdCommand = cmdCommand + " && " + string.Format(ADBHelper.TAP_DEVICES, x, y);
                }
                string result = ADBHelper.ADB(deviceId, cmdCommand);
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBHelper)}, params; {nameof(Tap)},deviceId; {deviceId}, Error; {ex.Message}, Exception; {ex}");
            }
        }
        public static void ClearCaches(string deviceId, string Package_Name)
        {
            int j = 5;
            try
            {
                while (j > 0)
                {
                    string cmdCommand = string.Format(ADBHelper.CLEARCACHES, Package_Name);
                    string result = ADBHelper.ADB(deviceId, cmdCommand);
                    if (result.Contains("Success"))
                    {
                        return;
                    }
                    j--;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBHelper)}, params; {nameof(ClearCaches)},deviceId; {deviceId}, Error; {ex.Message}, Exception; {ex}");
            }
        }
        /// <summary>
        /// Send Key
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="key"></param>
        public static void Key(string deviceId, ADBKeyEvent key)
        {
            try
            {
                string cmdCommand = string.Format(ADBHelper.KEY_DEVICES, key);
                string result = ADBHelper.ADB(deviceId, cmdCommand);
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBHelper)}, params; {nameof(Key)},deviceId; {deviceId}, Error; {ex.Message}, Exception; {ex}");
            }
        }

        /// <summary>
        /// Input Text
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="text"></param>
        public static void InputText(string deviceId, string text)
        {
            try
            {
                string cmdCommand = string.Format(ADBHelper.INPUT_TEXT_DEVICES, text.Replace(" ", "%s").Replace("&", "\\&").Replace("<", "\\<").Replace(">", "\\>").Replace("?", "\\?").Replace(":", "\\:").Replace("{", "\\{").Replace("}", "\\}").Replace("[", "\\[").Replace("]", "\\]").Replace("|", "\\|"));
                string result = ADBHelper.ADB(deviceId, cmdCommand);
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBHelper)}, params; {nameof(InputText)},deviceId; {deviceId}, Error; {ex.Message}, Exception; {ex}");
            }
        }
        /// <summary>
        /// Kéo theo phần trăm
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="duration"></param>
        public static void SwipeByPercent(string deviceId, double x1, double y1, double x2, double y2, int? duration = 100, int? Count = 1)
        {
            int j = 5;
        Start:
            try
            {
                if (j > 0)
                {
                    Point resolution = ADBHelper.GetScreenResolution(deviceId);
                    int X = (int)(x1 * ((double)resolution.X * 1.0 / 100.0));
                    int Y = (int)(y1 * ((double)resolution.Y * 1.0 / 100.0));
                    int X2 = (int)(x2 * ((double)resolution.X * 1.0 / 100.0));
                    int Y2 = (int)(y2 * ((double)resolution.Y * 1.0 / 100.0));
                    string cmdCommand = string.Format(ADBHelper.SWIPE_DEVICES, new object[]
                    {
                    X,
                    Y,
                    X2,
                    Y2,
                    duration
                    });
                    for (int i = 0; i < Count; i++)
                    {
                        string result = ADBHelper.ADB(deviceId, cmdCommand);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBHelper)}, params; {nameof(TapByPercent)},deviceId; {deviceId}, Error; {ex.Message}, Exception; {ex}");
                j--;
                goto Start;
            }
        }
        /// <summary>
        /// Kéo
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="duration"></param>
        public static void Swipe(string deviceId, int x1, int y1, int x2, int y2, int duration = 100)
        {

            string cmdCommand = string.Format(ADBHelper.SWIPE_DEVICES, new object[]
            {
                x1,
                y1,
                x2,
                y2,
                duration
            });
            try
            {
                string result = ADBHelper.ADB(deviceId, cmdCommand);
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBHelper)}, params; {nameof(Swipe)},deviceId; {deviceId}, Error; {ex.Message}, Exception; {ex}");
            }
        }
        /// <summary> 
        /// LongPress
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="duration">Thời gian dí</param>
        public static void LongPress(string deviceId, int x, int y, int duration = 100)
        {
            string cmdCommand = string.Format(ADBHelper.SWIPE_DEVICES, new object[]
            {
                x,
                y,
                x,
                y,
                duration
            });
            try
            {
                string result = ADBHelper.ADB(deviceId, cmdCommand);
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBHelper)}, params; {nameof(LongPress)},deviceId; {deviceId}, Error; {ex.Message}, Exception; {ex}");
            }
        }
        /// <summary>
        /// Chụp ảnh màn hình
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="isDeleteImageAfterCapture"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Bitmap ScreenShoot(string deviceId, bool isDeleteImageAfterCapture = true, string fileName = "screenShoot.png")
        {
            Bitmap result3 = null;
            try
            {
                fileName = "screenShoot_" + deviceId + ".png";
                string nameToSave = Path.GetFileNameWithoutExtension(fileName) + deviceId + Path.GetExtension(fileName);
                for (; ; )
                {
                    bool flag3 = File.Exists(nameToSave);
                    bool flag6 = !flag3;
                    if (flag6)
                    {
                        break;
                    }
                    try
                    {
                        File.Delete(nameToSave);
                        break;
                    }
                    catch (Exception ex)
                    {
                        break;
                    }
                }
                string Current = Directory.GetCurrentDirectory() + "\\" + nameToSave;
                string CurrentPath = Directory.GetCurrentDirectory().Replace("\\\\", "\\");
                CurrentPath = "\"" + CurrentPath + "\"";
                string cmdCommand = string.Format("shell screencap -p \"{0}\"", "/sdcard/" + nameToSave);
                string cmdCommand2 = string.Format(string.Concat(new string[]
                {
                " pull /sdcard/",
                nameToSave,
                " ",
                CurrentPath
                }), new object[0]);
                string result = ADB(deviceId, cmdCommand);
                string result2 = ADB(deviceId, cmdCommand2);

                try
                {
                    using (Bitmap bitmap = new Bitmap(Current))
                    {
                        result3 = new Bitmap(bitmap);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"End {nameof(ADBHelper)}, params; {nameof(ScreenShoot)},deviceId; {deviceId}, Error; {ex.Message}, Exception; {ex}");
                }
                if (isDeleteImageAfterCapture)
                {
                    try
                    {
                        File.Delete(nameToSave);
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"End {nameof(ADBHelper)}, params; {nameof(ScreenShoot)},deviceId; {deviceId}, Error; {ex.Message}, Exception; {ex}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBHelper)}, params; {nameof(ScreenShoot)},deviceId; {deviceId}, Error; {ex.Message}, Exception; {ex}");
            }
            return result3;
        }

        // ----------------------- Xử lý Ảnh -----------------------//

        /// <summary>
        /// Kiểm tra xem ảnh có tồn tại
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="pathPNG">Đường dẫn ảnh ảnh</param>
        /// <returns></returns>
        public static bool FindImage(string deviceId, string pathPNG, double precent = 0.9, int timeout = 60000)
        {
            bool resutl = false;
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (true)
                {
                    Bitmap subBitmap = ImageScanOpenCV.GetImage(pathPNG);
                    Bitmap screen = ADBHelper.ScreenShoot(deviceId);
                    Point? poinImage = ImageScanOpenCV.FindOutPoint(screen, subBitmap, precent);
                    if (poinImage != null)
                    {
                        resutl = true;
                        sw.Stop(); break;
                    }
                    if (sw.ElapsedMilliseconds > timeout)
                    {
                        resutl = false;
                        sw.Stop(); break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBHelper)}, params; {nameof(FindImage)},deviceId; {deviceId}, Error; {ex.Message}, Exception; {ex}");
            }
            return resutl;

        }
        /// <summary>
        /// Tìm vị trí hình ảnh
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="pathPNG"></param>
        /// <returns></returns>
        public static bool FindImageTap(string deviceId, string pathPNG, double precent = 0.9, int timeout = 60000)
        {
            bool resutl = false;
            try
            {
                //Set the timeout for the image search(in milliseconds)
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (true)
                {
                    Bitmap subBitmap = ImageScanOpenCV.GetImage(pathPNG);
                    Bitmap screen = ADBHelper.ScreenShoot(deviceId);
                    Point? poinImage = ImageScanOpenCV.FindOutPoint(screen, subBitmap, precent);
                    if (poinImage != null)
                    {
                        Tap(deviceId, poinImage.Value.X, poinImage.Value.Y);
                        sw.Stop();
                        ADB(deviceId, "shell rm " + screen);
                        resutl = true;
                        return resutl;
                    }
                    if (sw.ElapsedMilliseconds > timeout)
                    {
                        //Close(param, NameOrIndex);
                        sw.Stop();
                        resutl = false;
                        return resutl;
                    }
                    Thread.Sleep(2000);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBHelper)}, params; {nameof(FindImageTap)},deviceId; {deviceId}, Error; {ex.Message}, Exception; {ex}");
            }
            return resutl;
        }
        /// <summary>
        /// ///////////////
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="pathPNG"></param>
        /// <param name="precent"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static Point? FindImageReturnPoint(string deviceId, string pathPNG, double precent = 0.9, int timeout = 60000)
        {
            try
            {
                //Set the timeout for the image search(in milliseconds)
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (true)
                {
                    Bitmap subBitmap = ImageScanOpenCV.GetImage(pathPNG);
                    Bitmap screen = ADBHelper.ScreenShoot(deviceId);
                    Point? poinImage = ImageScanOpenCV.FindOutPoint(screen, subBitmap, precent);
                    if (poinImage != null)
                    {
                        return poinImage;
                    }
                    if (sw.ElapsedMilliseconds > timeout)
                    {
                        //Close(param, NameOrIndex);
                        sw.Stop();
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBHelper)}, params; {nameof(FindImageReturnPoint)},deviceId; {deviceId}, Error; {ex.Message}, Exception; {ex}");
            }
            return null;
        }
        /// <summary>
        /// //////
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool InputTextWithADBKeyboard(string deviceId, string text)
        {
            try
            {
                // ADBHelper.ADB(deviceId, "shell ime set com.android.adbkeyboard/.AdbIME");
                var convert = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
                string cmdCommand = $"shell am broadcast -a ADB_INPUT_B64 --es msg '{convert}'";
                if (ADBHelper.ADB(deviceId, cmdCommand).Contains("result=0"))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBHelper)}, params; {nameof(InputTextWithADBKeyboard)},deviceId; {deviceId}, Error; {ex.Message}, Exception; {ex}");
            }
            return false;
        }
        /// <summary>
        /// ////
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static bool ClearInputWithADBKeyboard(string deviceId)
        {
            try
            {
                if (ADBHelper.ADB(deviceId, "shell am broadcast -a ADB_CLEAR_TEXT").Contains("result=0"))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBHelper)}, params; {nameof(ClearInputWithADBKeyboard)},deviceId; {deviceId}, Error; {ex.Message}, Exception; {ex}");
            }
            return false;
        }
        /// <summary>
        /// ///
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public static bool TurnOnADBKeyboard(string deviceId)
        {
            try
            {
                if (ADBHelper.ADB(deviceId, "shell ime set com.android.adbkeyboard/.AdbIME").Contains("result=0"))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBHelper)}, params; {nameof(TurnOnADBKeyboard)},deviceId; {deviceId}, Error; {ex.Message}, Exception; {ex}");
            }
            return false;
        }
    }
}
