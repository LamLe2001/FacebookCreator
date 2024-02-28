using AutoUpdaterDotNET;
using FacebookCreator.ApiQnibot;
using FacebookCreator.Helper;
using LDPlayerAndADBController;
using System.Diagnostics;
using System.Reflection;

namespace FacebookCreator.Forms
{
    public partial class fLogin : Form
    {
        private readonly fMain _fMain;
        private JsonHelper jsonHelper;
        public fLogin(fMain fMain)
        {
            InitializeComponent();
            string path = Path.Combine(Environment.CurrentDirectory, "settings\\Applicetion.json");
            jsonHelper = new JsonHelper(path, isJsonString: false);

            CommonMethods.WireUpMouseEvents(bunifuCustomLabel1, btnClose);
            _fMain = fMain;
            txtKey.Text = jsonHelper.GetValuesFromInputString("Key");
            txtLDPlayer.Text = jsonHelper.GetValuesFromInputString("LDPlayer");
            btnLogin.Text = "Login";
            lbVersion.Text = $"vs {jsonHelper.GetValuesFromInputString("Version")}";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (MessageCommon.ShowConfirmationBox("Are you sure you want to close the software?") == DialogResult.Yes)
            {
                Environment.Exit(0);
            }
        }
        string updateUrl;
        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (btnLogin.Text == "Login")
            {
                try
                {
                    if (string.IsNullOrEmpty(txtKey.Text.Trim()) || string.IsNullOrEmpty(txtLDPlayer.Text.Trim()))
                    {
                        MessageCommon.ShowMessageBox("Please import the required information above", 4);
                        return;
                    }
                    if (!File.Exists($"{txtLDPlayer.Text.Trim()}\\adb.exe"))
                    {
                        MessageCommon.ShowMessageBox("Please check the LDPlayer directory again", 4);
                        return;
                    }
                    //HttpHelper httpHelper = new HttpHelper();
                    //string hardwareId = httpHelper.GetHardwareId();
                    //Constant.licenseKey = txtKey.Text.Trim();
                    //var softwareId = Constant.SoftwareId;
                    //var checkLicenseResult = await httpHelper.CheckLicense(Constant.licenseKey, hardwareId, softwareId);
                    //if (checkLicenseResult.Data is true)
                    //{
                    try
                    {
                        LDController.PathFolderLDPlayer = txtLDPlayer.Text.Trim();
                        SettingsTool.GetSettings("Applicetion").AddValue("Key", txtKey.Text.Trim());
                        SettingsTool.GetSettings("Applicetion").AddValue("LDPlayer", txtLDPlayer.Text.Trim());
                        SettingsTool.UpdateSetting("Applicetion");
                        _fMain.Show();
                        this.Hide();
                    }
                    catch (Exception ex)
                    {
                        MessageCommon.ShowMessageBox(ex.Message, 4);
                        return;
                    }
                    //}
                    //else
                    //{
                    //    try
                    //    {

                    //        MessageCommon.ShowMessageBox(checkLicenseResult.Message, 3);
                    //        var ps = new ProcessStartInfo("https://qnibot.com/")
                    //        {
                    //            UseShellExecute = true,
                    //            Verb = "open"
                    //        };
                    //        Process.Start(ps);
                    //        return;

                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        MessageCommon.ShowMessageBox(ex.Message, 4);
                    //        return;
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    MessageCommon.ShowMessageBox($"Error: {ex.Message}", 4);
                }
            }
            else
            {
                try
                {
                    AutoUpdater.Start(updateUrl);
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                    string version = fvi.FileVersion;
                    lbVersion.Text = $"vs {version}";
                    return;
                }
                catch (Exception ex)
                {
                    MessageCommon.ShowMessageBox(ex.Message, 4);
                    return;
                }
            }
        }
        private async void fLogin_Load(object sender, EventArgs e)
        {
            try
            {
                string version = jsonHelper.GetValuesFromInputString("Version");
                lbVersion.Text = $"vs {version}";
                HttpHelper httpHelper = new HttpHelper();
                var softwareId = Constant.SoftwareId;
                var checkLicenseResult = await httpHelper.CheckVersion(softwareId, version);
                if (checkLicenseResult.Data is false)
                {
                    btnLogin.Text = "Update";
                    updateUrl = checkLicenseResult.Message;
                    AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
                    AutoUpdater.DownloadPath = "update";
                    lbVersion.Text = $"vs {jsonHelper.GetValuesFromInputString("Version")}";
                }
            }
            catch (Exception ex)
            {
                MessageCommon.ShowMessageBox($"Error: {ex.Message}", 4);
            }

        }
        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            try
            {
                if (AutoUpdater.DownloadUpdate(args))
                {
                    btnLogin.Text = "Login";
                    lbVersion.Text = $"vs {jsonHelper.GetValuesFromInputString("Version")}";
                    Application.Exit();

                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
        }
    }
}
