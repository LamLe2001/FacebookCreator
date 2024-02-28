using AdvancedSharpAdbClient;
using FacebookCreator.Models;
using LDPlayerAndADBController;
using Serilog;

namespace FacebookCreator.MuiltiTask
{
    public class DeviceHelper
    {
        public static bool Connect(DeviceInfo device, int count = 1)
        {
            try
            {
                while (count > 0)
                {
                    string idTemp = (int.Parse(device.IndexLDPlayer) * 2 + 5555).ToString();
                    string deviceIdTemp = "127.0.0.1:" + idTemp;
                    LDController.ADB("index", device.IndexLDPlayer, $"disconnect {deviceIdTemp}");
                    LDController.ADB("index", device.IndexLDPlayer, $"connect {deviceIdTemp}");
                    AdbClient adbClient = new AdbClient();
                    adbClient.Connect(deviceIdTemp);
                    var data = adbClient.GetDevices().FirstOrDefault(x => x.Serial == deviceIdTemp);
                    if (data != null)
                    {
                        if (data.State == DeviceState.Online)
                        {
                            device.Id = data.Serial;
                            device.Data = data;
                            device.AdbClient = adbClient;
                            device.IndexLDPlayer = device.IndexLDPlayer;
                            return true;
                        }
                    }
                    Thread.Sleep(500);
                    count--;
                }

            }
            catch (Exception ex)
            {
                Log.Error($"ERROR: {nameof(DeviceHelper)}, params; {nameof(Connect)}, Error; {ex.Message}, Exception; {ex}");
                return false;
            }
            return false;
        }
        public static async Task<List<LDplayerInfo>> GetLDplayersAnysc()
        {
            List<LDplayerInfo> result = new List<LDplayerInfo>();
            try
            {
                var listLdplayer = LDController.GetDevices2();
                foreach (var ldplayer in listLdplayer)
                {
                    LDplayerInfo lDplayerInfo = new LDplayerInfo();
                    lDplayerInfo.Index = ldplayer.index.ToString();
                    lDplayerInfo.Name = ldplayer.name;
                    if (LDController.IsDevice_Running("index", lDplayerInfo.Index) == true)
                    {
                        lDplayerInfo.Status = "Online";
                        DeviceInfo device = new DeviceInfo();
                        device.IndexLDPlayer = lDplayerInfo.Index;
                        bool connect = true;
                        while (connect)
                        {
                            if (DeviceHelper.Connect(device))
                            {
                                connect = false;
                            }
                        }
                        lDplayerInfo.DeviceId = device.Id;
                    }
                    else
                    {
                        lDplayerInfo.Status = "Offline";
                    }
                    result.Add(lDplayerInfo);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return result;
            }
            return result;
        }
    }
}
