using Serilog;
using System.Net;
using static FacebookCreator.ProxyDroid.ProxyHelper;

namespace FacebookCreator.ProxyDroid
{
    public class ProxyHelper
    {
        public static bool CheckProxy(string host, string port, string? username, string? password)
        {
            try
            {
                string proxyAddress = host + ":" + port;

                // Create a WebRequest using proxy
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api64.ipify.org/");
                request.Proxy = new WebProxy(proxyAddress);

                // Set proxy credentials if provided
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    request.Proxy.Credentials = new NetworkCredential(username, password);
                }

                // Send a GET request to check the proxy
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // Check the HTTP status code to determine proxy validity
                    HttpStatusCode statusCode = response.StatusCode;

                    // Return true if status code indicates success
                    return statusCode >= HttpStatusCode.OK && statusCode < HttpStatusCode.Ambiguous;
                }
            }
            catch (WebException)
            {
                // An error occurred when using the proxy
                return false;
            }
        }

        public async static Task<List<string>> ReadProxy(List<string> lines)
        {
            List<string> proxies = new List<string>();
            try
            {
                Log.Information($"Start {nameof(ProxyHelper)}, params:  {nameof(ReadProxy)}, lines {lines} ");
                foreach (var line in lines)
                {
                    string[] proxy = line.Split(':');
                    if (!string.IsNullOrEmpty(proxy[0]) && !string.IsNullOrEmpty(proxy[1]))
                    {
                        if (CheckProxy(proxy[0], proxy[1], proxy[2], proxy[3]))
                        {
                            proxies.Add(line);
                        }

                    }
                }
                Log.Information($"End {nameof(ProxyHelper)}, params; {nameof(ReadProxy)}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
            return proxies;
        }

        public class ProxyInfo
        {
            public string? Host { get; set; }
            public string? Port { get; set; }
            public string? Username { get; set; }
            public string? Password { get; set; }
            public bool IsRunning { get; set; } = false;
        }
    }
}
