using System.Net;
using System.Text.RegularExpressions;

namespace FacebookCreator.Helper
{
    public static class CommonMethods
    {
        private static bool isMoving = false;
        private static Point lastLocation;
        private static Control currentControl;
        public static void WireUpMouseEvents(Control control, Button cancelButton)
        {
            control.MouseDown += Form_MouseDown;
            control.MouseMove += Form_MouseMove;
            control.MouseUp += Form_MouseUp;
            currentControl = control;
            cancelButton.Click += btnCancel_Click;
        }

        private static void Form_MouseDown(object sender, MouseEventArgs e)
        {
            isMoving = true;
            lastLocation = e.Location;
        }

        private static void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMoving)
            {
                Control form = ((Control)sender).FindForm(); // Lấy reference đến form chứa control
                form.Left += e.X - lastLocation.X;
                form.Top += e.Y - lastLocation.Y;
            }
        }

        private static void Form_MouseUp(object sender, MouseEventArgs e)
        {
            isMoving = false;
        }
        private static void btnCancel_Click(object sender, EventArgs e)
        {
            Button cancelButton = (Button)sender;
            Form form = cancelButton.FindForm();

            // Thực hiện logic khi nhấn nút Cancel tại đây
            //form.Controls["button1"].Enabled = false;
            form.Close();
        }
        public static void CloseForm(Form form)
        {
            //form.Controls["button1"].Enabled = false;
            form.Close();
        }
        /// <summary>
        /// Cập nhật số lượng dòng trong RichTextBox vào văn bản của một Control khác.
        /// </summary>
        /// <param name="F58992BD">RichTextBox chứa các dòng cần đếm.</param>
        /// <param name="control">Control khác để cập nhật số lượng dòng.</param>
        /// <param name="def">Cờ chỉ định cách xử lý dòng trống (mặc định: 0).</param>
        public static void UpdateLineCount(RichTextBox rtbox, Control control, int def = 0)
        {
            try
            {
                string originalText = control.Text;
                List<string> lines = new List<string>();

                if (def != 0)
                {
                    lines = rtbox.Text.Split(new string[] { "\n|\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                else
                {
                    lines = rtbox.Lines.ToList();
                }

                lines = RemoveEmptyLines(lines);
                control.Text = originalText.Replace(Regex.Match(originalText, "\\((.*?)\\)").Value, "(" + lines.Count + ")");
            }
            catch
            {
                // Xử lý ngoại lệ tại đây (nếu cần)
            }
        }
        /// <summary>
        /// Loại bỏ các dòng trống từ một danh sách dòng.
        /// </summary>
        /// <param name="lines">Danh sách dòng ban đầu.</param>
        /// <returns>Danh sách dòng đã loại bỏ các dòng trống.</returns>
        public static List<string> RemoveEmptyLines(List<string> lines)
        {
            List<string> result = new List<string>();

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (!string.IsNullOrEmpty(trimmedLine))
                {
                    result.Add(trimmedLine);
                }
            }

            return result;
        }
        /// <summary>
        /// Thay thế chuỗi trong Text của một Control bằng một giá trị mới.
        /// </summary>
        /// <param name="control">Control cần thay thế Text.</param>
        /// <param name="newValue">Giá trị mới để thay thế.</param>
        public static void UpdateLineCount(Control control, int def = 0)
        {
            try
            {
                string text = control.Text;
                control.Text = text.Replace(Regex.Match(text, "\\((.*?)\\)").Value, "(" + def + ")");
            }
            catch
            {
            }
        }
        /// <summary>
        /// Xáo trộn danh sách các chuỗi.
        /// </summary>
        /// <param name="inputList">Danh sách đầu vào.</param>
        /// <returns>Danh sách đã được xáo trộn.</returns>
        public static List<string> ShuffleList(List<string> inputList)
        {
            string temp = "";
            int count = inputList.Count;
            int index = 0;
            Random rd = new Random();
            while (count != 0)
            {
                index = rd.Next(0, inputList.Count);
                count--;
                temp = inputList[count];
                inputList[count] = inputList[index];
                inputList[index] = temp;
            }
            return inputList;
        }
        public static List<string> Shuffle(List<string> inputList)
        {
            List<string> result = new List<string>();
            Random rd = new Random();
            int count = inputList.Count;
            int index = 0;
            while (count != 0)
            {
                index = rd.Next(0, inputList.Count);
                count--;
                result.Add(inputList[index]);
            }
            return result;
        }
        public static async Task<bool> CheckPorxy(string host, string port, string? username, string? password, bool http = true)
        {
            try
            {
                string proxyAddress = host + ":" + port;
                // Tạo một WebRequest sử dụng proxy
                WebRequest request = WebRequest.Create("https://www.google.com/");
                request.Proxy = new WebProxy(proxyAddress);
                {
                    if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                    {
                        // Xác thực proxy nếu cần thiết
                        request.Proxy.Credentials = new NetworkCredential(username, password);
                    }
                    if (http == false)
                    {
                        // Sử dụng SOCKS5 proxy
                        ((WebProxy)request.Proxy).UseDefaultCredentials = false;
                        ((WebProxy)request.Proxy).BypassProxyOnLocal = false;
                    }
                    // Thực hiện một yêu cầu GET đơn giản để kiểm tra proxy
                    using (WebResponse response = request.GetResponse())
                    {
                        // Kiểm tra mã trạng thái HTTP để xác định tính hợp lệ của proxy
                        HttpStatusCode statusCode = ((HttpWebResponse)response).StatusCode;
                        return (int)statusCode >= 200 && (int)statusCode < 300;
                    }
                }
            }
            catch (WebException)
            {
                // Xảy ra lỗi khi yêu cầu sử dụng proxy
                return false;
            }
        }
    }
}
