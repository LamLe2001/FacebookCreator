namespace FacebookCreator.Helper
{
    public class MessageCommon
    {
        /// <summary>
        /// Hiển thị hộp thoại thông báo.
        /// </summary>
        /// <param name="message">Nội dung thông báo.</param>
        /// <param name="iconType">Loại biểu tượng hiển thị trong hộp thoại.</param>
        public static void ShowMessageBox(string message, int iconType = 1)
        {
            MessageBoxIcon icon = MessageBoxIcon.None;
            switch (iconType)
            {
                case 1:
                    icon = MessageBoxIcon.Asterisk;
                    break;
                case 2:
                    icon = MessageBoxIcon.Hand;
                    break;
                case 3:
                    icon = MessageBoxIcon.Exclamation;
                    break;
                case 4:
                    icon = MessageBoxIcon.Error;
                    break;
            }
            MessageBox.Show(message, "QNISoft", MessageBoxButtons.OK, icon);
        }

        /// <summary>
        /// Hiển thị hộp thoại xác nhận với hai lựa chọn Yes/No.
        /// </summary>
        /// <param name="question">Nội dung câu hỏi.</param>
        /// <returns>Giá trị DialogResult của hộp thoại.</returns>
        public static DialogResult ShowConfirmationBox(string question)
        {
            return MessageBox.Show(question, "QNISoft", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

    }
}
