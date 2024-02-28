using AdvancedSharpAdbClient;
using FacebookCreator.Helper;
using LDPlayerAndADBController;
using LDPlayerAndADBController.ADBClient;
using LDPlayerAndADBController.AdbController;
using Serilog;
using System.Net;
using System.Text.RegularExpressions;

namespace FacebookCreator.FacebookConsoller
{
    public class FacebookController : IFacebookController
    {
        private string packageFacebook = "com.facebook.katana";
        public async Task<int> CheckLayoutFacebookAsync(string index, DeviceData data, AdbClient client)
        {
            try
            {
                int j = 0;
                while (j < 5)
                {
                    LDController.RunApp("index", index, "com.facebook.katana");
                    await LDController.DelayAsync(3);
                    if (client.IsAppRunning(data, "com.facebook.katana"))
                    {
                        if (ADBClientController.ElementIsExist(data, client, "content-desc='Mobile number or phonenumber'", 40) == true)
                        {
                            return 1;
                        }
                        else if (ADBClientController.FindElementIsExistOrClickByClass(data, client, "Refresh", "android.widget.Button", 10))
                        {
                            return 2;
                        }
                    }
                    LDController.KillApp("index", index, "com.facebook.katana");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(FacebookController)}, params; {nameof(CheckLayoutFacebookAsync)}, deviceId; {data.Serial}, Error; {ex.Message}, Exception; {ex}");
            }
            return 0;
        }
        public async Task<string> ClickCreaNewAccountFacebookAsync(DeviceData data, AdbClient adbClient)
        {
            try
            {
                List<string> list = new List<string> { "What's your name?" };
                if (ADBClientController.ClickButton(data, adbClient, "Create new account", 20))
                {
                    ADBClientController.ClickButton(data, adbClient, "Get started", 30);
                    foreach (var item in list)
                    {
                        if (ADBClientController.FindElementIsExistOrClickByClass(data, adbClient, item, "android.view.View", 30))
                        {
                            return item;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(FacebookController)}, params; {nameof(ClickCreaNewAccountFacebookAsync)}, deviceId; {data.Serial}, Error; {ex.Message}, Exception; {ex}");
                return null;
            }
        }
        public async Task<string> ImportFullnameFacebookAsync(string index, DeviceData data, AdbClient adbClient, string firtname, string lastname)
        {
            try
            {
                List<string> list = new List<string> { "Set date" };
                if (ADBClientController.FindElementIsExistOrClickByClass(data, adbClient, "What's your name?", "android.view.View", 30))
                {
                    if (ADBClientController.FindElementIsExistOrClickByClass(data, adbClient, "First name", "android.view.View", 10, true))
                    {
                        Thread.Sleep(200);
                        ADBHelper.InputTextWithADBKeyboard(data.Serial, firtname);
                        Thread.Sleep(200);
                    }
                    if (ADBClientController.FindElementIsExistOrClickByClass(data, adbClient, "Last name", "android.view.View", 10, true))
                    {
                        Thread.Sleep(200);
                        ADBHelper.InputTextWithADBKeyboard(data.Serial, lastname);
                        Thread.Sleep(200);
                    }
                    if (ADBClientController.ClickButton(data, adbClient, "Next", 20))
                    {
                        foreach (var item in list)
                        {
                            if (ADBClientController.FindElementIsExistOrClickByClass(data, adbClient, item, "android.widget.TextView", 30))
                            {
                                await LDController.DelayAsync();
                                return item;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(FacebookController)}, params; {nameof(ImportFullnameFacebookAsync)}, deviceId; {data.Serial}, Error; {ex.Message}, Exception; {ex}");
                return null;
            }
        }
        public async Task<string> SelecBirthDayFacebookAsync(string index, DeviceData data, AdbClient adbClient)
        {
            try
            {
                List<string> list = new List<string> { "What's your gender?" };
                if (ADBClientController.FindElementIsExistOrClickByClass(data, adbClient, "Set date", "android.widget.TextView", 30))
                {
                    Random random = new Random();
                    var select = random.Next(6, 14);
                    ADBHelper.SwipeByPercent(data.Serial, 26.2, 41.1, 26.8, 57.9, select);// thang
                    ADBHelper.SwipeByPercent(data.Serial, 48.5, 41.1, 48.5, 57.0, select);// ngay
                    ADBHelper.SwipeByPercent(data.Serial, 71.7, 41.1, 71.1, 56.7, select);// nam
                    if (ADBClientController.ClickButton(data, adbClient, "SET", 30))
                    {
                        if (ADBClientController.FindElementIsExistOrClickByClass(data, adbClient, "What's your birthday?", "android.view.View", 30))
                        {
                            if (ADBClientController.ClickButton(data, adbClient, "Next", 30))
                            {
                                foreach (var item in list)
                                {
                                    if (ADBClientController.FindElementIsExistOrClickByClass(data, adbClient, item, "android.view.View", 30))
                                    {
                                        return item;
                                    }
                                }
                            }
                        }

                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(FacebookController)}, params; {nameof(SelecBirthDayFacebookAsync)}, deviceId; {data.Serial}, Error; {ex.Message}, Exception; {ex}");
                return null;
            }
        }
        public async Task<string> SelecGenderFacebookAsync(DeviceData data, AdbClient adbClient, int? gender)
        {
            try
            {
                List<string> list = new List<string> { "What's your mobile number?" };
                if (ADBClientController.FindElementIsExistOrClickByClass(data, adbClient, "What's your gender?", "android.view.View", 30))
                {
                    if (gender == 0)
                    {
                        ADBClientController.ClickButton(data, adbClient, "Male", 10);
                    }
                    else
                    {
                        ADBClientController.ClickButton(data, adbClient, "Female", 10);
                    }
                    if (ADBClientController.ClickButton(data, adbClient, "Next", 30))
                    {
                        ADBClientController.ClickButton(data, adbClient, "ALLOW", 30);
                        await LDController.DelayAsync(1, 3);
                        ADBClientController.ClickButton(data, adbClient, "ALLOW", 10);
                    }
                    foreach (var item in list)
                    {
                        if (ADBClientController.FindElementIsExistOrClickByClass(data, adbClient, item, "android.view.View", 30))
                        {
                            return item;
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(FacebookController)}, params; {nameof(SelecGenderFacebookAsync)}, deviceId; {data.Serial}, Error; {ex.Message}, Exception; {ex}");
                return null;
            }
        }
        public async Task<string> ImportEmailFacebookAsync(string index, DeviceData data, AdbClient adbClient, string email)
        {
            try
            {
                List<string> list = new List<string> { "Create a password" };
                if (ADBClientController.ClickButton(data, adbClient, "Sign up with phonenumber", 30))
                {
                    if (ADBClientController.FindElementIsExistOrClickByClass(data, adbClient, "Email", "android.view.View", 30, true))
                    {
                        Thread.Sleep(200);
                        ADBHelper.InputTextWithADBKeyboard(data.Serial, email);
                        Thread.Sleep(200);
                    }
                    if (ADBClientController.ClickButton(data, adbClient, "Next", 30))
                    {
                        foreach (var item in list)
                        {
                            if (ADBClientController.FindElementIsExistOrClickByClass(data, adbClient, item, "android.view.View", 30))
                            {
                                return item;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(FacebookController)}, params; {nameof(ImportEmailFacebookAsync)}, deviceId; {data.Serial}, Error; {ex.Message}, Exception; {ex}");
                return null;
            }
        }
        public async Task<string> ImportPhonenumberFacebookAsync(string index, DeviceData data, AdbClient adbClient, string phonenumber)
        {
            try
            {
                List<string> list = new List<string> { "Create a password" };
                if (ADBClientController.FindElementIsExistOrClickByClass(data, adbClient, "What's your mobile number?", "android.view.View", 30))
                {
                    if (ADBClientController.ClickElement(data, adbClient, "class='android.widget.EditText'", 6))
                    {
                        Thread.Sleep(500);
                        ADBHelper.InputTextWithADBKeyboard(data.Serial, phonenumber);
                        Thread.Sleep(500);
                        if (ADBClientController.ClickButton(data, adbClient, "Next", 10))
                        {
                            foreach (var item in list)
                            {
                                if (ADBClientController.FindElementIsExistOrClickByClass(data, adbClient, item, "android.view.View", 20))
                                {
                                    return item;
                                }
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(FacebookController)}, params; {nameof(ImportPhonenumberFacebookAsync)}, cmd; {data.Serial}, Error; {ex.Message}, Exception; {ex}");
                return null;
            }
        }
        public async Task<string> ImportPasswrodFacebookAsync(string index, DeviceData data, AdbClient adbClient, string password)
        {
            try
            {
                List<string> list = new List<string> { "Enter the confirmation code" };
                if (ADBClientController.FindElementIsExistOrClickByClass(data, adbClient, "Password", "android.view.View", 30, true))
                {
                    Thread.Sleep(200);
                    ADBHelper.InputTextWithADBKeyboard(data.Serial, password);
                    Thread.Sleep(200);
                    if (ADBClientController.ClickButton(data, adbClient, "Next", 30))
                    {
                        ADBClientController.ClickButton(data, adbClient, "Not now", 20);
                        ADBClientController.FindElementIsExistOrClickByClass(data, adbClient, "I agree", "android.view.View", 30, true);
                        foreach (var item in list)
                        {
                            if (ADBClientController.FindElementIsExistOrClickByClass(data, adbClient, item, "android.view.View", 50))
                            {
                                return "Enter the confirmation code";
                            }
                        }
                        if (ADBClientController.FindElementIsExistOrClickByClass(data, adbClient, "We need more information", "android.view.ViewGroup", 10))
                        {
                            return "Ban 180 days";
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(FacebookController)}, params; {nameof(ImportPasswrodFacebookAsync)}, cmd; {data.Serial}, Error; {ex.Message}, Exception; {ex}");
                return null;
            }
        }
        public async Task<bool> FakeEmailFacebookAsync(string index, DeviceData data, AdbClient adbClient, string email)
        {
            try
            {
                if (ADBClientController.ClickButton(data, adbClient, "I didn’t get the code", 30))
                {
                    if (ADBClientController.FindElementIsExistOrClickByClass(data, adbClient, "Change phonenumber", "android.view.ViewGroup", 30, true))
                    {
                        if (ADBClientController.FindElementIsExistOrClickByClass(data, adbClient, "Email", "android.view.View", 30, true))
                        {
                            Thread.Sleep(200);
                            ADBHelper.InputText(data.Serial, email);
                            Thread.Sleep(200);
                            if (ADBClientController.ClickButton(data, adbClient, "Next", 30))
                            {
                                if (ADBClientController.FindElementIsExistOrClickByClass(data, adbClient, "Enter the confirmation code", "android.view.View", 30))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(FacebookController)}, params; {nameof(FakeEmailFacebookAsync)}, cmd; {data.Serial}, Error; {ex.Message}, Exception; {ex}");
                return false;
            }
        }
        public async Task<bool> ImportCodeFacebookAsync(string index, DeviceData data, AdbClient adbClient, string code)
        {
            try
            {
                if (ADBClientController.FindElementIsExistOrClickByClass(data, adbClient, "Confirmation code", "android.view.View", 30, true))
                {
                    Thread.Sleep(200);
                    ADBHelper.InputTextWithADBKeyboard(data.Serial, code);
                    Thread.Sleep(200);
                    if (ADBClientController.ClickButton(data, adbClient, "Next", 30))
                    {
                        ADBClientController.ClickButton(data, adbClient, "Skip", 20);
                        LDController.KillApp("index", index, packageFacebook);
                        await LDController.DelayAsync(2, 4);
                        LDController.RunApp("index", index, packageFacebook);
                        await LDController.DelayAsync(10, 30);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(FacebookController)}, params; {nameof(ImportCodeFacebookAsync)}, cmd; {data.Serial}, Error; {ex.Message}, Exception; {ex}");
                return false;
            }
        }
        public async Task<int> CheckLoginSuccesFacebookAsync(DeviceData data, AdbClient client)
        {
            List<string> listElement = new List<string>
            {
                "content-desc='Mobile number or phonenumber'", "text ='Unable to log in'", "text='Allow Facebook to access your location?'",
                "text='We disabled your account'" ,"content-desc='Go to profile'",
                "text='Allow Facebook to access your contacts?'", "content-desc='Continue in English (US)'", "text='Find friends'",
                "text='Save your login info?'", "text='Add number'",
                "text='Access to contacts'",  "text='OK'","text='Skip'", "text='ALLOW'"
                ,"text='The password you entered is incorrect. To log in, you'll need to enter a code.'","text='Is this your account?'",
                "text='Check your notifications on another device'"
                ,"content-desc='Log in using another device'"
            };
            string findElement = string.Empty;
            foreach (string element in listElement)
            {
                try
                {
                    if (ADBClientController.ElementIsExist(data, client, element, 5))
                    {
                        findElement = element;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, ex.Message);
                }
            }
            switch (findElement)
            {
                case "content-desc='Mobile number or phonenumber'":
                    {
                        return 0;
                    }
                case "text ='Unable to log in'":
                    {
                        return 0;
                    }
                case "content-desc='Continue in English (US)'":
                    {
                        return 1;
                    }
                case "text='Find friends'":
                    {
                        return 1;
                    }
                case "text='Save your login info?'":
                    {
                        return 1;
                    }
                case "text='Add number'":
                    {
                        return 1;
                    }
                case "text='Access to contacts'":
                    {
                        return 1;
                    }
                case "text='Allow Facebook to access your contacts?'":
                    {
                        return 1;
                    }
                case "content-desc='Go to profile'":
                    {
                        return 1;
                    }
                case "text='Allow'":
                    {
                        return 1;
                    }
                case "text='Skip'":
                    {
                        return 1;
                    }
                case "text='ALLOW'":
                    {
                        return 1;
                    }
                case "text='OK'":
                    {
                        return 1;
                    }
                case "text='Allow Facebook to access your location?'":
                    {
                        return 1;
                    }
                case "text='We disabled your account'":
                    {
                        return 1;
                    }
                case "text='The password you entered is incorrect. To log in, you'll need to enter a code.'":
                    {
                        return 1;
                    }
                case "text='Is this your account?'":
                    {
                        return 1;
                    }
                case "text='Check your notifications on another device'":
                    {
                        return 1;
                    }
                case "content-desc='Log in using another device'":
                    {
                        await LDController.DelayAsync();
                        return 1;
                    }
                default:
                    {
                        try
                        {
                            var dump = client.DumpScreen(data);
                            Log.Information("///////////////////////////////////XML/////////////////////////////////////////", "///////////////////////////////////XML/////////////////////////////////////////");
                            Log.Information(dump.OuterXml);
                            Log.Information("///////////////////////////////////XML/////////////////////////////////////////", "///////////////////////////////////XML/////////////////////////////////////////");
                        }
                        catch (Exception ex)
                        {
                            Log.Error($"{nameof(FacebookController)}, params; {nameof(CheckLoginSuccesFacebookAsync)}, cmd; {data.Serial}, Error; {ex.Message}, Exception; {ex}");
                        }
                        return 0;
                    }
            }
        }
        public async Task<string> GetTokenAndUidFacebookAsync(string deviceId, string folderBackup)
        {
            try
            {
                string result = string.Empty;
                string copyFile = "shell su -c 'cp /data/data/com.facebook.katana/app_light_prefs/com.facebook.katana/authentication /sdcard/authentication.txt'";
                ADBHelper.ADB(deviceId, copyFile);
                await LDController.DelayAsync(2);
                ADBHelper.ADB(deviceId, $" pull /sdcard/authentication.txt {folderBackup}");
                await LDController.DelayAsync(2);
                string rawText = File.ReadAllText($"{folderBackup}\\authentication.txt");
                rawText = Regex.Replace(rawText, "[^\\u0020-\\u007E]", "|");
                var tokenAndroid = "EAA" + Regex.Match(rawText, "EAA(.*?)\\|").Groups[1].Value;
                var uid = Regex.Match(rawText, "uid\\|\\|(.*?)\\|").Groups[1].Value;
                result = tokenAndroid + "|" + uid;
                return result;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(FacebookController)}, params; {nameof(GetTokenAndUidFacebookAsync)}, cmd; {deviceId}, Error; {ex.Message}, Exception; {ex}");
                return null;
            }
        }
        public async Task<string> GetPassword2FAFacebookAsync(string deviceId, string index, DeviceData data, AdbClient client, string password)
        {
            try
            {
                int i = 0;
                while (i < 6)
                {
                    FacebookHelper.FacebookStartIntentUriHandler(deviceId, "fb://account_settings");
                    if (ADBClientController.ClickButton(data, client, "Security and login", 30))
                    {
                        if (ADBClientController.FindElementIsExistOrClickByClass(data, client, "Use two-factor authentication, Turned off, Log in with a code from your phone as well as a password", "android.view.View", 30, true))
                        {
                            if (ADBClientController.FindElementIsExistOrClickByClass(data, client, "Continue", "android.view.ViewGroup", 30, true))
                            {
                                var doc = FileHelper.ReadQRCode(ADBHelper.ScreenShoot(deviceId));
                                if (ADBClientController.FindElementIsExistOrClickByClass(data, client, "Continue", "android.view.ViewGroup", 30, true))
                                {
                                    if (!string.IsNullOrEmpty(doc))
                                    {
                                        int secretIndex = doc.IndexOf("secret=");
                                        if (secretIndex != -1)
                                        {
                                            int startIndex = secretIndex + 7; // Độ dài của "secret=" là 7
                                            int endIndex = doc.IndexOf("&", startIndex);
                                            if (endIndex == -1)
                                            {
                                                endIndex = doc.Length;
                                            }
                                            var tow2FA = doc.Substring(startIndex, endIndex - startIndex);
                                            if (!string.IsNullOrEmpty(tow2FA))
                                            {
                                                ADBHelper.TapByPercent(deviceId, 27.1, 33.5);
                                                string code = await FacebookHelper.GetFacebookLoginCode2FA(tow2FA);
                                                if (!string.IsNullOrEmpty(code))
                                                {
                                                    ADBHelper.InputText(deviceId, code);
                                                    Thread.Sleep(200);
                                                    if (ADBClientController.FindElementIsExistOrClickByClass(data, client, "Continue", "android.view.ViewGroup", 30, true))
                                                    {
                                                        if (ADBClientController.FindElementIsExistOrClickByClass(data, client, "Password", "android.widget.EditText", 30, true))
                                                        {
                                                            Thread.Sleep(200);
                                                            ADBHelper.InputText(deviceId, password);
                                                            Thread.Sleep(200);
                                                            if (ADBClientController.ClickButton(data, client, "CONTINUE", 30))
                                                            {
                                                            }
                                                        }
                                                        if (ADBClientController.ElementIsExist(data, client, "content-desc='Done'", 30))
                                                        {
                                                            return tow2FA;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    i++;
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(FacebookController)}, params; {nameof(GetPassword2FAFacebookAsync)}, deviceId; {deviceId}, Error; {ex.Message}, Exception; {ex}");
                return null;
            }

        }
        public async Task<bool> UploadAvatarToFacebookAsync(string accessToken, string imagePath, string? proxyUrl)
        {
            try
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
                    // Đọc dữ liệu ảnh từ đường dẫn tới file ảnh
                    var imageBytes = File.ReadAllBytes(imagePath);

                    // Tạo một multipart form data để upload ảnh
                    var form = new MultipartFormDataContent();
                    form.Add(new ByteArrayContent(imageBytes), "source", Path.GetFileName(imagePath));
                    // Gửi yêu cầu POST tới Graph API của Facebook để upload ảnh lên tài khoản người dùng
                    var response = await httpClient.PostAsync($"https://graph.facebook.com/me/picture?access_token={accessToken}", form);
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{nameof(FacebookController)}, params; {nameof(UploadAvatarToFacebookAsync)}, token; {accessToken}, Error; {ex.Message}, Exception; {ex}");
                return false;

            }
        }
    }
}
