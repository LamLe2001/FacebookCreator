using MailKit;
using MailKit.Net.Imap;
using Serilog;
using System.Text.RegularExpressions;

namespace FacebookCreator.Helper
{
    public class MailKitsHelper
    {
        public static bool CheckLogin(string email, string password, string imap, int port)
        {
            try
            {
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(imap) && port != null)
                {
                    using (var client = new ImapClient())
                    {
                        using (var cancel = new CancellationTokenSource())
                        {
                            client.Connect(imap, port, true, cancel.Token);
                            client.Authenticate(email, password, cancel.Token);
                            client.Disconnect(true, cancel.Token);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, ex.StackTrace);
                return false;
            }
            return false;
        }
        public static string GetCodeEmail(string email, string password, string imap, int port, string fromEmail, string toEmail)
        {
            try
            {
                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(imap) && port != null)
                {
                    using (var client = new ImapClient())
                    {
                        client.Connect(imap, port, true);
                        client.Authenticate(email, password);
                        var inbox = client.Inbox;
                        inbox.Open(FolderAccess.ReadWrite);
                        int index = inbox.Count - 1;
                        for (int i = index; i > 0; i--)
                        {
                            var message = inbox.GetMessage(i);
                            var a = message.To[0].ToString();
                            toEmail = toEmail.ToLower();
                            if (message.From[0].Name == fromEmail && a == toEmail)
                            {
                                string body = message.HtmlBody;
                                string pattern = @"\b\d{5}\b";
                                // Sử dụng regex để tìm các chuỗi phù hợp
                                MatchCollection matches = Regex.Matches(body, pattern);
                                // In ra các kết quả
                                foreach (Match match in matches)
                                {
                                    client.Disconnect(true);
                                    Log.Information($"GET_CODE {nameof(MailKitsHelper)}, params; {nameof(GetCodeEmail)},email; {email},password {password}, imap; {imap}, port; {port}, code; {match.Value}");
                                    return match.Value;
                                }
                            }
                        }
                        client.Disconnect(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(MailKitsHelper)}, params; {nameof(GetCodeEmail)},email; {email}, Error; {ex.Message}, Exception; {ex}");
            }
            return null;
        }
    }
}

