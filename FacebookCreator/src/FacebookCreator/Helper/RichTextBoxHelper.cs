namespace FacebookCreator.Helper
{
    public class RichTextBoxHelper
    {
        public static RichTextBox _RichTextBox;
        public static void WriteLogRichTextBox(string message, int type = 0)
        {
            message = $"{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss\t")}    {message}";
            if (_RichTextBox != null)
            {
                switch (type)
                {
                    case 0:
                        {
                            _RichTextBox.Invoke((MethodInvoker)delegate
                            {
                                _RichTextBox.InvokeEx(s => s.AppendText(message, Color.Black, _RichTextBox.Font, true));
                            });
                            break;
                        }
                    case 1:
                        {
                            _RichTextBox.Invoke((MethodInvoker)delegate
                            {
                                _RichTextBox.InvokeEx(s => s.AppendText(message, Color.Red, _RichTextBox.Font, true));
                            });
                            break;
                        }
                    case 2:
                        {
                            _RichTextBox.Invoke((MethodInvoker)delegate
                            {
                                _RichTextBox.InvokeEx(s => s.AppendText(message, Color.Green, _RichTextBox.Font, true));
                            });
                            break;
                        }
                }
            }

        }
    }
}
