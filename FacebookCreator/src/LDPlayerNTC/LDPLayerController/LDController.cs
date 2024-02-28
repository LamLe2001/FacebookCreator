using Emgu.CV.Flann;
using LDPlayerAndADBController.Helper;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Emgu.CV.DISOpticalFlow;
using static System.Net.Mime.MediaTypeNames;

namespace LDPlayerAndADBController
{
    public static class LDController
    {
        //-----------Path của ldconsole----------//

        public static string PathFolderLDPlayer { get; set; }
        /// <summary>
        /// /////////////
        /// </summary>
        /// <param name="cmdCommand"></param>
        /// <returns></returns>

        public static string ExecuteCMD_Result(string cmdCommand, int timeoutMilliseconds = 60000)
        {
            string result;
            var process = new Process();
            try
            {
                using (process)
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = $"{PathFolderLDPlayer}\\adb.exe",
                        Arguments = cmdCommand,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true
                    };
                    // Create a new Timer with the specified timeout value.
                    using (var processCancellationTokenSource = new CancellationTokenSource())
                    using (var timer = new Timer(state => processCancellationTokenSource.Cancel(), null, timeoutMilliseconds, Timeout.Infinite))
                    {
                        process.Start();
                        process.WaitForExit();

                        if (!processCancellationTokenSource.IsCancellationRequested)
                        {
                            result = process.StandardOutput.ReadToEnd();
                            process.Close();
                            return result;
                        }
                        else
                        {
                            // Handle timeout
                            result = "Timeout occurred";
                        }
                    }
                }
            }
            catch
            {
                result = null;
            }
            process.Close();
            process.Dispose();
            return result;
        }

        /// <summary>
        /// /////////        p.StandardInput.Close();
        /// </summary>
        /// <param name="cmd"></param>
        //----------CMD-------------------//
        public static void ExecuteLD(string cmd)
        {
            Process p = new Process();
            p.StartInfo.FileName = $"{PathFolderLDPlayer}\\ldconsole.exe";
            p.StartInfo.Arguments = cmd;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.EnableRaisingEvents = true;
            p.Start();
            p.WaitForExit();
            p.Close();
            p.Dispose();
        }

        public static void SortWnd()
        {
            ExecuteLD("sortWnd");
        }
        public static string ExecuteLD_Result(string cmdCommand, int timeoutMilliseconds = 60000)
        {
            string result;
            int retryCount = 0;
            const int maxRetryCount = 3;
            const int retryIntervalMilliseconds = 20000; // 20 seconds
            var process = new Process();
            try
            {

                do
                {
                    using (process)
                    {
                        process.StartInfo = new ProcessStartInfo
                        {
                            FileName = $"{PathFolderLDPlayer}\\ldconsole.exe",
                            Arguments = cmdCommand,
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            RedirectStandardInput = true,
                            RedirectStandardOutput = true
                        };

                        using (var processCancellationTokenSource = new CancellationTokenSource())
                        using (var timer = new Timer(state => processCancellationTokenSource.Cancel(), null, timeoutMilliseconds, Timeout.Infinite))
                        {
                            process.Start();
                            process.WaitForExit();

                            if (!processCancellationTokenSource.IsCancellationRequested)
                            {
                                result = process.StandardOutput.ReadToEnd();
                                process.Close();
                                process.Dispose();
                                return result;
                            }
                            else
                            {
                                // Handle timeout
                                result = "Timeout occurred";
                            }
                        }
                    }

                    if (result == null || string.IsNullOrEmpty(result.Trim()) || result == "Timeout occurred")
                    {
                        // Retry after 20 seconds
                        retryCount++;
                        if (retryCount <= maxRetryCount)
                        {
                            Thread.Sleep(retryIntervalMilliseconds);
                        }
                    }
                    else
                    {
                        break; // Break the loop if the result is not null or empty
                    }
                } while (retryCount <= maxRetryCount);
            }
            catch
            {
                result = null;
                process.Close();
                process.Dispose();
            }
            process.Close();
            process.Dispose();
            return result;
        }

        //----------Tương tác với các tab giả lập ---------//

        /// <summary>
        /// Mở giả lập
        /// </summary>
        /// <param name="param">
        /// name : Nếu điều khiển theo tên giả lập
        /// index : Nếu điều khiển theo index giả lập</param>
        /// <param name="NameOrIndex">Giá trị tên hoặc index giả lập</param>
        public static void Open(string param, string NameOrIndex, bool isChange = true)
        {
            if (isChange)
            {
                string pathDevices = Path.Combine(Environment.CurrentDirectory, "LDPlayer\\devices.json");
                string jsonString = System.IO.File.ReadAllText(pathDevices);
                JObject jsonObject = JObject.Parse(jsonString);
                var random = new Random();
                var randomIndex = random.Next(jsonObject.Count);
                var randomKey = jsonObject.Properties().ElementAt(randomIndex).Name;
                var randomElement = jsonObject[randomKey].ToString();
                JsonHelper jsonHelper = new JsonHelper(randomElement, true);
                var manufacturer = jsonHelper.GetValuesFromInputString("manufacturer");
                var model = jsonHelper.GetValuesFromInputString("model");
                var phone = Helpers.GenerateRandomString("123456789", 1);
                phone = phone + Helpers.GenerateRandomString("0123456789", 8);
                phone = $"+84{phone}";
                Change_Property(param, NameOrIndex, $"--manufacturer {manufacturer} --model {model} --pnumber {phone} --imei auto --imsi auto --simserial auto --androidid auto --mac auto");
            }
            ExecuteLD(string.Format("launch --{0} {1}", param, NameOrIndex));
        }


        /// <summary>
        /// Kiểm tra LD đã khởi động xong chưa
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <returns></returns>
        public static bool CheckLDStartDone(string param, string NameOrIndex)
        {
            string IdTemp = (int.Parse(NameOrIndex) * 2 + 5555).ToString();
            string DeviceIdTemp = "127.0.0.1:" + IdTemp; ;
            string str = string.Format("adb --{0} {1} --command \"{2}\"", param, NameOrIndex, "shell input tap 0 0");
            int num = 0;
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                FileName = $"{PathFolderLDPlayer}\\ldconsole.exe",
                Arguments = str,
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };
            do
            {
                process.Start();
                process.WaitForExit(3000);
                var check = process.StandardOutput.ReadToEnd();
                if (!(check == ""))
                {
                    Thread.Sleep(2000);
                    ++num;
                    LDController.ExecuteCMD_Result("devices ");
                    LDController.ExecuteCMD_Result("disconnect " + DeviceIdTemp);
                    LDController.ExecuteCMD_Result("connect " + DeviceIdTemp);
                }
                else
                {
                    goto label_1;
                }
            }
            while (num != 30);
            goto label_3;
        label_1:
            return true;
        label_3:
            return false;
        }
        /// <summary>
        /// Khởi động ứng dụng
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="Package_Name">Tên package ứng dụng</param>
        public static void Open_App(string param, string NameOrIndex, string Package_Name)
        {
            ExecuteLD(string.Format("launchex --{0} {1} --packagename {2}", param, NameOrIndex, Package_Name));
        }

        /// <summary>
        /// Đóng giả lập
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        public static void Close(string param, string NameOrIndex)
        {
            try
            {
                string IdTemp = (int.Parse(NameOrIndex) * 2 + 5555).ToString();
                string DeviceIdTemp = "127.0.0.1:" + IdTemp; ;
                LDController.ExecuteCMD_Result(" disconnect " + DeviceIdTemp);
                ExecuteLD(string.Format("quit --{0} {1}", param, NameOrIndex));
                return;
            }
            catch (Exception ex)
            {
                Log.Information(" LDController");
                Log.Error(ex.Message);
            }
        }

        /// <summary>
        /// Đóng tất cả giả lập
        /// </summary>
        public static void CloseAll()
        {
            ExecuteLD("quitall");
        }

        /// <summary>
        /// Khởi động lại giả lập
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        public static void ReBoot(string param, string NameOrIndex, bool Ischange = true)
        {
            string pathDevices = Path.Combine(Environment.CurrentDirectory, "LDPlayer\\devices.json");
            string jsonString = System.IO.File.ReadAllText(pathDevices);
            JObject jsonObject = JObject.Parse(jsonString);
            var random = new Random();
            var randomIndex = random.Next(jsonObject.Count);
            var randomKey = jsonObject.Properties().ElementAt(randomIndex).Name;
            var randomElement = jsonObject[randomKey].ToString();
            JsonHelper jsonHelper = new JsonHelper(randomElement, true);
            var manufacturer = jsonHelper.GetValuesFromInputString("manufacturer");
            var model = jsonHelper.GetValuesFromInputString("model");
            var phone = Helpers.GenerateRandomString("123456789", 1);
            phone = phone + Helpers.GenerateRandomString("0123456789", 8);
            phone = $"+84{phone}";
            Change_Property(param, NameOrIndex, $"--manufacturer {manufacturer} --model {model} --pnumber {phone} --imei auto --imsi auto --simserial auto --androidid auto --mac auto");
            ExecuteLD(string.Format("reboot --{0} {1}", param, NameOrIndex));
        }

        //------------Tùy chỉnh thông tin giả lập--------------//

        /// <summary>
        /// Tạo giả lập
        /// </summary>
        /// <param name="Name"></param>
        public static void Create(string Name)
        {
            ExecuteLD("add --name " + Name);
        }

        /// <summary>
        /// Copy giả lập
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="From_NameOrIndex"></param>
        public static void Copy(string Name, string From_NameOrIndex)
        {
            ExecuteLD(string.Format("copy --name \"{0}\" --from {1}", Name, From_NameOrIndex));
        }

        /// <summary>
        /// Xóa giả lập
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        public static void Delete(string param, string NameOrIndex)
        {
            ExecuteLD(string.Format("remove --{0} {1}", param, NameOrIndex));
        }

        /// <summary>
        /// Thay đổi tên giả lập
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="title_new"></param>
        public static void ReName(string param, string NameOrIndex, string title_new)
        {
            ExecuteLD(string.Format("rename --{0} {1} --title {2}", param, NameOrIndex, title_new));
        }

        //-----------Tùy chỉnh sâu giả lập--------------//

        /// <summary>
        /// Install app từ File trên PC
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="File_Name"></param>
        public static void InstallApp_File(string param, string NameOrIndex, string File_Name)
        {
            ExecuteLD_Result(string.Format(@"installapp --{0} {1} --filename ""{2}""", param, NameOrIndex, File_Name));
        }

        /// <summary>
        /// Install App từ Package
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="Package_Name"></param>
        public static void InstallApp_Package(string param, string NameOrIndex, string Package_Name)
        {
            ExecuteLD(string.Format("installapp --{0} {1} --packagename {2}", param, NameOrIndex, Package_Name));
        }

        /// <summary>
        /// UnInstall App
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="Package_Name"></param>
        public static void UnInstallApp(string param, string NameOrIndex, string Package_Name)
        {
            ExecuteLD(string.Format("uninstallapp --{0} {1} --packagename {2}", param, NameOrIndex, Package_Name));
        }

        /// <summary>
        /// Run App
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="Package_Name"></param>
        public static void RunApp(string param, string NameOrIndex, string Package_Name)
        {
            ExecuteLD(string.Format("runapp --{0} {1} --packagename {2}", param, NameOrIndex, Package_Name));
        }

        /// <summary>
        /// Kill App
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="Package_Name"></param>
        public static void KillApp(string param, string NameOrIndex, string Package_Name)
        {
            ExecuteLD(string.Format("killapp --{0} {1} --packagename {2}", param, NameOrIndex, Package_Name));
        }

        /// <summary>
        /// Thay đổi Property giả lập
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrId"></param>
        /// <param name="cmd">thông số của giả lập
        ///[--resolution ]
        ///[--cpu < 1 | 2 | 3 | 4 >]
        ///[--memory < 512 | 1024 | 2048 | 4096 | 8192 >]
        ///[--manufacturer asus]
        ///[--model ASUS_Z00DUO]
        ///[--pnumber 13812345678]
        ///[--imei ]
        ///[--imsi ]    
        ///[--simserial ]
        ///[--androidid ]
        ///[--mac ]
        ///[--autorotate < 1 | 0 >]
        ///[--lockwindow < 1 | 0 >]
        /// </param>
        public static void Change_Property(string param, string NameOrIndex, string cmd)
        {
            ExecuteLD(string.Format("modify --{0} {1} {2}", param, NameOrIndex, cmd));
        }

        /// <summary>
        /// ADB
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrId"></param>
        /// <param name="cmd">cmd adb</param>
        /// <returns>Result cmd</returns>
        public static string ADB(string param, string NameOrIndex, string cmd)
        {
            try
            {
                return ExecuteLD_Result(string.Format("adb --{0} {1} --command \"{2}\"", param, NameOrIndex, cmd));
            }
            catch (Exception ex)
            {
                Log.Information(" LDController");
                Log.Error(ex.Message);
            }
            return null;
        }
        /// <summary>
        /// Pull file
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="remote_file_path"></param>
        /// <param name="local_file_path"></param>
        public static void Pull(string param, string NameOrIndex, string remote_file_path, string local_file_path)
        {
            ExecuteLD_Result(string.Format(@"pull --{0} {1} --remote ""{2}"" --local ""{3}""", param, NameOrIndex, remote_file_path, local_file_path));
        }

        /// <summary>
        /// Push file
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="remote_file_path"></param>
        /// <param name="local_file_path"></param>
        public static void Push(string param, string NameOrIndex, string remote_file_path, string local_file_path)
        {
            ExecuteLD_Result(string.Format(@"push --{0} {1} --remote ""{2}"" --local ""{3}""", param, NameOrIndex, remote_file_path, local_file_path));
        }

        /// <summary>
        /// Backup File
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="Package_Name"></param>
        /// <param name="file_path"></param>
        public static void BackupApp(string param, string NameOrIndex, string Package_Name, string file_path)
        {
            ExecuteLD_Result(string.Format(@"backupapp --{0} {1} --packagename {2} --file ""{3}""", param, NameOrIndex, Package_Name, file_path));
        }

        /// <summary>
        /// Restore App
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="Package_Name"></param>
        /// <param name="file_path"></param>
        public static void RestoreApp(string param, string NameOrIndex, string Package_Name, string file_path)
        {
            ExecuteLD(string.Format(@"restoreapp --{0} {1} --packagename {2} --file ""{3}""", param, NameOrIndex, Package_Name, file_path));
        }

        /// <summary>
        /// Global config
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="fps"></param>
        /// <param name="audio"></param>
        /// <param name="fast_play"></param>
        /// <param name="clean_mode"></param>
        public static void Global_Config(string param, string NameOrIndex, string fps, string audio, string fast_play, string clean_mode)
        {
            //  [--fps <0~60>] [--audio <1 | 0>] [--fastplay <1 | 0>] [--cleanmode <1 | 0>]
            ExecuteLD(string.Format("globalsetting --{0} {1} --audio {2} --fastplay {3} --cleanmode {4}", param, NameOrIndex, audio, fast_play, clean_mode));
        }

        //---------Lấy thông tin multi giả lập-------------//

        /// <summary>
        /// Lấy danh sách giả lập
        /// </summary>
        /// <returns>List tên các giả lập</returns>
        public static List<string> GetDevices()
        {
            string[] arr = ExecuteLD_Result("list").Trim().Split('\n');
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == "")
                    return new List<string>();
                arr[i] = arr[i].Trim();
            }
            //System.Windows.Forms.MessageBox.Show(string.Join("|", arr));
            return arr.ToList<string>();
        }

        /// <summary>
        /// List tên giả lập đang chạy
        /// </summary>
        /// <returns>List tên các giả lập đang chạy</returns>
        public static List<string> GetDevices_Running()
        {
            string[] arr = ExecuteLD_Result("runninglist").Trim().Split('\n');
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == "")
                    return new List<string>();
                arr[i] = arr[i].Trim();
            }
            return arr.ToList<string>();
        }

        /// <summary>
        /// List chi tiết giả lập đang chạy
        /// </summary>
        /// <returns></returns>
        public static List<Info_Devices> GetDevices2_Running()
        {
            List<Info_Devices> list_running = new List<Info_Devices>();
            string[] arr = ExecuteLD_Result("runninglist").Trim().Split('\n');
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != "")
                {
                    Info_Devices item = new Info_Devices();
                    item.name = arr[i].Trim();
                    item.index = Int32.Parse(GetNameOrIndex("name", item.name));
                    list_running.Add(item);
                }
            }
            return list_running;
        }

        /// <summary>
        /// Từ Index hoặc Tên của giả lập đưa ra gia trị còn lại
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <returns>String index hoặc tên</returns>
        public static string GetNameOrIndex(string param, string NameOrIndex)
        {
            List<Info_Devices> listDevices = GetDevices2();
            if (param == "index")
            {
                foreach (Info_Devices item in listDevices)
                {
                    if (item.index.ToString() == NameOrIndex)
                    {
                        return item.name.ToString();
                    }
                }
            }
            if (param == "name")
            {
                foreach (Info_Devices item in listDevices)
                {
                    if (item.name == NameOrIndex)
                    {
                        return item.index.ToString();
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Kiểm tra xem giả lập có đang chạy không
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <returns></returns>
        public static bool IsDevice_Running(string param, string NameOrIndex)
        {
            string result = ExecuteLD_Result(string.Format("isrunning --{0} {1}", param, NameOrIndex)).Trim();
            if (result == "running")
                return true;
            return false;
        }

        /// <summary>
        /// List2 chi tiết các giả lập
        /// </summary>
        /// <returns></returns>
        public static List<Info_Devices> GetDevices2()
        {
            try
            {
                List<Info_Devices> listLDPlayer = new List<Info_Devices>();
                string[] arr = ExecuteLD_Result("list2").Trim().Split('\n');
                for (int i = 0; i < arr.Length; i++)
                {
                    Info_Devices devices = new Info_Devices();
                    string[] aDetail = arr[i].Trim().Split(',');
                    devices.index = int.Parse(aDetail[0]);
                    devices.name = aDetail[1];
                    devices.adb_id = "-1";
                    listLDPlayer.Add(devices);
                }
                //System.Windows.Forms.MessageBox.Show(string.Join("\n", arr));
                return listLDPlayer;
            }
            catch
            {
                return new List<Info_Devices>();
            }
        }


        //--------------------Viết lại KautoHelper------------------//


        private static string TAP_DEVICES = "shell input tap {0} {1}";
        private static string SWIPE_DEVICES = "shell input swipe {0} {1} {2} {3} {4}";
        private static string KEY_DEVICES = "shell input keyevent {0}";
        private static string INPUT_TEXT_DEVICES = "shell input text \"{0}\"";
        private static string CAPTURE_SCREEN_TO_DEVICES = "shell screencap -p \"{0}\"";
        private static string PULL_SCREEN_FROM_DEVICES = "pull \"{0}\"";
        private static string REMOVE_SCREEN_FROM_DEVICES = "shell rm -f \"{0}\"";
        private static string GET_SCREEN_RESOLUTION = "shell dumpsys display | grep mCurrentDisplayRect";
        private static string CLEARCACHES = "shell pm clear {0}";
        /// <summary>
        /// Delay
        /// </summary>
        /// <param name="delayTime"></param>
        public static async Task DelayAsync(int start = 8, int end = 12)
        {
            Random radom = new Random();
            var delayTime = radom.Next(start, end);
            await Task.Delay(delayTime * 1000);
        }
        public static async Task DelayAsync(int delayTime)
        {
            await Task.Delay(delayTime * 1000);
        }

        /// <summary>
        /// Lấy kích thước màn hình
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <returns></returns>
        public static Point GetScreenResolution(string param, string NameOrIndex)
        {
            string cmdCommand = string.Format(GET_SCREEN_RESOLUTION);
            string result = LDController.ADB(param, NameOrIndex, cmdCommand);
            result = result.Substring(result.IndexOf("- "));
            result = result.Substring(result.IndexOf(' '), result.IndexOf(')') - result.IndexOf(' '));
            string[] temp = result.Split(new char[]
            {
                ','
            });
            int x = Convert.ToInt32(temp[0].Trim());
            int y = Convert.ToInt32(temp[1].Trim());
            return new Point(x, y);
        }

        /// <summary>
        /// Click tọa độ phần trăm
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="count"></param>
        public static void TapByPercent(string param, string NameOrIndex, double x, double y, int count = 1)
        {
        Start:
            try
            {
                Point resolution = LDController.GetScreenResolution(param, NameOrIndex);
                int X = (int)(x * ((double)resolution.X * 1.0 / 100.0));
                int Y = (int)(y * ((double)resolution.Y * 1.0 / 100.0));
                string cmdCommand = string.Format(LDController.TAP_DEVICES, X, Y);
                for (int i = 1; i < count; i++)
                {
                    cmdCommand = cmdCommand + " && " + string.Format(LDController.TAP_DEVICES, X, Y);
                }
                string result = LDController.ADB(param, NameOrIndex, cmdCommand);
            }
            catch (Exception)
            {
                goto Start;
            }

        }

        /// <summary>
        /// Click theo tọa độ
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="count"></param>
        public static void Tap(string param, string NameOrIndex, int x, int y, int count = 1)
        {
            string cmdCommand = string.Format(LDController.TAP_DEVICES, x, y);
            for (int i = 1; i < count; i++)
            {
                cmdCommand = cmdCommand + " && " + string.Format(LDController.TAP_DEVICES, x, y);
            }
            string result = LDController.ADB(param, NameOrIndex, cmdCommand);
        }
        public static void ClearCaches(string param, string NameOrIndex, string Package_Name)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (true)
            {
                string cmdCommand = string.Format(LDController.CLEARCACHES, Package_Name);
                string result = LDController.ADB(param, NameOrIndex, cmdCommand);
                if (result.Contains("Success"))
                {
                    sw.Stop();
                    return;
                }
                if (sw.ElapsedMilliseconds > 60000)
                {
                    sw.Stop(); return;
                }
            }
        }
        /// <summary>
        /// Send Key
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="key"></param>
        public static void Key(string param, string NameOrIndex, ADBKeyEvent key)
        {
            string cmdCommand = string.Format(LDController.KEY_DEVICES, key);
            string result = LDController.ADB(param, NameOrIndex, cmdCommand);
        }

        /// <summary>
        /// Input Text
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="text"></param>
        public static void InputText(string param, string NameOrIndex, string text)
        {
            string cmdCommand = string.Format(LDController.INPUT_TEXT_DEVICES, text.Replace(" ", "%s").Replace("&", "\\&").Replace("<", "\\<").Replace(">", "\\>").Replace("?", "\\?").Replace(":", "\\:").Replace("{", "\\{").Replace("}", "\\}").Replace("[", "\\[").Replace("]", "\\]").Replace("|", "\\|"));
            string result = LDController.ADB(param, NameOrIndex, cmdCommand);
        }
        /// <summary>
        /// Kéo theo phần trăm
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="duration"></param>
        public static void SwipeByPercent(string param, string NameOrIndex, double x1, double y1, double x2, double y2, int? duration = 100, int? Count = 1)
        {
        Start:
            try
            {
                Point resolution = LDController.GetScreenResolution(param, NameOrIndex);
                int X = (int)(x1 * ((double)resolution.X * 1.0 / 100.0));
                int Y = (int)(y1 * ((double)resolution.Y * 1.0 / 100.0));
                int X2 = (int)(x2 * ((double)resolution.X * 1.0 / 100.0));
                int Y2 = (int)(y2 * ((double)resolution.Y * 1.0 / 100.0));
                string cmdCommand = string.Format(LDController.SWIPE_DEVICES, new object[]
                {
                    X,
                    Y,
                    X2,
                    Y2,
                    duration
                });
                for (int i = 0; i < Count; i++)
                {
                    string result = LDController.ADB(param, NameOrIndex, cmdCommand);
                }
            }
            catch (Exception)
            {
                goto Start;
            }

        }
        /// <summary>
        /// Kéo
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="duration"></param>
        public static void Swipe(string param, string NameOrIndex, int x1, int y1, int x2, int y2, int duration = 100)
        {
            string cmdCommand = string.Format(LDController.SWIPE_DEVICES, new object[]
            {
                x1,
                y1,
                x2,
                y2,
                duration
            });
            string result = LDController.ADB(param, NameOrIndex, cmdCommand);
        }

        /// <summary>
        /// Bật chế độ trên máy bay
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="cancellationToken"></param>
        public static void PlanModeON(string param, string NameOrIndex, CancellationToken cancellationToken)
        {
            bool isCancellationRequested = cancellationToken.IsCancellationRequested;
            if (!isCancellationRequested)
            {
                string cmdClearShoppe = "settings put global airplane_mode_on 1";
                cmdClearShoppe = string.Concat(new string[]
                {
                    cmdClearShoppe,
                    Environment.NewLine,
                    " am broadcast -a android.intent.action.AIRPLANE_MODE"
                });
                LDController.ADB(param, NameOrIndex, cmdClearShoppe);
            }
        }
        /// <summary>
        /// Tắt chế độ trên máy bay
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="cancellationToken"></param>
        public static void PlanModeOFF(string param, string NameOrIndex, CancellationToken cancellationToken)
        {
            bool isCancellationRequested = cancellationToken.IsCancellationRequested;
            if (!isCancellationRequested)
            {
                string cmdClearShoppe = "settings put global airplane_mode_on 0";
                cmdClearShoppe = string.Concat(new string[]
                {
                    cmdClearShoppe,
                    Environment.NewLine,
                    " am broadcast -a android.intent.action.AIRPLANE_MODE"
                });
                LDController.ADB(param, NameOrIndex, cmdClearShoppe);
            }
        }
        /// <summary> 
        /// LongPress
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="duration">Thời gian dí</param>
        public static void LongPress(string param, string NameOrIndex, int x, int y, int duration = 100)
        {
            string cmdCommand = string.Format(LDController.SWIPE_DEVICES, new object[]
            {
                x,
                y,
                x,
                y,
                duration
            });
            string result = LDController.ADB(param, NameOrIndex, cmdCommand);
        }
        /// <summary>
        /// Chụp ảnh màn hình
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="isDeleteImageAfterCapture"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Bitmap ScreenShoot(string param, string NameOrIndex, bool isDeleteImageAfterCapture = true, string fileName = "screenShoot.png")
        {
            string IdTemp = (int.Parse(NameOrIndex) * 2 + 5555).ToString();
            fileName = "screenShoot_" + IdTemp + ".png";
            string nameToSave = Path.GetFileNameWithoutExtension(fileName) + NameOrIndex + Path.GetExtension(fileName);
            for (; ; )
            {
                bool flag3 = File.Exists(nameToSave);
                bool flag6 = !flag3;
                if (flag6)
                {
                    break;
                }
                try
                {
                    File.Delete(nameToSave);
                    break;
                }
                catch (Exception ex)
                {
                    break;
                }
            }
            string Current = Directory.GetCurrentDirectory() + "\\" + nameToSave;
            string CurrentPath = Directory.GetCurrentDirectory().Replace("\\\\", "\\");
            CurrentPath = "\"" + CurrentPath + "\"";
            string cmdCommand = string.Format("shell screencap -p \"{0}\"", "/sdcard/" + nameToSave);
            string cmdCommand2 = string.Format(string.Concat(new string[]
            {
                " pull /sdcard/",
                nameToSave,
                " ",
                CurrentPath
            }), new object[0]);
            string result = ADB(param, NameOrIndex, cmdCommand);
            string result2 = ADB(param, NameOrIndex, cmdCommand2);
            Bitmap result3 = null;
            try
            {
                using (Bitmap bitmap = new Bitmap(Current))
                {
                    result3 = new Bitmap(bitmap);
                }
            }
            catch
            {
            }
            if (isDeleteImageAfterCapture)
            {
                try
                {
                    File.Delete(nameToSave);
                }
                catch
                {
                }
            }
            return result3;
        }

        // ----------------------- Xử lý Ảnh -----------------------//

        /// <summary>
        /// Kiểm tra xem ảnh có tồn tại
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="pathPNG">Đường dẫn ảnh ảnh</param>
        /// <returns></returns>
        public static bool FindImage(string param, string NameOrIndex, string pathPNG, double precent = 0.4, int timeout = 60000)
        {
            bool resutl = false;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (true)
            {
                Bitmap subBitmap = ImageScanOpenCV.GetImage(pathPNG);
                Bitmap screen = LDController.ScreenShoot(param, NameOrIndex);
                Point? poinImage = ImageScanOpenCV.FindOutPoint(screen, subBitmap, precent);
                if (poinImage != null)
                {
                    resutl = true;
                    sw.Stop(); break;
                }
                if (sw.ElapsedMilliseconds > timeout)
                {
                    resutl = false;
                    sw.Stop(); break;
                }
            }
            return resutl;

        }
        /// <summary>
        /// Tìm vị trí hình ảnh
        /// </summary>
        /// <param name="param"></param>
        /// <param name="NameOrIndex"></param>
        /// <param name="pathPNG"></param>
        /// <returns></returns>
        public static async Task<bool> FindImageTap(string param, string NameOrIndex, string pathPNG, double precent = 0.4, int timeout = 60000)
        {
            bool resutl = false;
            try
            {
                //Set the timeout for the image search(in milliseconds)
                Stopwatch sw = new Stopwatch();
                sw.Start();
                while (true)
                {
                    Bitmap subBitmap = ImageScanOpenCV.GetImage(pathPNG);
                    Bitmap screen = LDController.ScreenShoot(param, NameOrIndex);
                    Point? poinImage = ImageScanOpenCV.FindOutPoint(screen, subBitmap, precent);
                    if (poinImage != null)
                    {
                        Tap(param, NameOrIndex, poinImage.Value.X, poinImage.Value.Y);
                        sw.Stop();
                        ADB(param, NameOrIndex, "shell rm " + screen);
                        resutl = true;
                        return resutl;
                    }
                    if (sw.ElapsedMilliseconds > timeout)
                    {
                        //Close(param, NameOrIndex);
                        sw.Stop();
                        resutl = false;
                        return resutl;
                    }
                    await DelayAsync(2);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
            return resutl;
        }
        public static async Task<Point?> FindImageReturnPoint(string param, string NameOrIndex, string pathPNG, double precent = 0.4, int timeout = 60000)
        {
            //Set the timeout for the image search(in milliseconds)
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (true)
            {
                Bitmap subBitmap = ImageScanOpenCV.GetImage(pathPNG);
                Bitmap screen = LDController.ScreenShoot(param, NameOrIndex);
                Point? poinImage = ImageScanOpenCV.FindOutPoint(screen, subBitmap, precent);
                if (poinImage != null)
                {
                    return poinImage;
                }
                if (sw.ElapsedMilliseconds > timeout)
                {
                    //Close(param, NameOrIndex);
                    sw.Stop();
                    break;
                }
                await DelayAsync(1);
            }
            return null;
        }
        public static void addProxy(string param, string NameOrIndex, string ip, string port)
        {
            LDController.ADB(param, NameOrIndex, "shell settings put global http_proxy " + ip + ":" + port);
            LDController.ADB(param, NameOrIndex, "shell settings put global global_http_proxy_host " + ip);
            LDController.ADB(param, NameOrIndex, "shell settings put global global_http_proxy_port " + port);
        }

        public static void removeProxy(string param, string NameOrIndex)
        {
            LDController.ADB(param, NameOrIndex, "shell settings put global http_proxy :0");
        }
        public static void InputTextWithADBKeyboard(string index, string text)
        {
            int i = 0;
            while (i < 10)
            {
                LDController.ADB("index", index, "shell ime set com.android.adbkeyboard/.AdbIME");
                Thread.Sleep(1000);
                var convert = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
                string cmdCommand = $"shell am broadcast -a ADB_INPUT_B64 --es msg '{convert}'";
                if (LDController.ADB("index", index, cmdCommand).Contains("result=0"))
                {
                    return;
                }
                i++;
            }
        }
        public static void ClearInputWithADBKeyboard(string index)
        {
            int i = 0;
            while (i < 10)
            {
                LDController.ADB("index", index, "shell ime set com.android.adbkeyboard/.AdbIME");
                Thread.Sleep(1000);
                if (LDController.ADB("index", index, "shell am broadcast -a ADB_CLEAR_TEXT").Contains("result=0"))
                {
                    return;
                }
                i++;
            }
        }
        public static void EditFileConfigLDPlayer()
        {
            string pathConfig = PathFolderLDPlayer + "\\vms\\config\\leidians.config";
            if (File.Exists(pathConfig))
            {
                Helpers.SetValueInJsonFile(pathConfig, "languageId", "en_US");
                Helpers.SetValueInJsonFile(pathConfig, "exitFullscreenEsc", true);
                Helpers.SetValueInJsonFile(pathConfig, "productLanguageId", "en_US");
                Helpers.SetValueInJsonFile(pathConfig, "batchNewCount", 10);
                Helpers.SetValueInJsonFile(pathConfig, "batchCloneCount", 10);
                Helpers.SetValueInJsonFile(pathConfig, "framesPerSecond", 40);
                Helpers.SetValueInJsonFile(pathConfig, "windowsRowCount", 8);
            }
        }
        public static void SettingLDPlayerByIndex(string index, int cpu = 1, int memory = 1536)
        {
            string pathConfig = PathFolderLDPlayer + $"\\vms\\config\\leidian{index}.config";
            if (File.Exists(pathConfig))
            {
                string srcConfig = Path.Combine(Environment.CurrentDirectory, "LDPlayer\\leidian.txt");


                var doc = File.ReadAllLines(srcConfig);
                File.WriteAllBytes(pathConfig, new byte[0]);
                File.WriteAllLines(pathConfig, doc);
                //File.Copy(srcConfig, pathConfig);
                Helpers.SetValueInJsonFile(pathConfig, "statusSettings.playerName", $"Android {index}");
                Helpers.SetValueInJsonFile(pathConfig, "basicSettings.rootMode", true);
                Helpers.SetValueInJsonFile(pathConfig, "basicSettings.adbDebug", 1);
                Helpers.SetValueInJsonFile(pathConfig, "advancedSettings.cpuCount", cpu);
                Helpers.SetValueInJsonFile(pathConfig, "advancedSettings.memorySize", memory);
                string folderPath = Path.Combine(Environment.CurrentDirectory, $"LDPlayer\\{index}");
                if (!Directory.Exists(folderPath))
                {
                    // Thư mục không tồn tại, tạo mới thư mục
                    Directory.CreateDirectory(folderPath);
                }
                Helpers.SetValueInJsonFile(pathConfig, "statusSettings.sharedPictures", folderPath);
            }
        }
        public static void CopyFile(string sourceFilePath, string destinationFolderPath)
        {
            // Kiểm tra tệp tin nguồn có tồn tại không
            if (!File.Exists(sourceFilePath))
            {
                return;
            }
            // Kiểm tra thư mục đích có tồn tại không
            if (!Directory.Exists(destinationFolderPath))
            {
                return;
            }
            // Lấy tên tệp tin
            string fileName = Path.GetFileName(sourceFilePath);
            // Tạo đường dẫn đến tệp tin đích
            string destinationFilePath = Path.Combine(destinationFolderPath, fileName);
            // Sao chép tệp tin
            File.Copy(sourceFilePath, destinationFilePath, true);
        }
        public static string RandomStringWithExtraChars(List<string> stringList)
        {
            Random random = new Random();
            var numExtraChars = random.Next(10);
            string selectedString = stringList[random.Next(stringList.Count)];
            string extraChars = "";
            for (int i = 0; i < numExtraChars; i++)
            {
                char randomChar = (char)random.Next('a', 'z' + 1);
                extraChars += randomChar;
            }

            return selectedString + extraChars;
        }
        public static bool IsInstallApp(string index, string packageApp)
        {
            try
            {
                var strings = LDController.ADB("index", index, "shell pm list package").Replace("package:", "");
                var result = strings.Contains(packageApp);
                if (result)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
            return false;
        }
    }
}
