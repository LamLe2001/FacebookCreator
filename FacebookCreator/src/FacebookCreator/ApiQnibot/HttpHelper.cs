
using Newtonsoft.Json;
using Serilog;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace FacebookCreator.ApiQnibot
{
    public class HttpHelper
    {
        public async Task<ApiResponse> CheckLicense(string licenseKey, string hardwareId, string softwareId)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Constant.QnibotApiUrl);

            string query = "/api/licenses/isvalid?licenseKey=" + licenseKey + "&hardwareId=" + hardwareId + "&softwareId=" + softwareId;
            var response = await httpClient.GetAsync(query);
            var body = await response.Content.ReadAsStringAsync();
            ApiResponse data = JsonConvert.DeserializeObject<ApiResponse>(body);
            return data;

        }
        public async Task<ApiResponse> CheckVersion(string softwareId, string version)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(Constant.QnibotApiUrl);
            string query = "/api/Licenses/CheckVersion?softwareId=" + softwareId + "&version=" + version;
            var response = await httpClient.GetAsync(query);
            var body = await response.Content.ReadAsStringAsync();
            ApiResponse data = JsonConvert.DeserializeObject<ApiResponse>(body);
            return data;

        }
        private string RunCMD(string cmd)
        {
            Process cmdProcess;
            cmdProcess = new Process();
            cmdProcess.StartInfo.FileName = "cmd.exe";
            cmdProcess.StartInfo.Arguments = "/c " + cmd;
            cmdProcess.StartInfo.RedirectStandardOutput = true;
            cmdProcess.StartInfo.UseShellExecute = false;
            cmdProcess.StartInfo.CreateNoWindow = true;
            cmdProcess.Start();
            string output = cmdProcess.StandardOutput.ReadToEnd();
            cmdProcess.WaitForExit();
            if (string.IsNullOrEmpty(output))
                return "";
            return output;
        }
        public string GetHardwareId()
        {
            try
            {
                string outputs = RunCMD("wmic cpu get ProcessorId");
                using (StreamWriter ProcessorId = new StreamWriter("ProcessorId.txt", true))
                {
                    ProcessorId.WriteLine(outputs);
                    ProcessorId.Close();
                }
                string[] liness = File.ReadAllLines("ProcessorId.txt");
                File.Delete("ProcessorId.txt");
                File.Delete("ProcessorId.txt");
                string strs = Regex.Replace(liness[2], @"\s", "");
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                string sMacAddress = string.Empty;
                foreach (NetworkInterface adapter in nics)
                {
                    if (sMacAddress == string.Empty)
                    {
                        IPInterfaceProperties properties = adapter.GetIPProperties();
                        sMacAddress = adapter.GetPhysicalAddress().ToString();
                    }
                }
                string inputString = strs + sMacAddress;
                byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);

                MD5 md5 = MD5.Create();
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                string result = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                if (ex.InnerException != null)
                {
                    Log.Error(ex.InnerException.ToString());
                }
            }
            return string.Empty;
        }
    }
}
