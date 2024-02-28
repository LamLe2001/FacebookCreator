using AdvancedSharpAdbClient;
using LDPlayerAndADBController.ADBClient;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace LDPlayerAndADBController.AdbController
{
    public class ADBClientController
    {
        public static void ClearTextElement(DeviceData deviceData, AdbClient adbClient, string xpath, int charCount)
        {
            try
            {
                int i = 0;
                while (i < 5)
                {
                    var dump = adbClient.DumpScreen(deviceData);
                    if (dump != null)
                    {
                        break;
                    }
                    else
                    {
                        ADBHelper.ExecuteADB_Result($"disconnect {deviceData.Serial}");
                        ADBHelper.ExecuteADB_Result($"connect {deviceData.Serial}");
                        adbClient = new AdbClient();
                        adbClient.Connect("127.0.0.1:62001");
                        deviceData = adbClient.GetDevices().FirstOrDefault(x => x.Serial == deviceData.Serial);
                    }
                    i++;
                }
                ClickElement(deviceData, adbClient, xpath, 30);
                adbClient.ClearInput(deviceData, charCount);
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBClientController)}, params; {nameof(ClearTextElement)},deviceId; {deviceData.Serial}, Error; {ex.Message}, Exception; {ex}");
            }
        }
        public static bool ClickElement(DeviceData deviceData, AdbClient adbClient, string xpath, int timeout)
        {
            try
            {
                int i = 0;
                while (i < 5)
                {
                    var dump = adbClient.DumpScreen(deviceData);
                    if (dump != null)
                    {
                        break;
                    }
                    else
                    {
                        ADBHelper.ExecuteADB_Result($"disconnect {deviceData.Serial}");
                        ADBHelper.ExecuteADB_Result($"connect {deviceData.Serial}");
                        adbClient = new AdbClient();
                        adbClient.Connect("127.0.0.1:62001");
                        deviceData = adbClient.GetDevices().FirstOrDefault(x => x.Serial == deviceData.Serial);
                    }
                    i++;
                }
                var element = FindElement(deviceData, adbClient, xpath, timeout);
                if (element.Y != 0 && element.X != 0)
                {
                    adbClient.Click(deviceData, element);
                    return true;
                }

            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBClientController)}, params; {nameof(ClickElement)},deviceId; {deviceData.Serial}, Error; {ex.Message}, Exception; {ex}");
            }
            return false;
        }
        public static bool ElementIsExist(DeviceData deviceData, AdbClient adbClient, string xpath, int timeout)
        {
            try
            {
                int i = 0;
                while (i < 5)
                {
                    var dump = adbClient.DumpScreen(deviceData);
                    if (dump != null)
                    {
                        break;
                    }
                    else
                    {
                        ADBHelper.ExecuteADB_Result($"disconnect {deviceData.Serial}");
                        ADBHelper.ExecuteADB_Result($"connect {deviceData.Serial}");
                        adbClient = new AdbClient();
                        adbClient.Connect("127.0.0.1:62001");
                        deviceData = adbClient.GetDevices().FirstOrDefault(x => x.Serial == deviceData.Serial);
                    }
                    i++;
                }
                for (int j = 0; j < timeout; i++)
                {
                    Element element = adbClient.FindElement(deviceData, "//node[@" + xpath + "]", TimeSpan.FromSeconds(3));
                    if (element != null)
                    {
                        return true;
                    }
                    Thread.Sleep(500);
                }

            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBClientController)}, params; {nameof(ElementIsExist)},deviceId; {deviceData.Serial}, Error; {ex.Message}, Exception; {ex}");
            }
            return false;
        }
        public static Cords FindElement(DeviceData deviceData, AdbClient adbClient, string xpath, int timeout)
        {
            Cords result = new Cords();
            try
            {
                int i = 0;
                while (i < 5)
                {
                    var dump = adbClient.DumpScreen(deviceData);
                    if (dump != null)
                    {
                        break;
                    }
                    else
                    {
                        ADBHelper.ExecuteADB_Result($"disconnect {deviceData.Serial}");
                        ADBHelper.ExecuteADB_Result($"connect {deviceData.Serial}");
                        adbClient = new AdbClient();
                        adbClient.Connect("127.0.0.1:62001");
                        deviceData = adbClient.GetDevices().FirstOrDefault(x => x.Serial == deviceData.Serial);
                    }
                    i++;
                }
                for (int j = 0; j < timeout; j++)
                {
                    Element element = adbClient.FindElement(deviceData, "//node[@" + xpath + "]", TimeSpan.FromSeconds(3));
                    if (element != null)
                    {
                        result = element.Cords;
                        return result;
                    }
                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBClientController)}, params; {nameof(FindElement)},deviceId; {deviceData.Serial}, Error; {ex.Message}, Exception; {ex}");
            }
            return result;
        }
        public static bool FindElementAndClickCondition(DeviceData deviceData, AdbClient adbClient, string xpath, string condition, int timeout = 30)
        {
            try
            {
                int i = 0;
                while (i < 5)
                {
                    var dump = adbClient.DumpScreen(deviceData);
                    if (dump != null)
                    {
                        break;
                    }
                    else
                    {
                        ADBHelper.ExecuteADB_Result($"disconnect {deviceData.Serial}");
                        ADBHelper.ExecuteADB_Result($"connect {deviceData.Serial}");
                        adbClient = new AdbClient();
                        adbClient.Connect("127.0.0.1:62001");
                        deviceData = adbClient.GetDevices().FirstOrDefault(x => x.Serial == deviceData.Serial);
                    }
                    i++;
                }
                var findElements = adbClient.FindElements(deviceData, "//node[@" + xpath + "]", TimeSpan.FromSeconds(3));
                if (findElements == null)
                {
                    return false;
                }
                foreach (var element in findElements)
                {
                    adbClient.Click(deviceData, element.Cords);
                    if (ElementIsExist(deviceData, adbClient, "//node[@" + condition + "]", 10) == true)
                    {
                        return true;
                    }
                    else
                    {
                        adbClient.BackBtn(deviceData);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBClientController)}, params; {nameof(FindElementAndClickCondition)},deviceId; {deviceData.Serial}, Error; {ex.Message}, Exception; {ex}");
            }

            return false;
        }
        public static void InputElement(DeviceData deviceData, AdbClient adbClient, string xpath, string text)
        {
            try
            {
                int i = 0;
                while (i < 5)
                {
                    var dump = adbClient.DumpScreen(deviceData);
                    if (dump != null)
                    {
                        break;
                    }
                    else
                    {
                        ADBHelper.ExecuteADB_Result($"disconnect {deviceData.Serial}");
                        ADBHelper.ExecuteADB_Result($"connect {deviceData.Serial}");
                        adbClient = new AdbClient();
                        adbClient.Connect("127.0.0.1:62001");
                        deviceData = adbClient.GetDevices().FirstOrDefault(x => x.Serial == deviceData.Serial);
                    }
                    i++;
                }
                ClickElement(deviceData, adbClient, xpath, 30);
                adbClient.SendText(deviceData, text);
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBClientController)}, params; {nameof(InputElement)},deviceId; {deviceData.Serial}, Error; {ex.Message}, Exception; {ex}");
            }
        }
        public static void SwipeElement(DeviceData deviceData, AdbClient adbClient, string xpathFirst, string xpathSecond, int timeout)
        {
            try
            {
                int i = 0;
                while (i < 5)
                {
                    var dump = adbClient.DumpScreen(deviceData);
                    if (dump != null)
                    {
                        break;
                    }
                    else
                    {
                        ADBHelper.ExecuteADB_Result($"disconnect {deviceData.Serial}");
                        ADBHelper.ExecuteADB_Result($"connect {deviceData.Serial}");
                        adbClient = new AdbClient();
                        adbClient.Connect("127.0.0.1:62001");
                        deviceData = adbClient.GetDevices().FirstOrDefault(x => x.Serial == deviceData.Serial);
                    }
                    i++;
                }
                var start = adbClient.FindElement(deviceData, "//node[@" + xpathFirst + "]", TimeSpan.FromSeconds(10));
                var end = adbClient.FindElement(deviceData, "//node[@" + xpathSecond + "]", TimeSpan.FromSeconds(10));
                if (start != null && end != null)
                {
                    adbClient.Swipe(deviceData, start, end, timeout);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBClientController)}, params; {nameof(SwipeElement)},deviceId; {deviceData.Serial}, Error; {ex.Message}, Exception; {ex}");
            }

        }
        public static List<Element> FindElements(DeviceData deviceData, AdbClient adbClient, string xpath, int timeout)
        {
            List<Element> result = new List<Element>();
            try
            {
                for (int i = 0; i < timeout; i++)
                {
                    var elements = adbClient.FindElements(deviceData, "//node[@" + xpath + "]", TimeSpan.FromSeconds(3));
                    result.Clear();
                    if (elements != null)
                    {
                        foreach (Element element in elements)
                        {
                            result.Add(element);
                        }
                        if (result.Count > 0)
                        {
                            return result;
                        }
                    }
                    Thread.Sleep(500);
                }

            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBClientController)}, params; {nameof(FindElements)},deviceId; {deviceData.Serial}, Error; {ex.Message}, Exception; {ex}");
            }
            return result;
        }
        public static bool ClickButton(DeviceData deviceData, AdbClient adbClient, string name, int timeout = 3)
        {
            try
            {
                int i = 0;
                while (i < 5)
                {
                    var dump = adbClient.DumpScreen(deviceData);
                    if (dump != null)
                    {
                        break;
                    }
                    else
                    {
                        ADBHelper.ExecuteADB_Result($"disconnect {deviceData.Serial}");
                        ADBHelper.ExecuteADB_Result($"connect {deviceData.Serial}");
                        adbClient = new AdbClient();
                        adbClient.Connect("127.0.0.1:62001");
                        deviceData = adbClient.GetDevices().FirstOrDefault(x => x.Serial == deviceData.Serial);
                    }
                    i++;
                }
                for (int j = 0; j < timeout; j++)
                {
                    var elements = adbClient.FindElements(deviceData, "//node[@class='android.widget.Button']", TimeSpan.FromSeconds(3));
                    if (elements != null && elements.Count() > 0)
                    {
                        foreach (Element element in elements)
                        {
                            if (element.Attributes.ContainsValue(name))
                            {
                                element.Click();
                                return true;
                            }
                        }
                    }
                    Thread.Sleep(500);
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBClientController)}, params; {nameof(ClickButton)},deviceId; {deviceData.Serial}, Error; {ex.Message}, Exception; {ex}");
                return false;
            }
        }
        public static bool FindElementIsExistOrClickByClass(DeviceData deviceData, AdbClient adbClient, string name, string nameClass, int timeout = 3, bool isClick = false)
        {
            try
            {
                int i = 0;
                while (i < 5)
                {
                    var dump = adbClient.DumpScreen(deviceData);
                    if (dump != null)
                    {
                        break;
                    }
                    else
                    {
                        ADBHelper.ExecuteADB_Result($"disconnect {deviceData.Serial}");
                        ADBHelper.ExecuteADB_Result($"connect {deviceData.Serial}");
                        adbClient = new AdbClient();
                        adbClient.Connect("127.0.0.1:62001");
                        deviceData = adbClient.GetDevices().FirstOrDefault(x => x.Serial == deviceData.Serial);
                    }
                    i++;
                }
                for (int j = 0; j < timeout; j++)
                {
                    var elements = adbClient.FindElements(deviceData, $"//node[@class='{nameClass}']", TimeSpan.FromSeconds(3));
                    if (elements != null && elements.Count() > 0)
                    {
                        foreach (Element element in elements)
                        {
                            if (element.Attributes.ContainsValue(name))
                            {
                                element.Click();
                                return true;
                            }
                        }
                    }
                    Thread.Sleep(500);
                }
                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"End {nameof(ADBClientController)}, params; {nameof(ClickButton)},deviceId; {deviceData.Serial}, Error; {ex.Message}, Exception; {ex}");
                return false;
            }
        }
    }
}
