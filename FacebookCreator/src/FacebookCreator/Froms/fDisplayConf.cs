using FacebookCreator.Helper;

namespace FacebookCreator.Forms
{
    public partial class fDisplayConf : Form
    {
        internal static bool add = false;
        internal static string folderName = string.Empty;

        public fDisplayConf()
        {
            InitializeComponent();
            CommonMethods.WireUpMouseEvents(bunifuCustomLabel1, btnCancel);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            CommonMethods.CloseForm(this);
        }

        private void fDisplayConf_Load(object sender, EventArgs e)
        {
            ckbEmail.Checked = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbEmail");
            ckbPhonenumber.Checked = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbPhonenumber");
            ckbUsername.Checked = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbUsername");
            ckbFullname.Checked = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbFullname");
            ckbPassword.Checked = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbPassword");
            ckbProxy.Checked = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbProxy");
            ckbStatus.Checked = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbStatus");
            ckbBackup.Checked = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbBackup");
            ckbTowFa.Checked = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbTowFa");
            ckbServer.Checked = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbServer");
            ckbPort.Checked = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbPort");
            ckbAvatar.Checked = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbAvatar");
            ckbPasswordEmail.Checked = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbPasswordEmail");
            ckbDay.Checked = SettingsTool.GetSettings("configDatagridview").GetBooleanValue("ckbDay");

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SettingsTool.GetSettings("configDatagridview").AddValue("ckbEmail", ckbEmail.Checked);
            SettingsTool.GetSettings("configDatagridview").AddValue("ckbPhonenumber", ckbPhonenumber.Checked);
            SettingsTool.GetSettings("configDatagridview").AddValue("ckbUsername", ckbUsername.Checked);
            SettingsTool.GetSettings("configDatagridview").AddValue("ckbFullname", ckbFullname.Checked);
            SettingsTool.GetSettings("configDatagridview").AddValue("ckbPassword", ckbPassword.Checked);
            SettingsTool.GetSettings("configDatagridview").AddValue("ckbProxy", ckbProxy.Checked);
            SettingsTool.GetSettings("configDatagridview").AddValue("ckbStatus", ckbStatus.Checked);
            SettingsTool.GetSettings("configDatagridview").AddValue("ckbBackup", ckbBackup.Checked);
            SettingsTool.GetSettings("configDatagridview").AddValue("ckbTowFa", ckbTowFa.Checked);
            SettingsTool.GetSettings("configDatagridview").AddValue("ckbServer", ckbServer.Checked);
            SettingsTool.GetSettings("configDatagridview").AddValue("ckbPort", ckbPort.Checked);
            SettingsTool.GetSettings("configDatagridview").AddValue("ckbAvatar", ckbAvatar.Checked);
            SettingsTool.GetSettings("configDatagridview").AddValue("ckbPasswordEmail", ckbPasswordEmail.Checked);
            SettingsTool.GetSettings("configDatagridview").AddValue("ckbDay", ckbDay.Checked);
            SettingsTool.UpdateSetting("configDatagridview");
            CommonMethods.CloseForm(this);
        }
    }
}
