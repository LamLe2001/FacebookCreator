namespace FacebookCreator.Helper
{
    public class FormHelper
    {
        public static List<string> RemoveEmptyStrings(List<string> list_0)
        {
            List<string> list = new List<string>();
            string text = "";
            for (int i = 0; i < list_0.Count; i++)
            {
                text = list_0[i].Trim();
                if (text != "")
                {
                    list.Add(text);
                }
            }
            return list;
        }
        public static void ShowFormWithoutTaskbar(Form form)
        {
            try
            {
                form.ShowInTaskbar = false;
                form.ShowDialog();
            }
            catch
            {
            }
        }
    }
}
