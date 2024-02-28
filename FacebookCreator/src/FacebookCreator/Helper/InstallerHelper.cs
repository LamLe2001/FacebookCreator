using System.Diagnostics;

namespace FacebookCreator.Helper
{
    public class InstallerHelper
    {

        public static bool IsDotnetSDKInstalled()
        {
            Process process = new Process();
            process.StartInfo.FileName = "dotnet";
            process.StartInfo.Arguments = "--version";
            //process.StartInfo.Arguments = "--list - sdks"; 
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            if (!string.IsNullOrEmpty(output))
            {
                return true;
            }
            return false;
        }

        public static void InstallDotnetSDK(string DotnetSDKInstallerPath)
        {
            string foler = Path.Combine(Environment.CurrentDirectory, "Environment");
            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.WorkingDirectory = foler;
            processStartInfo.FileName = "cmd.exe";
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.Verb = "runas";
            process.StartInfo = processStartInfo;
            process.Start();
            process.StandardInput.WriteLine(DotnetSDKInstallerPath);
            process.StandardInput.Flush();
            process.StandardInput.Close();
            process.WaitForExit();
        }
        public static bool IsSqlLocalDBInstalled()
        {
            bool result = false;
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "sqllocaldb";
                process.StartInfo.Arguments = "v";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                if (output.Contains("Microsoft SQL Server 2019"))
                {
                    result = true;
                }
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        public static void InstallSqlLocalDB(string SqlLocalDBInstallerPath)
        {
            string foler = Path.Combine(Environment.CurrentDirectory, "Environment");
            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.WorkingDirectory = foler;
            processStartInfo.FileName = "cmd.exe";
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.Verb = "runas";
            process.StartInfo = processStartInfo;
            process.Start();
            process.StandardInput.WriteLine(SqlLocalDBInstallerPath);
            process.StandardInput.Flush();
            process.StandardInput.Close();
            process.WaitForExit();
        }
        public static bool IsVCRedistInstalled()
        {
            Process process = new Process();
            process.StartInfo.FileName = "reg";
            process.StartInfo.Arguments = "query \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\14.0\\VC\\Runtimes\\x64\" /reg:32";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return output.Contains("Version");
        }

        public static void InstallVCRedist(string VCRedistInstallerPath)
        {
            string foler = Path.Combine(Environment.CurrentDirectory, "Environment");
            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.WorkingDirectory = foler;
            processStartInfo.FileName = "cmd.exe";
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.RedirectStandardInput = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.Verb = "runas";
            process.StartInfo = processStartInfo;
            process.Start();
            process.StandardInput.WriteLine(VCRedistInstallerPath);
            process.StandardInput.Flush();
            process.StandardInput.Close();
            process.WaitForExit();
        }
    }
}
