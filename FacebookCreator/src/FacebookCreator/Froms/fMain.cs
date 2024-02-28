using AdvancedSharpAdbClient;
using FacebookController.Helper;
using FacebookCreator.Entities;
using FacebookCreator.FacebookConsoller;
using FacebookCreator.FiveSimApi;
using FacebookCreator.Forms;
using FacebookCreator.Froms;
using FacebookCreator.GetSmsCode;
using FacebookCreator.Helper;
using FacebookCreator.Models;
using FacebookCreator.MuiltiTask;
using FacebookCreator.OtpServices;
using FacebookCreator.OtpServices.CountryHelper;
using FacebookCreator.Repositories;
using FacebookCreator.Sms365Api;
using FacebookCreator.ViotpApi;
using ICSharpCode.SharpZipLib.Zip;
using LDPlayerAndADBController;
using LDPlayerAndADBController.ADBClient;
using Serilog;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Web.Services.Description;

namespace FacebookCreator
{
    public partial class fMain : Form
    {
        private readonly IFacebookController _facebookController;
        private readonly IAccountRepository _accountRepository;
        TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
        private readonly BaseViewModel _baseViewModel;
        private int columnCount = 0;
        private int rowCount = 0;
        private int originalFormWidth;
        private List<string> _listProxy = new List<string>();
        private List<string> _listFirtname = new List<string>();
        private List<string> _listLastname = new List<string>();
        private List<string> _listUsername = new List<string>();
        private List<string> _listAvatar = new List<string>();
        private readonly int _accountCreate = 0;
        private object _lockObject;
        private List<ProfileModel> profileModels;
        private JsonHelper jsonHelper;
        private Random random;
        private CancellationTokenSource cancellationTokenSource;
        private string _OtpCountry = string.Empty;
        private List<CountryCode> listCountryCode;
        private List<ComboBoxItem> listItem;
        private bool ckbCatchAll = false;
        private List<string> _listEmailRecovery = new List<string>();
        private List<string> _listEmail = new List<string>();
        public fMain(IAccountRepository accountRepository, IFacebookController facebookController)
        {
            string path = Path.Combine(Environment.CurrentDirectory, "settings\\configGeneral.json");
            jsonHelper = new JsonHelper(path, isJsonString: false);
            InitializeComponent();
            _accountRepository = accountRepository;
            InitializeSavedValues();
            originalFormWidth = this.Width;
            _baseViewModel = new BaseViewModel();
            btnDisplay.Text = "5";
            writeFile();
            profileModels = new List<ProfileModel>();
            _lockObject = new object();
            RichTextBoxHelper._RichTextBox = rtbLogs;
            txtFolderAvatar.Enabled = cbUpdateAvatar.Checked;
            _facebookController = facebookController;
            random = new Random();
        }
        private void writeFile()
        {
            if (File.Exists(GlobalModels.PathProxy))
            {
                var lines = File.ReadAllLines(GlobalModels.PathProxy);
                _listProxy.Clear();
                foreach (var line in lines)
                {
                    _listProxy.Add(line);
                }
            }
            if (File.Exists(GlobalModels.PathFirstName))
            {
                var lines = File.ReadAllLines(GlobalModels.PathFirstName);
                _listFirtname.Clear();
                foreach (var line in lines)
                {
                    _listFirtname.Add(line);
                }
            }
            if (File.Exists(GlobalModels.PathLastName))
            {
                var lines = File.ReadAllLines(GlobalModels.PathLastName);
                _listLastname.Clear();
                foreach (var line in lines)
                {
                    _listLastname.Add(line);
                }
            }
            if (File.Exists(GlobalModels.PathUserName))
            {
                var lines = File.ReadAllLines(GlobalModels.PathUserName);
                _listUsername.Clear();
                foreach (var line in lines)
                {
                    _listUsername.Add(line);
                }
            }
            if (File.Exists(GlobalModels.PathEmail))
            {
                var lines = File.ReadAllLines(GlobalModels.PathEmail);
                _listEmail.Clear();
                foreach (var line in lines)
                {
                    _listEmail.Add(line);
                }
            }
            if (File.Exists(GlobalModels.PathEmailRecovery))
            {
                var lines = File.ReadAllLines(GlobalModels.PathEmailRecovery);
                _listEmailRecovery.Clear();
                foreach (var line in lines)
                {
                    _listEmailRecovery.Add(line);
                }
            }
        }

        private void InitializeSavedValues()
        {
            numberThread.Value = jsonHelper.GetIntType("numberThread");
            rdoNoProxy.Checked = jsonHelper.GetBooleanValue("rdoNoProxy");
            rdoProxy.Checked = jsonHelper.GetBooleanValue("rdoProxy");
            radioCustomizePass.Checked = jsonHelper.GetBooleanValue("radioCustomizePass");
            txtPass.Text = jsonHelper.GetValuesFromInputString("txtPass");
            radioRandomPass.Checked = jsonHelper.GetBooleanValue("radioRandomPass");
            NumberPass.Value = jsonHelper.GetIntType("NumberPass");
            cb2FA.Checked = jsonHelper.GetBooleanValue("cb2FA");
            radioRadomFullNameUs.Checked = jsonHelper.GetBooleanValue("radioRadomFullNameUs");
            radioCustomizeFullName.Checked = jsonHelper.GetBooleanValue("radioCustomizeFullName");
            radioRadomFullNameVN.Checked = jsonHelper.GetBooleanValue("radioRadomFullNameVN");
            cbBoy.Checked = jsonHelper.GetBooleanValue("cbBoy");
            cbWonman.Checked = jsonHelper.GetBooleanValue("cbWonman");
            cbUpdateAvatar.Checked = jsonHelper.GetBooleanValue("cbUpdateAvatar");
            txtFolderAvatar.Text = jsonHelper.GetValuesFromInputString("txtFolderAvatar");
            rbRegWithPhone.Checked = true;
            rbRegWithEmail.Checked = false;
            gbPhone.Visible = true;
            btnAddAccount.Visible = false;
            if (jsonHelper.GetIntType("Action", 0) == 1)
            {
                rbRegWithPhone.Checked = false;
                rbRegWithEmail.Checked = true;
                gbPhone.Visible = false;
                btnAddAccount.Visible = true;
            }
            _OtpCountry = jsonHelper.GetValuesFromInputString("OtpCountry", "+84");
            CBoxOtpService.SelectedIndex = jsonHelper.GetIntType("Service", 0);
            txtApikey.Text = jsonHelper.GetValuesFromInputString("Key");
            rbVietnamese.Checked = false;
            rbLaos.Checked = true;
            if (jsonHelper.GetIntType("CountryViotp", 0) == 1)
            {
                rbVietnamese.Checked = true;
                rbLaos.Checked = false;
            }
            ckbAddEmail.Checked = jsonHelper.GetBooleanValue("ckbAddEmail", false);
            btnSettingEmail.Enabled = ckbAddEmail.Checked;
            ckbCatchAll = SettingsTool.GetSettings("configGeneral").GetBooleanValue("ckbCatchAll");
        }
        private bool CheckProxy(string host, string port, string? username, string? password)
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
        private void saveProperties()
        {
            SettingsTool.GetSettings("configGeneral").AddValue("numberThread", numberThread.Value);
            SettingsTool.GetSettings("configGeneral").AddValue("rdoNoProxy", rdoNoProxy.Checked);
            SettingsTool.GetSettings("configGeneral").AddValue("rdoProxy", rdoProxy.Checked);
            SettingsTool.GetSettings("configGeneral").AddValue("radioCustomizePass", radioCustomizePass.Checked);
            SettingsTool.GetSettings("configGeneral").AddValue("txtPass", txtPass.Text.Trim());
            SettingsTool.GetSettings("configGeneral").AddValue("radioRandomPass", radioRandomPass.Checked);
            SettingsTool.GetSettings("configGeneral").AddValue("NumberPass", NumberPass.Value);
            SettingsTool.GetSettings("configGeneral").AddValue("cb2FA", cb2FA.Checked);
            SettingsTool.GetSettings("configGeneral").AddValue("radioRadomFullNameUs", radioRadomFullNameUs.Checked);
            SettingsTool.GetSettings("configGeneral").AddValue("radioCustomizeFullName", radioCustomizeFullName.Checked);
            SettingsTool.GetSettings("configGeneral").AddValue("radioRadomFullNameVN", radioRadomFullNameVN.Checked);
            SettingsTool.GetSettings("configGeneral").AddValue("cbBoy", cbBoy.Checked);
            SettingsTool.GetSettings("configGeneral").AddValue("cbWonman", cbWonman.Checked);
            SettingsTool.GetSettings("configGeneral").AddValue("cbUpdateAvatar", cbUpdateAvatar.Checked);
            SettingsTool.GetSettings("configGeneral").AddValue("txtFolderAvatar", txtFolderAvatar.Text.Trim());
            int num = 0;
            if (rbRegWithEmail.Checked)
            {
                num = 1;
            }
            SettingsTool.GetSettings("configGeneral").AddValue("Action", num);
            SettingsTool.GetSettings("configGeneral").AddValue("OtpCountry", _OtpCountry);
            SettingsTool.GetSettings("configGeneral").AddValue("Service", CBoxOtpService.SelectedIndex);
            SettingsTool.GetSettings("configGeneral").AddValue("Key", txtApikey.Text.Trim());
            int ctountryViotp = 0;
            if (rbVietnamese.Checked)
            {
                ctountryViotp = 1;
            }
            SettingsTool.GetSettings("configGeneral").AddValue("CountryViotp", ctountryViotp);
            SettingsTool.GetSettings("configGeneral").AddValue("ckbAddEmail", ckbAddEmail.Checked);
            SettingsTool.GetSettings("configGeneral").AddValue("numberAccount", numberAccount.Value);
            SettingsTool.UpdateSetting("configGeneral");
        }
        private async void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                LDController.EditFileConfigLDPlayer();
                btnStart.Enabled = false;
                if (!AdbServer.Instance.GetStatus().IsRunning)
                {
                    AdbServer server = new AdbServer();
                    StartServerResult result = server.StartServer($"{LDController.PathFolderLDPlayer}\\adb.exe", false);
                    if (result != StartServerResult.Started)
                    {
                        MessageCommon.ShowMessageBox("Can't start adb server", 2);
                        return;
                    }
                }
                tableLayoutPanel.Dock = DockStyle.Fill;
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                tableLayoutPanel.AutoScroll = true;
                var ldplayers = LDController.GetDevices2();
                if (LDController.GetDevices2().Count == 0 || ldplayers.Count < (int)numberThread.Value)
                {
                    MessageCommon.ShowMessageBox("Please check LDPlayer!", 2);
                    btnStart.Enabled = true;
                    return;
                }
                saveProperties();
                Random random = new Random();
                if (rdoNoProxy.Checked)
                {
                    _listProxy.Clear();
                }
                if (radioRadomFullNameVN.Checked)
                {
                    var firtsnames = FileHelper.ReadFile(GlobalModels.PathDataFirstNameVN);
                    var lastnames = FileHelper.ReadFile(GlobalModels.PathDataLastNameVN);
                    if (firtsnames.Count > 0 && lastnames.Count > 0)
                    {
                        _listFirtname.Clear();
                        _listLastname.Clear();
                        foreach (var item in firtsnames)
                        {
                            _listFirtname.Add(item);
                        }
                        foreach (var item in lastnames)
                        {
                            _listLastname.Add(item);
                        }
                    }

                }
                else if (radioRadomFullNameUs.Checked == true)
                {
                    var firtsnames = FileHelper.ReadFile(GlobalModels.PathDataFirstNameUS);
                    var lastnames = FileHelper.ReadFile(GlobalModels.PathDataLastNameUs);
                    if (firtsnames.Count > 0 && lastnames.Count > 0)
                    {
                        _listFirtname.Clear();
                        _listLastname.Clear();
                        foreach (var item in firtsnames)
                        {
                            _listFirtname.Add(item);
                        }
                        foreach (var item in lastnames)
                        {
                            _listLastname.Add(item);
                        }
                    }
                }
                //-----------------------------------------------//
                List<string> list = new List<string>();
                Queue<string> queueProxy = new Queue<string>(); ;
                if (rdoProxy.Checked)
                {
                    list.AddRange(_listProxy);
                    while (numberAccount.Value > list.Count)
                    {
                        list.AddRange(_listProxy);
                    }
                    queueProxy = new Queue<string>(list);
                }
                Queue<string> queueAvatar = new Queue<string>();
                if (cbUpdateAvatar.Checked && Directory.Exists(txtFolderAvatar.Text.Trim()))
                {
                    _listAvatar.Clear();
                    var files = FileHelper.ReadImageFiles(txtFolderAvatar.Text.Trim());
                    if (files.Count == 0)
                    {
                        return;
                    }
                    _listAvatar.AddRange(files);
                    while (numberAccount.Value > _listAvatar.Count)
                    {
                        _listAvatar.AddRange(files);
                    }
                    queueAvatar = new Queue<string>(_listAvatar);
                }
                profileModels.Clear();
                Queue<string> queueEmail = new Queue<string>();
                if (rbRegWithEmail.Checked)
                {
                    queueEmail = new Queue<string>(_listEmail);
                }
                Queue<string> queueEmailRecovery = new Queue<string>();

