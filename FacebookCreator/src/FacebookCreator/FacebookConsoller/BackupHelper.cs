using LDPlayerAndADBController;
using Serilog;
using System;
using System.Diagnostics;

namespace FacebookController.Helper
{
    public class BackupHelper
    {
        public static async Task BackupAccountAsync(string index, string packageApp, string username)
        {
            CompressedFileZip(index, packageApp);
            await LDController.DelayAsync(7);
            string copyBackup = $"shell su -c 'cp /data/data/{packageApp}/backup.tar.gz /sdcard/backup.tar.gz'";
            LDController.ADB("index", index, copyBackup);
            await LDController.DelayAsync(5);
            string fromFile = Path.Combine(Environment.CurrentDirectory, $"BackupData\\{username}");
            if (!Directory.Exists(fromFile))
            {
                Directory.CreateDirectory(fromFile);
            }
            string IdTemp = (int.Parse(index) * 2 + 5555).ToString();
            await LDController.DelayAsync(2);
            string DeviceIdTemp = "127.0.0.1:" + IdTemp;
            LDController.ExecuteCMD_Result("connect " + DeviceIdTemp);
            string pull = $" -s {DeviceIdTemp} pull /sdcard/backup.tar.gz {fromFile}\\backup.tar.gz";
            RunCommand(pull);
            await LDController.DelayAsync(1);
            await ClearCashFacebookAsync(index);
        }
        public static async Task BackupDeviceAsync(string index, string username)
        {
            string fromFile = Path.Combine(Environment.CurrentDirectory, $"BackupData\\{username}");
            string pathConfig = LDController.PathFolderLDPlayer + $"\\vms\\config\\leidian{index}.config";
            if (!Directory.Exists(fromFile))
            {
                Directory.CreateDirectory(fromFile);
            }
            string srcConfig = $"{fromFile}\\leidian.txt";
            var doc = File.ReadAllLines(pathConfig);
            File.WriteAllBytes(srcConfig, new byte[0]);
            File.WriteAllLines(srcConfig, doc);
        }
        public static async Task ClearCashFacebookAsync(string index)
        {
            string cmd = "adb --index {0} --command shell su -c rm -rf {1}";
            LDController.ADB("index", index, "shell su -c rm -rf /data/data/com.facebook.katana/backup.tar.gz");
            await LDController.DelayAsync(1);
            LDController.ADB("index", index, "shell su -c rm -rf /sdcard/backup.tar.gz");
            await LDController.DelayAsync(1);
            LDController.ADB("index", index, "shell su -c rm -rf /data/data/com.facebook.katana/databases");
            await LDController.DelayAsync(1);
            LDController.ADB("index", index, "shell su -c rm -rf /data/data/com.facebook.katana/app_light_prefs");
            await LDController.DelayAsync(1);
            LDController.ADB("index", index, "shell su -c rm -rf /data/data/com.facebook.katana/shared_prefs");
            await LDController.DelayAsync(1);
            LDController.ADB("index", index, "shell su -c rm -rf /data/data/com.facebook.katana/files/mobileconfig");
            await LDController.DelayAsync(1);
        }
        public static async void CompressedFileZip(string index, string packageApp)
        {
            string IdTemp = (int.Parse(index) * 2 + 5555).ToString();
            string DeviceIdTemp = "127.0.0.1:" + IdTemp;
            LDController.ExecuteCMD_Result("disconnect " + DeviceIdTemp);
            LDController.ExecuteCMD_Result("connect " + DeviceIdTemp);
            string cmdCompressedFileZip = " -s " + DeviceIdTemp + "  shell su -c 'tar -czvpf /data/data/" + packageApp + "/backup.tar.gz /data/data/" + packageApp + "/databases /data/data/" + packageApp + "/app_light_prefs /data/data/" + packageApp + "/shared_prefs /data/data/" + packageApp + "/files/mobileconfig'";
            RunCommand(cmdCompressedFileZip);

        }
        public static string RunCommand(string command)
        {
            string output = string.Empty;
            // Tạo một ProcessStartInfo để cấu hình lệnh cmd
            ProcessStartInfo processStartInfo = new ProcessStartInfo($"{LDController.PathFolderLDPlayer}\\adb.exe", command);
            Process process = new Process();
            try
            {
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.UseShellExecute = false;
                processStartInfo.CreateNoWindow = true;
                // Tạo một Process để thực thi lệnh cmd
                process.StartInfo = processStartInfo;
                process.Start();

                // Đọc dữ liệu đầu ra từ quá trình cmd
                output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi nếu có
                Log.Error(ex, ex.Message);
            }
            process.Close();
            process.Dispose();
            return output;
        }

    }
}
