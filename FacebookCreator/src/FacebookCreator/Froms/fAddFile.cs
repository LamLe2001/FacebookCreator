using FacebookCreator.Entities;
using FacebookCreator.Helper;
using FacebookCreator.ProxyDroid;
using FacebookCreator.Repositories;
using Serilog;
using System.Windows.Shapes;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FacebookCreator.Froms
{
    public partial class fAddFile : Form
    {
        private string _type { get; set; }
        internal List<string> _list = new List<string>();
        public fAddFile(string type, List<string> list)
        {
            _type = type;
            InitializeComponent();
            CommonMethods.WireUpMouseEvents(lblHeader, btnClose);
            lblHeader.Text = $"Import {type}";
            string str2 = "System.Collections.Generic.List`1[System.String]";
            if (list.Count > 0 && !list[0].Equals(str2))
            {
                rtbData.Lines = list.ToArray();
            }
            _list.AddRange(list);
            lblTotal.Text = rtbData.Lines.ToList().Count.ToString();
            switch (type)
            {
                case "Proxy":
                    {
                        lbNotifi.Text = "Format: address:port:username:password";
                        return;
                    }
                case "Accounts":
                    {
                        lbNotifi.Text = "Format: email|password|server|port";
                        return;
                    }
                default:
                    {
                        lbNotifi.Visible = false;
                        return;
                    }
            }

        }
        private async void btnSave_Click(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            try
            {
                switch (_type)
                {
                    case "Proxy":
                        {
                            _list.Clear();
                            int success = 0;
                            int error = 0;
                            List<Task> tasks = new List<Task>();
                            foreach (var item in rtbData.Lines)
                            {
                                tasks.Add(Task.Run(async () =>
                                {
                                    ProcessLineAsync(item, ref success, ref error);
                                }));
                            }
                            await Task.WhenAll(tasks);
                            lblSuccess.Text = success.ToString();
                            lblError.Text = error.ToString();

                            break;
                        }
                    case "Accounts":
                        {
                            _list.Clear();
                            int success = 0;
                            int error = 0;
                            List<Task> tasks = new List<Task>();
                            foreach (var item in rtbData.Lines)
                            {
                                if (!string.IsNullOrEmpty(item))
                                {
                                    tasks.Add(Task.Run(() =>
                                    {
                                        ProcessLine(item, ref success, ref error);
                                    }));
                                }
                            }
                            await Task.WhenAll(tasks);
                            lblSuccess.Text = success.ToString();
                            lblError.Text = error.ToString();
                            break;
                        }
                    case "EmailRecovery":
                        {
                            _list.Clear();
                            int success = 0;
                            int error = 0;
                            foreach (var item in rtbData.Lines)
                            {
                                if (!string.IsNullOrEmpty(item))
                                {
                                    var parts = item.Split('|');
                                    try
                                    {
                                        if (parts.Length >= 4 && !string.IsNullOrEmpty(parts[0]) && !string.IsNullOrEmpty(parts[1]) && !string.IsNullOrEmpty(parts[2]) && int.TryParse(parts[3], out int result))
                                        {
                                            _list.Add(item);
                                            success++;
                                        }
                                        else
                                        {
                                            error++;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Error(ex, ex.Message);
                                        error++;
                                    }
                                }
                            }
                            lblSuccess.Text = success.ToString();
                            lblError.Text = error.ToString();
                            SettingsTool.GetSettings("configGeneral").AddValue("ckbCatchAll", ckbCatchAll.Checked);
                            SettingsTool.UpdateSetting("configGeneral");
                            break;
                        }
                    default:
                        {
                            _list = rtbData.Text.Split(new string[1] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            lblSuccess.Text = _list.Count.ToString();
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                MessageCommon.ShowMessageBox(ex.Message, 4);
                btnSave.Enabled = true;
                return;
            }
            MessageCommon.ShowMessageBox("Successfully Import");
            btnSave.Enabled = true;
        }
        private void ProcessLine(string line, ref int success, ref int error)
        {
            var parts = line.Split('|');
            try
            {
                if (parts.Length >= 4 && !string.IsNullOrEmpty(parts[0]) && !string.IsNullOrEmpty(parts[1]) && !string.IsNullOrEmpty(parts[2]) && int.TryParse(parts[3], out int result))
                {
                    lock (_list) // Ensure thread-safe access to _list
                    {
                        _list.Add(line);
                        Interlocked.Increment(ref success);
                    }
                }
                else
                {
                    Interlocked.Increment(ref error);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                Interlocked.Increment(ref error);
            }
        }
        private void ProcessLineAsync(string line, ref int success, ref int error)
        {
            var parts = line.Split(':');
            try
            {
                if (parts.Length >= 2 && !string.IsNullOrEmpty(parts[0]) && !string.IsNullOrEmpty(parts[1]))
                {
                    lock (_list)
                    {
                        _list.Add(line);
                        Interlocked.Increment(ref success);
                    }
                }
                else
                {
                    Interlocked.Increment(ref error);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                Interlocked.Increment(ref error);
            }
        }
        public class ProcessingResult
        {
            public int SuccessCount { get; set; }
            public int ErrorCount { get; set; }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rtbData_TextChanged(object sender, EventArgs e)
        {
            List<string> listAccount = rtbData.Lines.ToList();
            listAccount = CommonMethods.RemoveEmptyLines(listAccount);
            lblTotal.Text = listAccount.Count.ToString();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