                if (rbRegWithPhone.Checked && ckbAddEmail.Checked)
                {
                    var files = _listEmailRecovery;
                    while (numberAccount.Value > files.Count)
                    {
                        files.AddRange(_listEmailRecovery);
                    }
                    queueEmailRecovery = new Queue<string>(files);
                }
                for (int i = 0; i < numberAccount.Value; i++)
                {
                    ProfileModel profile = new ProfileModel();
                    profile.Backup = Path.Combine(Environment.CurrentDirectory, "Data\\DataImport\\Admin\\Backup");
                    profile.Firstname = _listFirtname[random.Next(0, _listFirtname.Count)];
                    profile.Lastname = _listLastname[random.Next(0, _listLastname.Count)];
                    if (radioCustomizePass.Checked)
                    {
                        profile.Password = txtPass.Text.Trim();
                    }
                    else
                    {
                        profile.Password = Helpers.GenerateRandomString("abcdefghijklmnopqrstuvwxyz0123456789", (int)NumberPass.Value);
                    }
                    if (_listProxy.Count > 0 && rdoProxy.Checked)
                    {
                        string text = queueProxy.Dequeue().Replace("'", "''");
                        profile.Proxy = text;
                    }
                    if (_listAvatar.Count > 0 && cbUpdateAvatar.Checked)
                    {
                        string text = queueAvatar.Dequeue().Replace("'", "''");
                        profile.FileImage = text;
                    }
                    profile.IsUsing = false;
                    if (_listEmail.Count > 0 && rbRegWithEmail.Checked)
                    {
                        string text = queueEmail.Dequeue().Replace("'", "''");
                        var account = text.Split('|');
                        if (account != null && account.Length > 3)
                        {
                            profile.Email = account[0];
                            profile.PasswordEmail = account[1];
                            profile.Port = account[2];
                            profile.Server = account[3];
                        }
                    }
                    if (rbRegWithPhone.Checked && ckbAddEmail.Checked)
                    {
                        string text = queueEmailRecovery.Dequeue().Replace("'", "''");
                        var account = text.Split('|');
                        if (account != null && account.Length > 3)
                        {
                            if (ckbCatchAll)
                            {
                                profile.Domain = account[0];
                            }
                            else
                            {
                                profile.Email = account[0];
                            }
                            profile.PasswordEmail = account[1];
                            profile.Port = account[3];
                            profile.Server = account[2];
                        }
                    }
                    profileModels.Add(profile);
                }
                if (profileModels.Count <= 0)
                {
                    MessageCommon.ShowMessageBox("Please select an Email that has not been registered for account creation.", 3);
                    btnStart.Enabled = true;
                    return;
                }
                numberAccount.Value = profileModels.Count;
                foreach (var item in ldplayers)
                {
                    if (!LDController.IsDevice_Running("index", item.index.ToString()))
                    {
                        DeviceInfo device = new DeviceInfo();
                        device.IndexLDPlayer = item.index.ToString();
                        device.AdbClient = new AdbClient();
                        device.Data = new DeviceData();
                        device.View = new ViewInfo();
                        device.View.Embeddedpanel = new Panel();
                        device.View.StatusLabel = new Label();
                        device.View.LdplayerHandle = new IntPtr();
                        device.View.Panel = new Panel();
                        device.View.PanelButton = new Panel();
                        device.View.BtnClose = new Button();
                        device.sourceToken = new CancellationTokenSource();
                        device.pauseToken = new PauseTokenSource();
                        device.DataGridView = new DataGridView();
                        device.DataGridView = dtgvAccount;
                        device.RowDataGridView = new DataGridViewRow();
                        GlobalModels.Devices.Add(device);
                    }
                    if (GlobalModels.Devices.Count >= (int)numberThread.Value)
                    {
                        break;
                    }
                }
                if (GlobalModels.Devices.Count <= 0 || GlobalModels.Devices.Count >= numberAccount.Value)
                {
                    MessageCommon.ShowMessageBox("Please check LDPlayer.", 3);
                    btnStart.Enabled = true;
                    return;
                }
                List<Task> tasks = new List<Task>();
                cancellationTokenSource = new CancellationTokenSource();
                foreach (var item in GlobalModels.Devices)
                {
                    lock (_lockObject)
                    {
                        File.Delete($"{LDController.PathFolderLDPlayer}\\vms\\leidian{item.IndexLDPlayer}\\data.vmdk");
                        System.IO.Compression.ZipFile.ExtractToDirectory(Path.Combine(Environment.CurrentDirectory, "LDplayer\\data.zip"), $"{LDController.PathFolderLDPlayer}\\vms\\leidian{item.IndexLDPlayer}");
                        File.Delete($"{LDController.PathFolderLDPlayer}\\vms\\config\\leidian{item.IndexLDPlayer}.config");
                        string newPath = Path.Combine($"{LDController.PathFolderLDPlayer}\\vms\\config", $"leidian{item.IndexLDPlayer}.config");
                        File.Copy(Path.Combine(Environment.CurrentDirectory, "LDplayer\\leidian.txt"), newPath);
                    }
                    int i = 6;
                    while (i > 0)
                    {
                        LDController.Open("index", item.IndexLDPlayer);
                        await LDController.DelayAsync(5, 10);
                        if (LDController.IsDevice_Running("index", item.IndexLDPlayer))
                        {
                            if (columnCount >= int.Parse(btnDisplay.Text.Trim()))
                            {
                                rowCount++;
                                columnCount = 0;
                                tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                            }
                            item.View.originalParentHandle = IntPtr.Zero;
                            addViewControl(item, tableLayoutPanel);
                            columnCount++;
                            await LDController.DelayAsync(8, 15);
                            pDevice.Controls.Add(tableLayoutPanel);

                            tasks.Add(Task.Run(async () =>
                            {
                                await StartDevice(item);
                            }, cancellationTokenSource.Token));
                            break;
                        }
                        i--;
                    }
                }
                await Task.WhenAll(tasks);
                btnStart.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageCommon.ShowMessageBox(ex.Message, 4);
                Log.Error(ex, ex.Message);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 2);
            }
        }
        private async Task StartDevice(DeviceInfo device)
        {
            try
            {
                while (true)
                {
                    ProfileModel selectAccount;
                    writeLog(device, "Online", null, device.View.StatusLabel, 2);
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    while (true)
                    {
                        await LDController.DelayAsync().ConfigureAwait(false);
                        if (DeviceHelper.Connect(device, 10))
                        {
                            DataGridViewHelper.SetCellValueByColumnValue(dtgvLDPlayers, device.IndexLDPlayer, "tIndex", "tDeviceId", device.Id, 2);
                            DataGridViewHelper.SetCellValueByColumnValue(dtgvLDPlayers, device.IndexLDPlayer, "tIndex", "tName", device.Data.Model, 2);
                            ADBHelper.TurnOnADBKeyboard(device.Id);
                            writeLog(device, "Connect Success", null, device.View.StatusLabel, 2);
                            device.Status = "Connect";
                            sw.Stop();
                            break;
                        }
                        if (sw.ElapsedMilliseconds > 120000)
                        {
                            device.Status = "reboot";
                            sw.Stop(); break;
                        }
                    }
                    if (device.Status == "Connect")
                    {
                        lock (_lockObject)
                        {
                            selectAccount = profileModels.FirstOrDefault(x => x.IsUsing == false);
                        }
                        if (selectAccount == null)
                        {
                            LDController.Close("index", device.IndexLDPlayer);
                            tableLayoutPanel.Invoke((MethodInvoker)delegate
                            {
                                tableLayoutPanel.Controls.Remove(device.View.Panel);
                            });
                            RepositionLDPlayers();
                            return;
                        }
                        selectAccount.IsUsing = true;
                        if (!rdoNoProxy.Checked && !string.IsNullOrEmpty(selectAccount.Proxy))
                        {
                            writeLog(device, "Connect Proxy", null, device.View.StatusLabel);
                            string ip = string.Empty, port = string.Empty, username = string.Empty, password = string.Empty;
                            string[] proxy = selectAccount.Proxy.Split(':');
                            if (proxy.Length > 0)
                            {
                                ip = proxy[0];
                                port = proxy[1];
                                if (proxy.Length > 3)
                                {
                                    username = proxy[2];
                                    password = proxy[3];
                                }
                                if (!string.IsNullOrEmpty(ip) && !string.IsNullOrEmpty(port))
                                {
                                    for (int i = 0; i < 10; i++)
                                    {
                                        if (CheckProxy(ip, port, username, password))
                                        {
                                            writeLog(device, $"{selectAccount.Proxy}", null, device.View.StatusLabel);
                                            await ChangeProxy(device.IndexLDPlayer, device.Id, ip, port, username, password);
                                            await LDController.DelayAsync(10);
                                            i = 10;
                                            break;
                                        }
                                        else
                                        {
                                            writeLog(device, $"Proxy Fail: {selectAccount.Proxy}", null, device.View.StatusLabel, 1);
                                            selectAccount.Proxy = _listProxy[random.Next(0, _listProxy.Count)];
                                            device.Status = "reboot";
                                        }
                                    }
                                }
                            }
                        }
                        //////////////////////////////////////////Reg Facebook///////////////////////////////////////
                        if (device.Status != "reboot")
                        {
                            writeLog(device, "Facebook", null, device.View.StatusLabel);
                            var checkLayoutFB = await _facebookController.CheckLayoutFacebookAsync(device.IndexLDPlayer, device.Data, device.AdbClient);
                            if (checkLayoutFB == 1)
                            {
                                if (await _facebookController.ClickCreaNewAccountFacebookAsync(device.Data, device.AdbClient) == "What's your name?")
                                {
                                    writeLog(device, "Import Fullname", null, device.View.StatusLabel);
                                    if (await _facebookController.ImportFullnameFacebookAsync(device.IndexLDPlayer, device.Data, device.AdbClient, selectAccount.Firstname, selectAccount.Lastname) == "Set date")
                                    {
                                        writeLog(device, $"{selectAccount.Firstname} {selectAccount.Lastname}", null, device.View.StatusLabel);
                                        if (await _facebookController.SelecBirthDayFacebookAsync(device.IndexLDPlayer, device.Data, device.AdbClient) == "What's your gender?")
                                        {
                                            writeLog(device, $"Select Gender", null, device.View.StatusLabel);
                                            if (await _facebookController.SelecGenderFacebookAsync(device.Data, device.AdbClient, selectAccount.Gender) == "What's your mobile number?")
                                            {
                                                if (selectAccount.Gender == 0)
                                                {
                                                    writeLog(device, $"Male", null, device.View.StatusLabel);
                                                }
                                                else
                                                {
                                                    writeLog(device, $"Female", null, device.View.StatusLabel);
                                                }
                                                string phonenumber = string.Empty, idPhone = string.Empty, code = string.Empty;
                                                if (rbRegWithEmail.Checked)
                                                {
                                                    string email = Helpers.GenerateRandomString("abcdefghijklmnopqrstuvwxyz", random.Next(1, 5));
                                                    email = email + Helpers.GenerateRandomString("abcdefghijklmnopqrstuvwxyz0123456789", random.Next(8, 20));
                                                    email = email + "@gmail.com";
                                                    writeLog(device, $"Import Email: {email}", null, device.View.StatusLabel);
                                                    if (await _facebookController.ImportEmailFacebookAsync(device.IndexLDPlayer, device.Data, device.AdbClient, email) == "Create a password")
                                                    {
                                                        device.Status = "Create a password";
                                                    }
                                                }
                                                else if (rbRegWithPhone.Checked)
                                                {
                                                    var service = GlobalModels.Service;
                                                    int GetPhone = 10;
                                                    while (GetPhone > 0)
                                                    {
                                                        switch (service)
                                                        {
                                                            case "5sim.net":
                                                                {
                                                                    var httpResult = await FiveSimHttpHelper.BuyPhoneNumber(GlobalModels.Contry, txtApikey.Text.Trim());
                                                                    if (httpResult.StatusCode == 200)
                                                                    {
                                                                        var getBuyPhone = (FiveResult)httpResult.Data;
                                                                        phonenumber = getBuyPhone.phone;
                                                                        idPhone = getBuyPhone.id.ToString();
                                                                        GetPhone = 0;
                                                                        break;
                                                                    }
                                                                    else
                                                                    {
                                                                        var message = httpResult.Data;
                                                                        if (message != null)
                                                                        {
                                                                            writeLog(device, message.ToString(), null, device.View.StatusLabel, 1);
                                                                        }
                                                                        else
                                                                        {
                                                                            writeLog(device, "No Phone", null, device.View.StatusLabel, 1);
                                                                        }
                                                                    }
                                                                    GetPhone--;
                                                                    continue;
                                                                }
                                                            case "Viotp.com":
                                                                {
                                                                    string appId = "36";
                                                                    if (rbLaos.Checked)
                                                                    {
                                                                        appId = "36&country=la";
                                                                    }
                                                                    var getBuyPhone = await ViotpHttpHelper.BuyPhoneNumber(txtApikey.Text.Trim(), appId);
                                                                    if (getBuyPhone.success == true)
                                                                    {
                                                                        idPhone = getBuyPhone.data.request_id.ToString();
                                                                        phonenumber = getBuyPhone.data.countryCode + getBuyPhone.data.phone_number;
                                                                        GetPhone = 0;
                                                                        break;
                                                                    }
                                                                    else
                                                                    {
                                                                        writeLog(device, getBuyPhone.message, null, device.View.StatusLabel, 1);
                                                                    }
                                                                    GetPhone--;
                                                                    continue;
                                                                }
                                                            case "365sms.org":
                                                                {
                                                                    var getBuyPhone = await Sms365HttpHelper.BuyPhoneNumber(txtApikey.Text.Trim(), GlobalModels.Contry);
                                                                    string[] result = getBuyPhone.Split(':');
                                                                    if (result[0] == "ACCESS_NUMBER")
                                                                    {
                                                                        phonenumber = result[2];
                                                                        idPhone = result[1];
                                                                        GetPhone = 0;
                                                                        break;
                                                                    }
                                                                    else
                                                                    {
                                                                        writeLog(device, getBuyPhone, null, device.View.StatusLabel, 1);
                                                                    }
                                                                    GetPhone--;
                                                                    continue;
                                                                }
                                                            case "Getsmscode.io":
                                                                {
                                                                    var getBuyPhone = await GetSmsCodeHttpHelper.BuyPhoneNumber(txtApikey.Text.Trim(), GlobalModels.Contry);
                                                                    if (getBuyPhone == null)
                                                                    {
                                                                        writeLog(device, "Error", null, device.View.StatusLabel, 1);
                                                                    }
                                                                    else if (getBuyPhone.Status)
                                                                    {
                                                                        phonenumber = getBuyPhone.data.Phone_number;
                                                                        idPhone = getBuyPhone.data.ActivationId;
                                                                        GetPhone = 0;
                                                                        break;
                                                                    }
                                                                    else
                                                                    {
                                                                        writeLog(device, getBuyPhone.errors.error_mess, null, device.View.StatusLabel, 1);
                                                                    }
                                                                    GetPhone--;
                                                                    continue;
                                                                }
                                                            default:
                                                                {
                                                                    GetPhone--;
                                                                    continue;
                                                                }
                                                        }
                                                    }
                                                    writeLog(device, $"Import Phone: {phonenumber}", null, device.View.StatusLabel);
                                                    if (await _facebookController.ImportPhonenumberFacebookAsync(device.IndexLDPlayer, device.Data, device.AdbClient, phonenumber) == "Create a password")
                                                    {
                                                        device.Status = "Create a password";
                                                    }
                                                }
                                                if (device.Status == "Create a password")
                                                {
                                                    writeLog(device, $"Import Password {selectAccount.Password}", null, device.View.StatusLabel);
                                                    if (await _facebookController.ImportPasswrodFacebookAsync(device.IndexLDPlayer, device.Data, device.AdbClient, selectAccount.Password) == "Enter the confirmation code")
                                                    {
                                                        if (rbRegWithEmail.Checked)
                                                        {
                                                            writeLog(device, $"Import Email {selectAccount.Email}", null, device.View.StatusLabel);
                                                            if (await _facebookController.FakeEmailFacebookAsync(device.IndexLDPlayer, device.Data, device.AdbClient, selectAccount.Email))
                                                            {
                                                                writeLog(device, $"Get code Email {selectAccount.Email}", null, device.View.StatusLabel);
                                                                code = MailKitsHelper.GetCodeEmail(selectAccount.Email, selectAccount.Password, selectAccount.Server, int.Parse(selectAccount.Port), selectAccount.Email, "Facebook");
                                                            }
                                                        }
                                                        else if (rbRegWithPhone.Checked)
                                                        {
                                                            var service = GlobalModels.Service;
                                                            int GetPhone = 10;
                                                            while (GetPhone > 0)
                                                            {
                                                                writeLog(device, $" Get Code {service}", null, device.View.StatusLabel);
                                                                await LDController.DelayAsync(10, 20);
                                                                switch (service)
                                                                {
                                                                    case "5sim.net":
                                                                        {
                                                                            await LDController.DelayAsync(10, 20);
                                                                            var httpResult = await FiveSimHttpHelper.GetOtp(txtApikey.Text.Trim(), idPhone);
                                                                            if (httpResult.StatusCode == 200)
                                                                            {
                                                                                var getOtp = (FiveResult)httpResult.Data;
                                                                                code = getOtp.sms[0].code;
                                                                                GetPhone = 0;
                                                                                break;
                                                                            }
                                                                            else
                                                                            {
                                                                                var message = httpResult.Data;
                                                                                if (message != null)
                                                                                {
                                                                                    writeLog(device, message.ToString(), null, device.View.StatusLabel, 1);
                                                                                }
                                                                                else
                                                                                {
                                                                                    writeLog(device, "No Code", null, device.View.StatusLabel, 1);
                                                                                }
                                                                                await LDController.DelayAsync(5, 10);
                                                                            }
                                                                            GetPhone--;
                                                                            continue;
                                                                        }
                                                                    case "Viotp.com":
                                                                        {
                                                                            var getOtp = await ViotpHttpHelper.GetOtp(txtApikey.Text.Trim(), idPhone);
                                                                            if (getOtp.success == true)
                                                                            {
                                                                                if (getOtp.data?.Status == 1)
                                                                                {
                                                                                    code = getOtp.data.Code;
                                                                                    GetPhone = 0;
                                                                                    break;
                                                                                }
                                                                                writeLog(device, getOtp.message, null, device.View.StatusLabel, 1);
                                                                            }
                                                                            else
                                                                            {
                                                                                writeLog(device, getOtp.message, null, device.View.StatusLabel, 1);
                                                                            }
                                                                            await LDController.DelayAsync(5, 10);
                                                                            GetPhone--;
                                                                            continue;
                                                                        }
                                                                    case "365sms.org":
                                                                        {
                                                                            await LDController.DelayAsync(10, 20);
                                                                            var getOtp = await Sms365HttpHelper.GetOtp(txtApikey.Text.Trim(), idPhone);
                                                                            string[] result = getOtp.Split(':');
                                                                            if (result[0] == "STATUS_OK")
                                                                            {
                                                                                code = result[1];
                                                                                GetPhone = 0;
                                                                                break;
                                                                            }
                                                                            else
                                                                            {
                                                                                writeLog(device, getOtp, null, device.View.StatusLabel, 1);
                                                                                await LDController.DelayAsync(5, 10);
                                                                            }
                                                                            GetPhone--;
                                                                            continue;
                                                                        }
                                                                    case "Getsmscode.io":
                                                                        {
                                                                            await LDController.DelayAsync(20, 40);
                                                                            var getOtp = await GetSmsCodeHttpHelper.GetOtp(txtApikey.Text.Trim(), idPhone);
                                                                            if (getOtp != null && getOtp.Status)
                                                                            {
                                                                                code = getOtp.sms_code;
                                                                                GetPhone = 0;
                                                                                break;
                                                                            }
                                                                            else
                                                                            {
                                                                                writeLog(device, getOtp.error, null, device.View.StatusLabel, 1);
                                                                                await LDController.DelayAsync(5, 10);
                                                                            }
                                                                            GetPhone--;
                                                                            continue;
                                                                        }
                                                                    default:
                                                                        {
                                                                            continue;
                                                                        }
                                                                }
                                                            }

                                                        }
                                                        if (!string.IsNullOrEmpty(code))
                                                        {
                                                            if (await _facebookController.ImportCodeFacebookAsync(device.IndexLDPlayer, device.Data, device.AdbClient, code))
                                                            {
                                                                if (await _facebookController.CheckLoginSuccesFacebookAsync(device.Data, device.AdbClient) == 1)
                                                                {
                                                                    string username = string.Empty;
                                                                    Account account = new Account();
                                                                    account.Id = new Guid();
                                                                    if (rbRegWithPhone.Checked)
                                                                    {
                                                                        account.Phonenumber = phonenumber;
                                                                        username = phonenumber;
                                                                    }
                                                                    else if (rbRegWithEmail.Checked)
                                                                    {
                                                                        account.Email = selectAccount.Email;
                                                                        account.PasswordEmail = selectAccount.PasswordEmail;
                                                                        account.Server = selectAccount.Server;
                                                                        account.Port = selectAccount.Port;
                                                                        username = selectAccount.Email;
                                                                    }
                                                                    account.FullName = $"{selectAccount.Firstname} {selectAccount.Lastname}";
                                                                    account.Status = "Done";
                                                                    account.CreateDate = DateTime.Now;
                                                                    account.Password = selectAccount.Password;
                                                                    account.Proxy = selectAccount.Proxy;
                                                                    try
                                                                    {
                                                                        _accountRepository.Add(account);
                                                                        loadAccount();
                                                                        writeLog(device, $"Done", account.Id.ToString(), device.View.StatusLabel, 2);

                                                                    }
                                                                    catch (Exception ex)
                                                                    {
                                                                        writeLog(device, $"Fail add Account error: {ex.Message}", null, device.View.StatusLabel, 1);
                                                                        Log.Error(ex, ex.Message);
                                                                    }
                                                                    string userBackup = selectAccount.Email;
                                                                    if (rbRegWithPhone.Checked)
                                                                    {
                                                                        userBackup = phonenumber;
                                                                    }
                                                                    await BackupHelper.BackupAccountAsync(device.IndexLDPlayer, "com.facebook.katana", userBackup);
                                                                    await BackupHelper.BackupDeviceAsync(device.IndexLDPlayer, userBackup);
                                                                    account.Backup = Path.Combine(Environment.CurrentDirectory, $"BackupData\\{userBackup}");
                                                                    _accountRepository.UpdateBackupByIdAccount(account.Id, Path.Combine(Environment.CurrentDirectory, $"BackupData\\{userBackup}"));
                                                                    DataGridViewHelper.SetCellValueByColumnValue(dtgvAccount, account.Id.ToString(), "cIdAccount", "cBackup", Path.Combine(Environment.CurrentDirectory, $"BackupData\\{userBackup}"));
                                                                    writeLog(device, Path.Combine(Environment.CurrentDirectory, $"BackupData\\{userBackup}"), account.Id.ToString(), device.View.StatusLabel, 2);
                                                                    var user = await _facebookController.GetTokenAndUidFacebookAsync(device.Id, selectAccount.Backup);
                                                                    if (!string.IsNullOrEmpty(user) && user.Split('|').Length > 1)
                                                                    {
                                                                        var token = user.Split('|')[0];
                                                                        var uid = user.Split('|')[1];
                                                                        if (!string.IsNullOrEmpty(uid))
                                                                        {
                                                                            _accountRepository.UpdateUidByIdAccount(account.Id, uid);
                                                                            DataGridViewHelper.SetCellValueByColumnValue(dtgvAccount, account.Id.ToString(), "cIdAccount", "cUid", uid, 2);
                                                                            writeLog(device, $"Uid: {uid}", account.Id.ToString(), device.View.StatusLabel, 2);
                                                                        }
                                                                        if (!string.IsNullOrEmpty(token))
                                                                        {
                                                                            _accountRepository.UpdateTokenByIdAccount(account.Id, token);
                                                                            DataGridViewHelper.SetCellValueByColumnValue(dtgvAccount, account.Id.ToString(), "cIdAccount", "cToken", token, 2);
                                                                            writeLog(device, $"Token: {token}", account.Id.ToString(), device.View.StatusLabel, 2);
                                                                        }
                                                                        if (await _facebookController.UploadAvatarToFacebookAsync(token, selectAccount.FileImage, selectAccount.Proxy))
                                                                        {
                                                                            writeLog(device, $"Update Avatar {selectAccount.FileImage}", account.Id.ToString(), device.View.StatusLabel, 2);
                                                                            _accountRepository.UpdateAvatarByIdAccount(account.Id, selectAccount.FileImage);
                                                                            DataGridViewHelper.SetCellValueByColumnValue(dtgvAccount, account.Id.ToString(), "cIdAccount", "cAvatar", selectAccount.FileImage, 2);
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }


                                                {

                                                    {
                                                        writeLog(device, $"Import Password: {selectAccount.Password}", selectAccount.IdAccount, device.View.StatusLabel);
                                                        DataGridViewHelper.SetCellValueByColumnValue(dtgvAccount, selectAccount.IdAccount, "cIdAccount", "cPassword", selectAccount.Password);
                                                        writeLog(device, $"Get code: {selectAccount.Email}", selectAccount.IdAccount, device.View.StatusLabel);
                                                        var codeEmail = MailKitsHelper.GetCodeEmail(selectAccount.Email, selectAccount.PasswordEmail, selectAccount.Server, int.Parse(selectAccount.Port), "Facebook");
                                                        if (codeEmail.ToString().Length == 5)
                                                        {
                                                            writeLog(device, $"Get code: {codeEmail}", selectAccount.IdAccount, device.View.StatusLabel);
                                                            if (await _facebookController.ImportCodeFacebookAsync(device.IndexLDPlayer, device.Data, device.AdbClient, codeEmail.ToString()))
                                                            {
                                                                writeLog(device, $"Check succes", selectAccount.IdAccount, device.View.StatusLabel);
                                                                if (await _facebookController.CheckLoginSuccesFacebookAsync(device.Data, device.AdbClient) == 1)
                                                                {
                                                                    writeLog(device, $"succes", selectAccount.IdAccount, device.View.StatusLabel);
                                                                    await BackupHelper.BackupAccountAsync(device.IndexLDPlayer, "com.facebook.katana", selectAccount.Email);
                                                                    await BackupHelper.BackupDeviceAsync(device.IndexLDPlayer, selectAccount.Email);
                                                                    writeLog(device, $"Backup", selectAccount.IdAccount, device.View.StatusLabel);
                                                                    Account account = new Account();
                                                                    account.Id = new Guid(selectAccount.IdAccount);
                                                                    account.FullName = $"{selectAccount.Firstname} {selectAccount.Lastname}";
                                                                    account.Status = "Done";
                                                                    account.CreateDate = DateTime.Now;
                                                                    account.Password = selectAccount.Password;
                                                                    account.Backup = Path.Combine(Environment.CurrentDirectory, $"BackupData\\{selectAccount.Email}");
                                                                    account.Gender = selectAccount.Gender;
                                                                    account.Proxy = selectAccount.Proxy;
                                                                    _accountRepository.Update(account);
                                                                    writeLog(device, $"Update Acccount", selectAccount.IdAccount, device.View.StatusLabel);
                                                                    DataGridViewHelper.SetCellValueByColumnValue(dtgvAccount, selectAccount.IdAccount, "cIdAccount", "cBackup", account.Backup, 2);
                                                                    DataGridViewHelper.SetCellValueByColumnValue(dtgvAccount, selectAccount.IdAccount, "cIdAccount", "cStatus", "Done", 2);
                                                                    writeLog(device, $"Get token and Uid ", selectAccount.IdAccount, device.View.StatusLabel, 2);

                                                                    if (cbUpdateAvatar.Checked && user != null && !string.IsNullOrEmpty(user.TokenAndroid) && !string.IsNullOrEmpty(user.Uid))
                                                                    {
                                                                        writeLog(device, $"Token: {user.TokenAndroid}  Uid: {user.Uid}", selectAccount.IdAccount, device.View.StatusLabel, 2);
                                                                        _accountRepository.UpdateTokenByIdAccount(account.Id, user.TokenAndroid);
                                                                        DataGridViewHelper.SetCellValueByColumnValue(dtgvAccount, selectAccount.IdAccount, "cIdAccount", "cToken", user.TokenAndroid, 2);
                                                                        _accountRepository.UpdateUidByIdAccount(account.Id, user.Uid);
                                                                        DataGridViewHelper.SetCellValueByColumnValue(dtgvAccount, selectAccount.IdAccount, "cIdAccount", "cUid", user.Uid, 2);
                                                                        if (await _facebookController.UploadAvatarToFacebookAsync(user.TokenAndroid, selectAccount.FileImage, selectAccount.Proxy))
                                                                        {
                                                                            writeLog(device, $"Update Avatar {selectAccount.FileImage}", selectAccount.IdAccount, device.View.StatusLabel, 2);
                                                                            _accountRepository.UpdateAvatarByIdAccount(account.Id, selectAccount.FileImage);
                                                                            DataGridViewHelper.SetCellValueByColumnValue(dtgvAccount, selectAccount.IdAccount, "cIdAccount", "cAvatar", selectAccount.FileImage, 2);
                                                                        }
                                                                    }
                                                                    if (cb2FA.Checked)
                                                                    {
                                                                        writeLog(device, $"turn On 2FA ", selectAccount.IdAccount, device.View.StatusLabel, 2);
                                                                        var tow2FA = await _facebookController.GetPassword2FAFacebookAsync(device.Id, device.IndexLDPlayer, device.Data, device.AdbClient, selectAccount.Password);
                                                                        if (!string.IsNullOrEmpty(tow2FA) && tow2FA.Length == 32)
                                                                        {
                                                                            writeLog(device, $"2FA: {tow2FA} ", selectAccount.IdAccount, device.View.StatusLabel, 2);
                                                                            _accountRepository.UpdateTowFAByIdAccount(new Guid(selectAccount.IdAccount), tow2FA);
                                                                            DataGridViewHelper.SetCellValueByColumnValue(dtgvAccount, selectAccount.IdAccount, "cIdAccount", "c2FA", tow2FA, 2);
                                                                        }
                                                                    }
                                                                    string message = $"{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss\t")}    {account.Email} | {account.PasswordEmail} | {account.Uid} | {account.Password} | {account.Proxy} | {account.Token} | {account.TowFA} | {account.Backup} | {account.FullName} | {account.Avatar}";
                                                                    rtbResult.Invoke((MethodInvoker)delegate
                                                                    {
                                                                        rtbResult.InvokeEx(s => s.AppendText(message, Color.Green, rtbResult.Font, true));
                                                                    });
                                                                }
                                                                else
                                                                {
                                                                    DataGridViewHelper.SetCellValueByColumnValue(dtgvAccount, selectAccount.IdAccount, "cIdAccount", "cStatus", "Fail", 1);
                                                                    _accountRepository.UpdateStatusByIdAccount(new Guid(selectAccount.IdAccount), "Fail");
                                                                }
                                                            }
                                                            else
                                                            {
                                                                DataGridViewHelper.SetCellValueByColumnValue(dtgvAccount, selectAccount.IdAccount, "cIdAccount", "cStatus", "Fail", 1);
                                                                _accountRepository.UpdateStatusByIdAccount(new Guid(selectAccount.IdAccount), "Fail");
                                                            }
                                                        }
                                                        else
                                                        {
                                                            DataGridViewHelper.SetCellValueByColumnValue(dtgvAccount, selectAccount.IdAccount, "cIdAccount", "cStatus", "Fail", 1);
                                                            _accountRepository.UpdateStatusByIdAccount(new Guid(selectAccount.IdAccount), "Fail");
                                                            writeLog(device, $"Fail", selectAccount.IdAccount, device.View.StatusLabel, 1);
                                                        }
                                                    }
                                                    else if (checkPoint == "Ban 180 days")
                                                    {
                                                        selectAccount.Firstname = _listFirtname[random.Next(_listFirtname.Count)];
                                                        selectAccount.Lastname = _listLastname[random.Next(_listLastname.Count)];
                                                        selectAccount.Proxy = _listProxy[random.Next(0, _listProxy.Count)];
                                                        selectAccount.IsUsing = false;
                                                    }
                                                    else
                                                    {
                                                        selectAccount.Firstname = _listFirtname[random.Next(_listFirtname.Count)];
                                                        selectAccount.Lastname = _listLastname[random.Next(_listLastname.Count)];
                                                        selectAccount.Proxy = _listProxy[random.Next(0, _listProxy.Count)];
                                                        selectAccount.IsUsing = false;
                                                    }
                                                }
                                                else
                                                {
                                                    selectAccount.Firstname = _listFirtname[random.Next(_listFirtname.Count)];
                                                    selectAccount.Lastname = _listLastname[random.Next(_listLastname.Count)];
                                                    selectAccount.Proxy = _listProxy[random.Next(0, _listProxy.Count)];
                                                    selectAccount.IsUsing = false;
                                                }
                                            }
                                            else
                                            {
                                                selectAccount.Firstname = _listFirtname[random.Next(_listFirtname.Count)];
                                                selectAccount.Lastname = _listLastname[random.Next(_listLastname.Count)];
                                                selectAccount.Proxy = _listProxy[random.Next(0, _listProxy.Count)];
                                                selectAccount.IsUsing = false;
                                            }
                                        }
                                        else
                                        {
                                            selectAccount.Firstname = _listFirtname[random.Next(_listFirtname.Count)];
                                            selectAccount.Lastname = _listLastname[random.Next(_listLastname.Count)];
                                            selectAccount.Proxy = _listProxy[random.Next(0, _listProxy.Count)];
                                            selectAccount.IsUsing = false;
                                        }
                                    }
                                    else
                                    {
                                        selectAccount.Firstname = _listFirtname[random.Next(_listFirtname.Count)];
                                        selectAccount.Lastname = _listLastname[random.Next(_listLastname.Count)];
                                        selectAccount.Proxy = _listProxy[random.Next(0, _listProxy.Count)];
                                        selectAccount.IsUsing = false;
                                    }
                                }
                                else
                                {
                                    selectAccount.Firstname = _listFirtname[random.Next(_listFirtname.Count)];
                                    selectAccount.Lastname = _listLastname[random.Next(_listLastname.Count)];
                                    selectAccount.Proxy = _listProxy[random.Next(0, _listProxy.Count)];
                                    selectAccount.IsUsing = false;
                                }
                            }
                            else if (checkLayoutFB == 2)
                            {
                                writeLog(device, $"Proxy Fail {selectAccount.Proxy}", selectAccount.IdAccount, device.View.StatusLabel, 1);
                                Random random = new Random();
                                selectAccount.Firstname = _listFirtname[random.Next(_listFirtname.Count)];
                                selectAccount.Lastname = _listLastname[random.Next(_listLastname.Count)];
                                selectAccount.Proxy = _listProxy[random.Next(0, _listProxy.Count)];
                                selectAccount.IsUsing = false;
                            }
                            else
                            {
                                selectAccount.Firstname = _listFirtname[random.Next(_listFirtname.Count)];
                                selectAccount.Lastname = _listLastname[random.Next(_listLastname.Count)];
                                selectAccount.Proxy = _listProxy[random.Next(0, _listProxy.Count)];
                                selectAccount.IsUsing = false;
                            }
                        }

                    }
                    ///////////////////////////Reboot/////////////////////////////////
                    writeLog(device, $"Reboot", null, device.View.StatusLabel);
                    LDController.Close("index", device.IndexLDPlayer);
                    await LDController.DelayAsync(7).ConfigureAwait(false); ;
                    lock (_lockObject)
                    {
                        try
                        {
                            File.Delete($"{LDController.PathFolderLDPlayer}\\vms\\leidian{device.IndexLDPlayer}\\data.vmdk");
                            System.IO.Compression.ZipFile.ExtractToDirectory(Path.Combine(Environment.CurrentDirectory, "LDPlayer\\data.zip"), $"{LDController.PathFolderLDPlayer}\\vms\\leidian{device.IndexLDPlayer}");
                            Thread.Sleep(7000);
                        }
                        catch (Exception ex)
                        {
                            Thread.Sleep(3000);
                            File.Delete($"{LDController.PathFolderLDPlayer}\\vms\\leidian{device.IndexLDPlayer}\\data.vmdk");
                            System.IO.Compression.ZipFile.ExtractToDirectory(Path.Combine(Environment.CurrentDirectory, "LDPlayer\\data.zip"), $"{LDController.PathFolderLDPlayer}\\vms\\leidian{device.IndexLDPlayer}");
                        }

                    }
                    if (cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        if (this.InvokeRequired) // Kiểm tra xem hiện tại có đang ở UI thread hay không
                        {
                            this.Invoke((Action)(() =>
                            {
                                LDController.Close("index", device.IndexLDPlayer);
                                tableLayoutPanel.Controls.Remove(device.View.Panel);
                                RepositionLDPlayers();

                            }));
                            return;
                        }
                        else
                        {
                            LDController.Close("index", device.IndexLDPlayer);
                            tableLayoutPanel.Controls.Remove(device.View.Panel);
                            RepositionLDPlayers();
                            return;
                        }
                    }
                    rebootLDPlayer(device, true);
                    await LDController.DelayAsync(10).ConfigureAwait(false); ;

                }
            }
            catch (Exception ex)
            {
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                Log.Error(ex, ex.Message);
            }

        }
        private void rtbLogs_TextChanged(object sender, EventArgs e)
        {
            rtbLogs.SelectionStart = rtbLogs.Text.Length;
            rtbLogs.ScrollToCaret();
        }

        private void rtbResult_TextChanged(object sender, EventArgs e)
        {
            rtbResult.SelectionStart = rtbResult.Text.Length;
            rtbResult.ScrollToCaret();
        }

        private void fMain_Load(object sender, EventArgs e)
        {
            try
            {
                txtPathLDPlayers.Text = LDController.PathFolderLDPlayer;
                loadAccount();
                loadFileJsonService();
                loadDevice();
                GetConfigDatagridview();
                AdbServer adbServer = new AdbServer();
                adbServer.StartServer($"{LDController.PathFolderLDPlayer}\\adb.exe", true);
                if (cbUpdateAvatar.Checked)
                {
                    txtFolderAvatar.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                MessageCommon.ShowMessageBox(ex.Message, 4);
            }
        }
        private void GetConfigDatagridview()
        {
            try
            {
                dtgvAccount.Columns["cPhone"].Visible = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbPhonenumber");
                dtgvAccount.Columns["cEmail"].Visible = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbEmail");
                dtgvAccount.Columns["cPasswordEmail"].Visible = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbPasswordEmail");
                dtgvAccount.Columns["cServer"].Visible = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbServer");
                dtgvAccount.Columns["cPort"].Visible = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbPort");
                dtgvAccount.Columns["cUsername"].Visible = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbUsername");
                dtgvAccount.Columns["cPassword"].Visible = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbPassword");
                dtgvAccount.Columns["cFullname"].Visible = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbFullname");
                dtgvAccount.Columns["cProxy"].Visible = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbProxy");
                dtgvAccount.Columns["c2FA"].Visible = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbTowFa");
                dtgvAccount.Columns["cBackup"].Visible = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbBackup");
                dtgvAccount.Columns["cStatus"].Visible = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbStatus");
                dtgvAccount.Columns["cAvatar"].Visible = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbAvatar");
                dtgvAccount.Columns["cCreateAt"].Visible = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbDay");
            }
            catch (Exception ex)
            {
                MessageCommon.ShowMessageBox(ex.Message, 4);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                Log.Error(ex, ex.Message);
            }

        }
        private void loadFileJsonService()
        {
            CountryCodeHelper cn = new CountryCodeHelper();
            listCountryCode = cn.GetCountryCodes();
            foreach (var code in listCountryCode)
            {
                listItem.Add(new ComboBoxItem
                {
                    Value = code.Dial_code,
                    DisplayText = code.Name + " " + code.Dial_code
                });
            }
            CBServiceCode.DataSource = listItem;
            var t = jsonHelper.GetValuesFromInputString("OtpCountry");
            if (!string.IsNullOrEmpty(t))
            {
                var countryCodeSelected = listCountryCode.FirstOrDefault(s => s.Dial_code == t);
                if (countryCodeSelected != null)
                {
                    var index = listCountryCode.IndexOf(countryCodeSelected);
                    CBServiceCode.SelectedIndex = index;
                }
            }
            CBoxOtpService_TextChanged(null, null);
        }
        private async void loadDevice()
        {
            try
            {
                var listLDPlayer = await DeviceHelper.GetLDplayersAnysc();
                dtgvLDPlayers.Invoke((MethodInvoker)delegate
                {
                    if (listLDPlayer.Count > 0)
                    {
                        // Clear existing rows
                        dtgvLDPlayers.Rows.Clear();
                        foreach (var device in listLDPlayer)
                        {
                            if (int.Parse(device.Index) < 999)
                            {
                                // Add rows with indexes 1, 5, and 8
                                dtgvLDPlayers.Rows.Add(device.Index, device.Name, device.DeviceId, device.Status);
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageCommon.ShowMessageBox(ex.Message, 4);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                Log.Error(ex, ex.Message);
            }
        }
        private async void dtgvAccount_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void btnLastName_Click(object sender, EventArgs e)
        {
            fAddFile fAddFile = new fAddFile("Last Name", _listLastname);
            fAddFile.ShowDialog();
            _listLastname.Clear();
            try
            {
                using (StreamWriter writer = new StreamWriter(GlobalModels.PathLastName))
                {
                    foreach (var line in fAddFile._list)
                    {
                        _listLastname.Add(line);
                        writer.WriteLine(line);
                    }
                }
            }
            catch (IOException ex)
            {
                Log.Error(ex, ex.Message);
                MessageCommon.ShowMessageBox(ex.Message, 4);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 2);
            }
        }

        private void btnAddFirstname_Click(object sender, EventArgs e)
        {
            fAddFile fAddFile = new fAddFile("First Name", _listFirtname);
            fAddFile.ShowDialog();
            _listFirtname.Clear();
            try
            {
                using (StreamWriter writer = new StreamWriter(GlobalModels.PathFirstName))
                {
                    foreach (var line in fAddFile._list)
                    {
                        _listFirtname.Add(line);
                        writer.WriteLine(line);
                    }
                }
            }
            catch (IOException ex)
            {
                Log.Error(ex, ex.Message);
                MessageCommon.ShowMessageBox(ex.Message, 4);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 2);
            }
        }
        private void radioRadomFullNameUs_CheckedChanged(object sender, EventArgs e)
        {
            plTenTuDat.Enabled = false;
        }

        private void radioRadomFullNameVN_CheckedChanged(object sender, EventArgs e)
        {
            plTenTuDat.Enabled = false;
        }

        private void radioCustomizeFullNameUs_CheckedChanged(object sender, EventArgs e)
        {
            plTenTuDat.Enabled = true;
        }

        private void radioRandomPass_CheckedChanged(object sender, EventArgs e)
        {
            txtPass.Enabled = false;
            NumberPass.Enabled = true;
        }

        private void radioCustomizePass_CheckedChanged(object sender, EventArgs e)
        {
            NumberPass.Enabled = false;
            txtPass.Enabled = true;
        }

        private void btnSettingUserName_Click(object sender, EventArgs e)
        {
            fAddFile fAddFile = new fAddFile("User Name", _listUsername);
            fAddFile.ShowDialog();
            _listUsername.Clear();
            try
            {
                using (StreamWriter writer = new StreamWriter(GlobalModels.PathUserName))
                {
                    foreach (var line in fAddFile._list)
                    {
                        _listUsername.Add(line);
                        writer.WriteLine(line);
                    }
                }
            }
            catch (IOException ex)
            {
                Log.Error(ex, ex.Message);
                MessageCommon.ShowMessageBox(ex.Message, 4);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 2);
            }
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            profileModels.Clear();

            foreach (var device in GlobalModels.Devices)
            {
                LDController.Close("index", device.IndexLDPlayer);
                tableLayoutPanel.Invoke((MethodInvoker)delegate
                {
                    tableLayoutPanel.Controls.Remove(device.View.Panel);
                });
                device.StopTask();
            }
            GlobalModels.Devices.Clear();
            tableLayoutPanel.Controls.Clear();
        }
        private async void btnSetup_Click(object sender, EventArgs e)
        {

            btnSetup.Enabled = false;
            var ldplayer = LDController.GetDevices2().FirstOrDefault();
            DeviceInfo device = new DeviceInfo();
            device.IndexLDPlayer = ldplayer.index.ToString();
            device.AdbClient = new AdbClient();
            device.Data = new DeviceData();
            device.View = new ViewInfo();
            device.View.Embeddedpanel = new Panel();
            device.View.StatusLabel = new Label();
            device.View.LdplayerHandle = new IntPtr();
            device.View.Panel = new Panel();
            device.View.PanelButton = new Panel();
            device.View.BtnClose = new Button();
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            tableLayoutPanel.AutoScroll = true;
            LDController.EditFileConfigLDPlayer();
            LDController.SettingLDPlayerByIndex(device.IndexLDPlayer, 4, 4096);
            await LDController.DelayAsync(1);
            bool isRunning = false;
            for (int i = 0; i < 10; i++)
            {
                LDController.Open("index", device.IndexLDPlayer);
                await LDController.DelayAsync(2);
                if (LDController.IsDevice_Running("index", device.IndexLDPlayer))
                {
                    if (columnCount >= int.Parse(btnDisplay.Text.Trim()))
                    {
                        rowCount++;
                        columnCount = 0;
                        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                    }
                    addViewControl(device, tableLayoutPanel);
                    writeLog(device, "Running", null, device.View.StatusLabel);
                    columnCount++;
                    await LDController.DelayAsync(7);
                    pDevice.Controls.Add(tableLayoutPanel);
                    isRunning = true;
                    break;
                }
            }
            if (isRunning)
            {
                cancellationTokenSource = new CancellationTokenSource();
                List<Task> tasks = new List<Task>();
                tasks.Add(Task.Run(async () =>
                {
                    await SetupLDPlayer(device);
                }, cancellationTokenSource.Token));
                await Task.WhenAll(tasks);
                btnSetup.Enabled = true;
                tableLayoutPanel.Controls.Clear();
                pDevice.Controls.Clear();
            }
        }
        private async Task SetupLDPlayer(DeviceInfo device)
        {
            string adbKeyboard = Path.Combine(Environment.CurrentDirectory, "App\\ADBKeyboard.apk");
            string vpnProxy = Path.Combine(Environment.CurrentDirectory, "App\\VAT-VpnProxy_v1.0.0.apk");
            string youtube = Path.Combine(Environment.CurrentDirectory, "App\\Facebook.apk");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (true)
            {
                await LDController.DelayAsync().ConfigureAwait(false);
                if (DeviceHelper.Connect(device))
                {
                    writeLog(device, "Done Connect", null, device.View.StatusLabel);
                    DataGridViewHelper.SetCellValueByColumnValue(dtgvLDPlayers, device.IndexLDPlayer, "tIndex", "tDeviceId", device.Id, 2);
                    DataGridViewHelper.SetCellValueByColumnValue(dtgvLDPlayers, device.IndexLDPlayer, "tIndex", "tName", device.Data.Model, 2);
                    device.Status = "Connect";
                    sw.Stop();
                    break;
                }
                if (sw.ElapsedMilliseconds > 60000)
                {
                    writeLog(device, "Connect Fail", null, device.View.StatusLabel, 1);
                    device.Status = "reboot";
                    sw.Stop();
                    break;
                }
            }
            if (device.Status == "Connect")
            {
                sw.Start();
                while (true)
                {
                    LDController.InstallApp_File("index", device.IndexLDPlayer, adbKeyboard);
                    await LDController.DelayAsync(1);
                    LDController.InstallApp_File("index", device.IndexLDPlayer, vpnProxy);
                    await LDController.DelayAsync(1);
                    LDController.InstallApp_File("index", device.IndexLDPlayer, youtube);
                    await LDController.DelayAsync(15);
                    var cmdResutl = LDController.ADB("index", device.IndexLDPlayer, "shell pm list package");
                    if (!string.IsNullOrEmpty(cmdResutl) && cmdResutl.Contains("com.facebook.katana") && cmdResutl.Contains("com.android.adbkeyboard") && cmdResutl.Contains("com.vat.vpn"))
                    {
                        writeLog(device, "Done InstallApp", null, device.View.StatusLabel, 2);
                        device.Status = "Done InstallApp";
                        sw.Stop();
                        break;
                    }
                    if (sw.ElapsedMilliseconds > 250000)
                    {
                        writeLog(device, "Fail InstallApp", null, device.View.StatusLabel, 1);
                        device.Status = "reboot";
                        sw.Stop();
                        break;
                    }
                }

            }
            if (device.Status == "Done InstallApp")
            {
                writeLog(device, "Setting", null, device.View.StatusLabel);
                await  ChangeProxy(device.IndexLDPlayer, device.Id, "127.0.0.1.256", "8080", "", "");
                if (BackupDataLDplayer($"{LDController.PathFolderLDPlayer}\\vms\\leidian{device.IndexLDPlayer}\\data.vmdk", Path.Combine(Environment.CurrentDirectory, $"LDplayer")))
                {
                    writeLog(device, "Done", null, device.View.StatusLabel, 2);
                    LDController.Close("index", device.IndexLDPlayer);
                    return;
                }
            }
            if (device.Status == "reboot")
            {
                writeLog(device, "Error", null, device.View.StatusLabel, 1);
                LDController.Close("index", device.IndexLDPlayer);
                return;
            }
        }
        private async Task ChangeProxy(string index, string deviceId, string ip, string port, string? username, string? password)
        {
            try
            {
                string cmd = $"shell am broadcast -a com.vat.vpn.CONNECT_PROXY -n com.vat.vpn/.ui.ProxyReceiver --es address {ip} --es port {port}";
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    cmd = $"shell am broadcast -a com.vat.vpn.CONNECT_PROXY -n com.vat.vpn/.ui.ProxyReceiver --es address {ip} --es port {port}  --es username {username} --es password {password}";
                }
                var connectProxie = ADBHelper.ADB(deviceId, cmd);
                if (string.IsNullOrEmpty(connectProxie) || !connectProxie.Contains("successful"))
                {
                    int i = 6;
                    while (i > 0)
                    {
                        LDController.RunApp("index", index, "com.vat.vpn");
                        string connect = Path.Combine(Environment.CurrentDirectory, "Database\\ImagesClick\\Proxy\\Connect.PNG");
                        string oke = Path.Combine(Environment.CurrentDirectory, "Database\\ImagesClick\\Proxy\\Ok.PNG");
                        string disconnection = Path.Combine(Environment.CurrentDirectory, "Database\\ImagesClick\\Proxy\\Disconnection.PNG");
                        if (LDController.FindImage("index", index, connect, 0.9, 30000))
                        {
                            ADBHelper.TapByPercent(deviceId, 35.6, 39.2);
                            Thread.Sleep(300);
                            ADBHelper.ClearInputWithADBKeyboard(deviceId);
                            Thread.Sleep(300);
                            ADBHelper.InputText(deviceId, ip);
                            Thread.Sleep(300);
                            ADBHelper.TapByPercent(deviceId, 79.8, 39.1);
                            Thread.Sleep(300);
                            ADBHelper.ClearInputWithADBKeyboard(deviceId);
                            Thread.Sleep(300);
                            ADBHelper.InputText(deviceId, port);
                            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
                            {
                                username = "";
                                password = "";
                            }
                            Thread.Sleep(300);
                            ADBHelper.TapByPercent(deviceId, 49.4, 51.6);
                            Thread.Sleep(300);
                            ADBHelper.ClearInputWithADBKeyboard(deviceId);
                            Thread.Sleep(300);
                            ADBHelper.InputText(deviceId, username);
                            Thread.Sleep(300);
                            ADBHelper.TapByPercent(deviceId, 49.7, 61.1);
                            Thread.Sleep(300);
                            ADBHelper.ClearInputWithADBKeyboard(deviceId);
                            Thread.Sleep(300);
                            ADBHelper.InputText(deviceId, password);
                            Thread.Sleep(300);
                            if (ADBHelper.FindImageTap(deviceId, connect, 0.9, 30000))
                            {
                                ADBHelper.FindImageTap(deviceId, oke, 0.9, 30000);
                                if (ADBHelper.FindImage(deviceId, disconnection, 0.29, 30000))
                                {
                                    return;
                                }
                            }

                        }
                        LDController.KillApp("index", index, "com.vat.vpn");
                        i--;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"ERROR: {nameof(fMain)}, params; {nameof(StartDevice)},Device; {index}, Proxy; {ip}:{port}:{username}:{password}, Error; {ex.Message}, Exception; {ex}");
                return;
            }
        }
        private bool BackupDataLDplayer(string fileData, string foler)
        {
            try
            {
                // Tạo thư mục đích nếu nó không tồn tại
                if (!Directory.Exists(foler))
                {
                    Directory.CreateDirectory(foler);
                }

                // Tạo đường dẫn đến tệp Zip
                string zipFilePath = Path.Combine(foler, "data.zip");

                // Sao chép tệp nguồn đến thư mục đích
                string destinationFilePath = Path.Combine(foler, Path.GetFileName(fileData));
                File.Copy(fileData, destinationFilePath, true);

                // Tạo một FileStream để ghi dữ liệu vào tệp Zip
                using (FileStream fs = new FileStream(zipFilePath, FileMode.Create))
                {
                    using (ZipOutputStream zipStream = new ZipOutputStream(fs))
                    {
                        // Tạo một đối tượng ZipEntry cho tệp bạn muốn nén
                        ZipEntry entry = new ZipEntry(Path.GetFileName(destinationFilePath));
                        zipStream.PutNextEntry(entry);

                        // Đọc dữ liệu từ tệp nguồn và sao chép vào tệp Zip
                        byte[] buffer = new byte[4096];
                        using (FileStream sourceStream = new FileStream(destinationFilePath, FileMode.Open, FileAccess.Read))
                        {
                            int bytesRead;
                            while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                zipStream.Write(buffer, 0, bytesRead);
                            }
                        }
                    }
                }

                // Xoá tệp nguồn ở thư mục chỉ định
                File.Delete(destinationFilePath);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"ERROR: {nameof(fMain)}, params; {nameof(BackupDataLDplayer)}, Error; {ex.Message}, Exception; {ex}");
                return false;
            }

        }
        private void writeLog(DeviceInfo device, string message, string? idAccount, Label? label, int type = 0, bool isWriteRichTextBox = true)
        {
            try
            {
                if (!string.IsNullOrEmpty(idAccount))
                {
                    DataGridViewHelper.SetCellValueByColumnValue(dtgvAccount, idAccount, "cIdAccount", "cStatus", message, type);
                }
                if (isWriteRichTextBox)
                {
                    RichTextBoxHelper.WriteLogRichTextBox($"LDPLayer: {device.IndexLDPlayer}  {message}", type);
                }
                if (!string.IsNullOrEmpty(device.IndexLDPlayer))
                {
                    DataGridViewHelper.SetCellValueByColumnValue(dtgvLDPlayers, device.IndexLDPlayer, "tIndex", "tStatus", message, type);
                }
                if (label != null)
                {
                    LabaleHelper.WriteLabale(label, message, type);
                }

            }
            catch (Exception ex)
            {
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                Log.Error(ex, ex.Message);
            }

        }
        private void dtgvLDPlayers_BindingContextChanged(object sender, EventArgs e)
        {
            DataGridView gridView = sender as DataGridView;
            if (null != gridView)
            {
                foreach (DataGridViewRow r in gridView.Rows)
                {
                    gridView.Rows[r.Index].HeaderCell.Value = (r.Index + 1).ToString();
                }
            }
        }

        private void fMain_Resize(object sender, EventArgs e)
        {
            if (this.Width < originalFormWidth)
            {
                // Giới hạn việc thu kéo Form lại không vượt quá Width ban đầu của Form
                this.Width = originalFormWidth;
            }
        }

        private void btnSettingProxy_Click(object sender, EventArgs e)
        {
            fAddFile fAddFile = new fAddFile("Proxy", _listProxy);
            fAddFile.ShowDialog();
            _listProxy.Clear();
            try
            {
                using (StreamWriter writer = new StreamWriter(GlobalModels.PathProxy))
                {
                    foreach (var line in fAddFile._list)
                    {
                        _listProxy.Add(line);
                        writer.WriteLine(line);
                    }
                }
            }
            catch (IOException ex)
            {
                Log.Error(ex, ex.Message);
                MessageCommon.ShowMessageBox(ex.Message, 4);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 2);
            }
        }

        private void cbUpdateAvatar_CheckedChanged(object sender, EventArgs e)
        {
            txtFolderAvatar.Enabled = cbUpdateAvatar.Checked;
        }
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PerformAction("All");
        }

        private void selectAllHighlightedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PerformAction("SelectHighline");
        }

        private void deselectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PerformAction("UnAll");
        }

        private void ExportFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                WriteDataGridViewToFile(saveFileDialog.FileName, dtgvAccount);
            }
        }
        private void WriteDataGridViewToFile(string filePath, DataGridView dataGridView)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    string rowData = "";
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (!cell.Visible || cell.Value == null || cell.ColumnIndex == 0) continue;
                        if (!string.IsNullOrEmpty(rowData))
                        {
                            rowData += " | ";
                        }

                        rowData += cell.Value.ToString();
                    }
                    writer.WriteLine(rowData);
                }
            }
        }
        private void deleteAccToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> list = GetListSelect();
                if (list.Count == 0)
                {
                    MessageCommon.ShowMessageBox("Please select the account to delete!", 4);
                    return;
                }
                if (MessageCommon.ShowConfirmationBox(string.Format("Do you want to delete the {0} selected accounts?", list.Count)) == DialogResult.No)
                {
                    return;
                }
                var check = _accountRepository.DeleteRange(list);
                if (check)
                {
                    MessageCommon.ShowMessageBox("Account deleted successfully!");
                    loadAccount();
                }
                else
                {
                    MessageCommon.ShowMessageBox("Delete failed, please try again later!", 4);
                }
            }
            catch (Exception ex)
            {
                MessageCommon.ShowMessageBox(ex.Message, 4);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                Log.Error(ex, ex.Message);
            }
        }
        private void PerformAction(string action)
        {
            try
            {
                switch (action)
                {
                    case "ToggleCheck":
                        {
                            for (int k = 0; k < dtgvAccount.SelectedRows.Count; k++)
                            {
                                int index = dtgvAccount.SelectedRows[k].Index;
                                SetCellAccount(index, "ChooseCol", !Convert.ToBoolean(GetCellValue(index, "ChooseCol")));
                            }
                            break;
                        }
                    case "SelectHighline":
                        {
                            DataGridViewSelectedRowCollection selectedRows = dtgvAccount.SelectedRows;
                            for (int j = 0; j < selectedRows.Count; j++)
                            {
                                SetCellAccount(selectedRows[j].Index, "ChooseCol", true);
                            }
                            break;
                        }
                    case "UnAll":
                        {
                            for (int l = 0; l < dtgvAccount.RowCount; l++)
                            {
                                SetCellAccount(l, "ChooseCol", false);
                            }
                            break;
                        }
                    case "All":
                        {
                            for (int i = 0; i < dtgvAccount.RowCount; i++)
                            {
                                SetCellAccount(i, "ChooseCol", true);
                            }
                            break;
                        }

                }
                numberAccount.Value = GetListSelect().Count;

            }
            catch (Exception ex)
            {
                MessageCommon.ShowMessageBox(ex.Message, 4);
                Log.Error(ex, ex.Message);
            }

        }
        private void SetCellAccount(int rowIndex, string columnName, object cellValue, bool allowNull = true)
        {
            if (allowNull || !(cellValue.ToString().Trim() == ""))
            {
                var status = DataGridViewHelper.GetCellValue(dtgvAccount, rowIndex, "cStatus");
                switch (status)
                {
                    case "Done":
                        {
                            DataGridViewHelper.SetCellValue(dtgvAccount, rowIndex, columnName, cellValue, 2);
                            break;
                        }
                    case "new":
                        {
                            DataGridViewHelper.SetCellValue(dtgvAccount, rowIndex, columnName, cellValue);
                            break;
                        }
                    default:
                        {
                            DataGridViewHelper.SetCellValue(dtgvAccount, rowIndex, columnName, cellValue, 1);
                            break;
                        }
                }
            }
        }
        private string GetCellValue(int rowIndex, string columnName)
        {
            return DataGridViewHelper.GetCellValue(dtgvAccount, rowIndex, columnName);
        }
        private void btnDisplay_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tableLayoutPanel.Controls.Count > 0)
            {
                RepositionLDPlayers(int.Parse(btnDisplay.Text.Trim()));
            }

        }
        private void fMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageCommon.ShowConfirmationBox("Are you sure you want to close the software?");
                if (result != DialogResult.Yes)
                {
                    e.Cancel = true; // Hủy việc đóng Form nếu người dùng không đồng ý
                }
                else
                {
                    Application.ExitThread();
                    Application.Exit();
                }
            }
        }
        private List<string> GetListSelect()
        {
            List<string> list = new List<string>();
            try
            {
                for (int i = 0; i < dtgvAccount.RowCount; i++)
                {
                    if (Convert.ToBoolean(dtgvAccount.Rows[i].Cells["ChooseCol"].Value))
                    {
                        list.Add(dtgvAccount.Rows[i].Cells["cIdAccount"].Value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageCommon.ShowMessageBox(ex.Message, 4);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                Log.Error(ex, ex.Message);
            }
            return list;
        }
        private void loadAccount()
        {
            try
            {
                lock (_lockObject)
                {
                    var accounts = _accountRepository.GetAll();
                    if (accounts.Count > 0)
                    {
                        dtgvAccount.Invoke((MethodInvoker)delegate
                        {
                            dtgvAccount.Rows.Clear();
                            int i = 1;
                            foreach (var a in accounts)
                            {
                                string gender = "";
                                if (a.Gender == 0)
                                {
                                    gender = "Male";
                                }
                                else if (a.Gender == 1)
                                {
                                    gender = "Female";
                                }
                                dtgvAccount.Rows.Add(false, i, a.Phonenumber, a.Email, a.PasswordEmail, a.Server, a.Port, a.Uid, a.Password, a.FullName, a.Proxy, a.TowFA, a.Token, a.Backup, a.Avatar, gender, a.Status, a.CreateDate, a.Id);
                                switch (a.Status)
                                {
                                    case "new":
                                        {
                                            dtgvAccount.Rows[i - 1].DefaultCellStyle.BackColor = Color.White;
                                            i++;
                                            continue;
                                        }
                                    case "Done":
                                        {
                                            dtgvAccount.Rows[i - 1].DefaultCellStyle.BackColor = Color.LightGreen;
                                            i++;
                                            continue;
                                        }
                                    default:
                                        {
                                            dtgvAccount.Rows[i - 1].DefaultCellStyle.BackColor = Color.Pink;
                                            i++;
                                            continue;
                                        }

                                }
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
            }
        }
        private void dtgvAccount_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex > -1)
            {
                try
                {
                    dtgvAccount.CurrentRow.Cells["ChooseCol"].Value = !Convert.ToBoolean(dtgvAccount.CurrentRow.Cells["ChooseCol"].Value);
                    numberAccount.Value = GetListSelect().Count;
                }
                catch (Exception ex)
                {
                    MessageCommon.ShowMessageBox(ex.Message, 4);
                    RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                    Log.Error(ex, ex.Message);
                }
            }
        }

        private void dtgvAccount_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                try
                {
                    dtgvAccount.CurrentRow.Cells["ChooseCol"].Value = !Convert.ToBoolean(dtgvAccount.CurrentRow.Cells["ChooseCol"].Value);
                    numberAccount.Value = GetListSelect().Count;
                }
                catch (Exception ex)
                {
                    MessageCommon.ShowMessageBox(ex.Message, 4);
                    RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                    Log.Error(ex, ex.Message);
                }
            }
        }

        private void addViewControl(DeviceInfo device, TableLayoutPanel tableLayoutPanel)
        {
            try
            {
                //IntPtr ldPlayerHandle = IntPtr.Zero; // Lưu trữ handle của cha trước khi gim
                string name = $"Qnibot{device.IndexLDPlayer}";
                LDController.ReName("index", device.IndexLDPlayer, name);
                bool result = true;
                while (result)
                {
                    Thread.Sleep(2000);
                    // Thử tìm LDPlayer với tên "name"
                    var procs = Process.GetProcessesByName("dnplayer");
                    foreach (var proc in procs)
                    {
                        if (proc.MainWindowTitle == name)
                        {
                            device.View.LdplayerHandle = proc.MainWindowHandle;

                            // Hiển thị LDPlayer trong panel nếu nó đã bị gỡ gim
                            if (!device.View.IsPinned && device.View.LdplayerHandle != IntPtr.Zero)
                            {
                                device.View.Panel.Size = new Size(200, 400);
                                device.View.Embeddedpanel.Size = new Size(200, 350);
                                device.View.Embeddedpanel.Location = new Point(0, 0);
                                device.View.Embeddedpanel.Enabled = false;
                                SetParent(device.View.LdplayerHandle, device.View.Embeddedpanel.Handle);
                                MoveWindow(device.View.LdplayerHandle, 0, 0, device.View.Embeddedpanel.Width, device.View.Embeddedpanel.Height, true);
                                device.View.Panel.Controls.Add(device.View.Embeddedpanel);

                                device.View.PanelButton.Size = new Size(200, 50);
                                device.View.PanelButton.Location = new Point(0, device.View.Embeddedpanel.Height);

                                // Thêm Label cho mỗi LDPlayer
                                device.View.StatusLabel.ForeColor = Color.Green;
                                device.View.StatusLabel.Text = "Running";
                                device.View.StatusLabel.Dock = DockStyle.Bottom;
                                device.View.PanelButton.Controls.Add(device.View.StatusLabel);

                                // Thêm Button để gỡ gim hoặc gim lại
                                device.View.BtnClose.Text = device.View.IsPinned ? "Pin" : "Unpin";
                                device.View.BtnClose.Dock = DockStyle.Bottom;
                                PictureBox iconPictureBox = new PictureBox();
                                iconPictureBox.Size = new Size(16, 16); // Kích thước biểu tượng
                                iconPictureBox.Location = new Point(5, 5); // Vị trí của biểu tượng trong nút
                                iconPictureBox.Image = Properties.Resources.UnpinIcon; // Thay đổi thành biểu tượng của bạn
                                device.View.BtnClose.Click += (sender, args) =>
                                {
                                    device.View.IsPinned = !device.View.IsPinned;
                                    // Đảo ngược trạng thái của LDPlayer (gỡ gim hoặc gim lại)
                                    if (device.View.IsPinned)
                                    {
                                        // Gỡ gim LDPlayer
                                        device.View.Embeddedpanel.Invoke((Action)(() =>
                                        {
                                            SetParent(device.View.LdplayerHandle, device.View.originalParentHandle);
                                        }));
                                        //SetParent(device.view.LdplayerHandle, device.view.originalParentHandle); // Đặt lại cha của LDPlayer
                                        MoveWindow(device.View.LdplayerHandle, 0, 0, device.View.Embeddedpanel.Width, device.View.Embeddedpanel.Height, true);
                                        iconPictureBox.Image = Properties.Resources.PinIcon;
                                    }
                                    else
                                    {
                                        // Lưu trữ cha hiện tại của LDPlayer và gỡ gim
                                        device.View.originalParentHandle = NativeMethods.GetParent(device.View.LdplayerHandle);
                                        device.View.Embeddedpanel.Invoke((Action)(() =>
                                        {
                                            SetParent(device.View.LdplayerHandle, device.View.Embeddedpanel.Handle);
                                        }));
                                        //SetParent(device.view.LdplayerHandle, device.view.Embeddedpanel.Handle);
                                        MoveWindow(device.View.LdplayerHandle, 0, 0, device.View.Embeddedpanel.Width, device.View.Embeddedpanel.Height, true);
                                        iconPictureBox.Image = Properties.Resources.UnpinIcon;
                                    }
                                    device.View.BtnClose.Text = device.View.IsPinned ? "Pin" : "Unpin";
                                };
                                device.View.BtnClose.Controls.Add(iconPictureBox);
                                device.View.PanelButton.Controls.Add(device.View.BtnClose);
                                device.View.Panel.Controls.Add(device.View.PanelButton);
                                tableLayoutPanel.Controls.Add(device.View.Panel);
                                tableLayoutPanel.SetCellPosition(device.View.Panel, new TableLayoutPanelCellPosition(columnCount, rowCount));
                                pDevice.Controls.Add(tableLayoutPanel);
                                result = false;
                                break;
                            }
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                MessageCommon.ShowMessageBox(ex.Message, 4);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                Log.Error(ex, ex.Message);
            }
        }
        public class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern IntPtr GetParent(IntPtr hWnd);
        }
        private void RepositionLDPlayers(int isRow = 5)
        {
            try
            {
                tableLayoutPanel.Invoke((MethodInvoker)delegate
                {
                    columnCount = 0;
                    rowCount = 0;
                    foreach (Control control in tableLayoutPanel.Controls)
                    {
                        if (control is Panel embeddedPanel)
                        {
                            int column = tableLayoutPanel.GetColumn(embeddedPanel);
                            int row = tableLayoutPanel.GetRow(embeddedPanel);

                            if (columnCount >= isRow)
                            {
                                rowCount++;
                                columnCount = 0;
                            }

                            tableLayoutPanel.SetCellPosition(embeddedPanel, new TableLayoutPanelCellPosition(columnCount, rowCount));
                            columnCount++;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                MessageCommon.ShowMessageBox(ex.Message, 4);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                Log.Error(ex, ex.Message);
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        public void EmbedLDPlayer(IntPtr ldPlayerHandle, IntPtr panelHandle)
        {
            try
            {
                SetParent(ldPlayerHandle, panelHandle);
                const int GWL_STYLE = -16;
                const int WS_VISIBLE = 0x10000000;
                const int WS_CHILD = 0x40000000;
                int style = GetWindowLong(ldPlayerHandle, GWL_STYLE);
                style = style & ~WS_VISIBLE;
                style = style | WS_CHILD;
                SetWindowLong(ldPlayerHandle, GWL_STYLE, new IntPtr(style));
                MoveWindow(ldPlayerHandle, 0, 0, Width, Height, true);
            }
            catch (Exception ex)
            {
                MessageCommon.ShowMessageBox(ex.Message, 4);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                Log.Error(ex, ex.Message);
            }

        }
        private void rebootLDPlayer(DeviceInfo device, bool isChange = true)
        {
            try
            {

                bool result = true;
                while (result)
                {
                    LDController.Open("index", device.IndexLDPlayer, isChange);
                    Thread.Sleep(5000);
                    var name = $"Qnibot{device.IndexLDPlayer}";
                    LDController.ReName("index", device.IndexLDPlayer, name);
                    var proc1 = Process.GetProcessesByName("dnplayer");
                    Parallel.ForEach(proc1, proc =>
                    {
                        if (proc.MainWindowTitle == name)
                        {
                            Thread.Sleep(1000);
                            device.View.LdplayerHandle = proc.MainWindowHandle;
                            device.View.Embeddedpanel.Invoke((Action)(() =>
                            {
                                SetParent(device.View.LdplayerHandle, device.View.Embeddedpanel.Handle);
                            }));
                            MoveWindow(device.View.LdplayerHandle, 0, 0, device.View.Embeddedpanel.Width, device.View.Embeddedpanel.Height, true);
                            Thread.Sleep(3000);
                            result = false;
                            device.View.IsPinned = false; // Hoặc true tùy thuộc vào trạng thái sau reboot
                            device.View.BtnClose.Text = device.View.IsPinned ? "Pin" : "Unpin";
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                Log.Error(ex, ex.Message);
            }
        }

        private void rtbLogs_DoubleClick(object sender, EventArgs e)
        {
            using (StreamWriter writer = new StreamWriter("Log.txt"))
            {
                if (!File.Exists("Log.txt"))
                {
                    File.WriteAllBytes("Log.txt", new byte[0]);
                }

                writer.Write(rtbLogs.Text);
                writer.Close();
            }
            string notepadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "notepad.exe");
            Process.Start(notepadPath, Path.Combine("Log.txt"));
        }

        private void rtbResult_DoubleClick(object sender, EventArgs e)
        {
            using (StreamWriter writer = new StreamWriter("Result.txt"))
            {
                if (!File.Exists("Result.txt"))
                {
                    File.WriteAllBytes("Result.txt", new byte[0]);
                }
                writer.Write(rtbResult.Text);
                writer.Close();
            }
            string notepadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "notepad.exe");
            Process.Start(notepadPath, Path.Combine("Result.txt"));
        }

        private void btnLoadDevice_Click(object sender, EventArgs e)
        {
            loadDevice();
        }

        private void dtgvLDPlayers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex > -1)
            {
                try
                {
                    dtgvLDPlayers.CurrentRow.Cells["cChose"].Value = !Convert.ToBoolean(dtgvLDPlayers.CurrentRow.Cells["cChose"].Value);
                }
                catch (Exception ex)
                {
                    MessageCommon.ShowMessageBox(ex.Message, 4);
                    RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 1);
                    Log.Error(ex, ex.Message);
                }
            }
        }

        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            fAddFile fAddFile = new fAddFile("Accounts", _listEmail);
            fAddFile.ShowDialog();
            _listEmail.Clear();
            try
            {
                using (StreamWriter writer = new StreamWriter(GlobalModels.PathEmail))
                {
                    foreach (var line in fAddFile._list)
                    {
                        _listEmail.Add(line);
                        writer.WriteLine(line);
                    }
                }
            }
            catch (IOException ex)
            {
                Log.Error(ex, ex.Message);
                MessageCommon.ShowMessageBox(ex.Message, 4);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 2);
            }
        }

        private void dtgvAccount_SelectionChanged(object sender, EventArgs e)
        {
        }

        private void CBoxOtpService_TextChanged(object sender, EventArgs e)
        {
            if (CBoxOtpService.Text == "Viotp.com")
            {
                CBServiceCode.Visible = false;
                plContry.Visible = true;
            }
            else
            {
                CBServiceCode.Visible = true;
                plContry.Visible = false;
            }
        }

        private void CBServiceCode_SelectedValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_OtpCountry))
            {
                _OtpCountry = CBServiceCode.SelectedValue.ToString();
                SettingsTool.GetSettings("configGeneral").AddValue("OtpCountry", _OtpCountry);
                SettingsTool.UpdateSetting("configGeneral");
                if (CBoxOtpService.Text == Services.Web5Sim)
                {

                    var line = listCountryCode.Where(x => x.Dial_code == _OtpCountry).FirstOrDefault().alias;
                    if (!string.IsNullOrEmpty(line))
                    {
                        GlobalModels.Contry = line;
                    }
                    else
                    {
                        MessageCommon.ShowMessageBox(" No Support Country", 3);
                        return;
                    }

                }
                if (CBoxOtpService.Text == Services.Sms365 || CBoxOtpService.Text.Contains(Services.Getsmscode))
                {
                    var line = listCountryCode.Where(x => x.Dial_code == _OtpCountry).FirstOrDefault().countryId;
                    if (!string.IsNullOrEmpty(line))
                    {
                        GlobalModels.Contry = line;
                    }
                    else
                    {
                        MessageCommon.ShowMessageBox("No Support Country", 3);
                        return;
                    }
                }
            }
        }

        private void rbRegWithPhone_CheckedChanged(object sender, EventArgs e)
        {
            gbPhone.Visible = true;
            btnAddAccount.Visible = false;
        }

        private void rbRegWithEmail_CheckedChanged(object sender, EventArgs e)
        {
            gbPhone.Visible = false;
            btnAddAccount.Visible = true;
        }

        private void ckbAddEmail_CheckedChanged(object sender, EventArgs e)
        {
            btnSettingEmail.Enabled = ckbAddEmail.Checked;
        }

        private void btnSettingEmail_Click(object sender, EventArgs e)
        {
            fAddFile fAddFile = new fAddFile("EmailRecovery", _listEmailRecovery);
            fAddFile.ShowDialog();
            _listEmailRecovery.Clear();
            try
            {
                using (StreamWriter writer = new StreamWriter(GlobalModels.PathEmailRecovery))
                {
                    foreach (var line in fAddFile._list)
                    {
                        _listEmailRecovery.Add(line);
                        writer.WriteLine(line);
                    }
                }
            }
            catch (IOException ex)
            {
                Log.Error(ex, ex.Message);
                MessageCommon.ShowMessageBox(ex.Message, 4);
                RichTextBoxHelper.WriteLogRichTextBox(ex.Message, 2);
            }
        }

        private void btnDisPlayConfiguration_Click(object sender, EventArgs e)
        {
            FormHelper.ShowFormWithoutTaskbar(new fDisplayConf());
            GetConfigDatagridview();
        }
    }
}