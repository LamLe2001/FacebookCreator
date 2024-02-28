using AdvancedSharpAdbClient;
using LDPlayerAndADBController;
using LDPlayerAndADBController.AdbController;
using Newtonsoft.Json;
using Serilog;
using System.Net;

namespace FacebookCreator.FacebookConsoller
{
    internal class FacebookHelper
    {
        private static string GraphApiBaseUrl = "https://graph.facebook.com/";
        public async static Task<string> GetFacebookLoginCode2FA(string passwrod2FA)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri("https://api.code.pro.vn");
                string query = "2fa/v1/get-code?secretKey=" + passwrod2FA;
                var response = await httpClient.GetAsync(query);
                var body = await response.Content.ReadAsStringAsync();
                Code2Fa data = JsonConvert.DeserializeObject<Code2Fa>(body);
                if (data != null)
                {
                    return data.Code;
                }

            }
            catch (Exception ex)
            {
                Log.Error(passwrod2FA, ex, ex.Message);

            }

            return string.Empty;
        }
        public static void FacebookStartIntentUriHandler(string deviceId, string url)
        {
            string cmd = " -s " + deviceId + " shell am start -n com.facebook.katana/.IntentUriHandler " + url;
            LDController.ExecuteCMD_Result(cmd);

        }
        public static async void BowerStartUrlByUid(string deviceId, DeviceData data, AdbClient adbClient, string url)
        {
            string cmd = $" -s {deviceId} shell am start -a android.intent.action.VIEW -d https://www.facebook.com/{url}";
            LDController.ExecuteCMD_Result(cmd);
            await LDController.DelayAsync(2);
            if (ADBClientController.ElementIsExist(data, adbClient, "text='Open with'", 5))
            {
                ADBClientController.ClickElement(data, adbClient, "text='Facebook'", 5);
                await LDController.DelayAsync(3);
                ADBClientController.ClickElement(data, adbClient, "text='ALWAYS'", 5);
            }
        }
        public static async Task<string> GetFacebookUserInfo(string accessToken, string? proxyUrl, string fieldsUrl = "id")
        {
            var httpClientHandler = new HttpClientHandler();
            if (!string.IsNullOrEmpty(proxyUrl))
            {
                var proxyParts = proxyUrl.Split(':');
                var Address = proxyParts[0];
                var Port = int.Parse(proxyParts[1]);
                var Username = proxyParts.Length > 2 ? proxyParts[2] : null;
                var Password = proxyParts.Length > 3 ? proxyParts[3] : null;
                if (!string.IsNullOrEmpty(Address) && Port > 0)
                {
                    var proxy = new WebProxy($"http://{Address}:{Port}");
                    if (!string.IsNullOrEmpty(Username))
                    {
                        proxy.Credentials = new NetworkCredential(Username, Password);
                    }
                    httpClientHandler.Proxy = proxy;
                }
            }
            using (var httpClient = new HttpClient(httpClientHandler))
            {
                // Lấy thông tin người dùng từ Graph API của Facebook với các trường yêu cầu
                var response = await httpClient.GetAsync($"{GraphApiBaseUrl}me?fields={fieldsUrl}&access_token={accessToken}");
                if (response.IsSuccessStatusCode)
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();
                    return jsonResult;
                }
                return null;
            }
        }
        public class Code2Fa
        {
            public string Code { get; set; }
            public string Lifetime { get; set; }
        }
    }
}
