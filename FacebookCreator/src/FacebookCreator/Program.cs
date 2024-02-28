using FacebookCreator.Entities;
using FacebookCreator.FacebookConsoller;
using FacebookCreator.Forms;
using FacebookCreator.Helper;
using FacebookCreator.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace FacebookCreator
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        private static readonly IHost _host = CreateHostBuilder();
        private static string dateFile = Path.Combine(Environment.CurrentDirectory, "Data\\Database\\DataAccount.mdf");
        private static string DotnetSDKInstallerPath = Path.Combine(Environment.CurrentDirectory, "Environment\\dotnet.exe"); // Đường dẫn đến tệp tin dotnet-sdk-7.0.305-win-x86.exe
        private static string SqlLocalDBInstallerPath = Path.Combine(Environment.CurrentDirectory, "Environment\\SqlLocalDB.msi"); // Đường dẫn đến tệp tin SqlLocalDB.msi
        private static string VCRedistInstallerPath = Path.Combine(Environment.CurrentDirectory, "Environment\\vcredist.exe"); // Đường dẫn đến tệp tin vc_redist.x64.exe
        [STAThread]
        static void Main()
        {
            Log.Logger = new LoggerConfiguration()
                 .MinimumLevel.Debug()
                 .WriteTo.File("LOGSAPP/myapp.txt", rollingInterval: RollingInterval.Day)
                 .CreateLogger();
            if (!InstallerHelper.IsDotnetSDKInstalled())
            {
                InstallerHelper.InstallDotnetSDK(DotnetSDKInstallerPath);
                Log.Information("Installer SDK");
                Thread.Sleep(5000);
            }
            if (!InstallerHelper.IsVCRedistInstalled())
            {
                bool result = true;
                while (result)
                {
                    Thread.Sleep(2000);
                    if (InstallerHelper.IsDotnetSDKInstalled())
                    {
                        result = false; break;
                    }
                }
                InstallerHelper.InstallVCRedist(VCRedistInstallerPath);
                Log.Information("Installer VC");
                Thread.Sleep(5000);
            }
            if (!InstallerHelper.IsSqlLocalDBInstalled())
            {
                bool result = true;
                while (result)
                {
                    Thread.Sleep(2000);
                    if (InstallerHelper.IsDotnetSDKInstalled() && InstallerHelper.IsVCRedistInstalled())
                    {
                        result = false; break;
                    }
                }
                InstallerHelper.InstallSqlLocalDB(SqlLocalDBInstallerPath);
                Log.Information("Installer SQl");
                Thread.Sleep(5000);
            }
            string folder = Path.Combine(Environment.CurrentDirectory, "Data\\DataImport\\User");
            string folderSession = Path.Combine(Environment.CurrentDirectory, "Data\\DataImport\\Admin\\Session");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            if (!Directory.Exists(folderSession))
            {
                Directory.CreateDirectory(folderSession);
            }
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            try
            {
                _host.Start();
                //Đoạn này mặc định của winform kệ nó thôi.
                Application.SetHighDpiMode(HighDpiMode.SystemAware);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //Lấy ra cái Form1 đã được khai báo trong services
                try
                {
                    bool result = true;
                    while (result)
                    {
                        Thread.Sleep(2000);
                        if (InstallerHelper.IsSqlLocalDBInstalled() && InstallerHelper.IsDotnetSDKInstalled() && InstallerHelper.IsVCRedistInstalled())
                        {
                            result = false; break;
                        }
                    }
                    var form1 = _host.Services.GetRequiredService<fLogin>();
                    //Lệnh chạy gốc là: Application.Run(new Form1);
                    //Đã được thay thế bằng lệnh sử dụng service khai báo trong host
                    Application.Run(form1);
                    Log.Information("Application start");
                }
                catch (Exception ex)
                {

                    Log.Error(ex.Message);
                    if (ex.InnerException != null)
                    {
                        Log.Error(ex.ToString());
                        Log.Error(ex.InnerException.Message);
                    }
                }

                //Khi form chính (form1) bị đóng <==> chương trình kết thúc ấy
                //thì dừng host
                _host.StopAsync().GetAwaiter().GetResult();
                //và giải phóng tài nguyên host đã sử dụng.
                _host.Dispose();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                if (ex.InnerException != null)
                {
                    Log.Error(ex.ToString());
                    Log.Error(ex.InnerException.Message);
                }
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        static IHost CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<fLogin>();
                    services.AddTransient<IAccountRepository, AccountRepository>();
                    services.AddTransient<IFacebookController, FacebookCreator.FacebookConsoller.FacebookController>();
                    services.AddSingleton<fMain>();
                    services.AddDbContext<ApplicationDbContext>(s => s.UseSqlServer($"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={dateFile};Integrated Security=True;Connect Timeout=30"));
                }).Build();
        }
    }
}