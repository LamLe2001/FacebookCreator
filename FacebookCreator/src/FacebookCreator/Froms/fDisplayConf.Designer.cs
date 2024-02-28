namespace FacebookCreator.Forms
{
    partial class fDisplayConf
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fDisplayConf));
            bunifuDragControl1 = new Bunifu.Framework.UI.BunifuDragControl(components);
            panel1 = new Panel();
            button1 = new Button();
            bunifuCustomLabel1 = new Bunifu.Framework.UI.BunifuCustomLabel();
            btnCancel = new Button();
            btnAdd = new Button();
            ckbPort = new CheckBox();
            ckbBackup = new CheckBox();
            ckbServer = new CheckBox();
            ckbEmail = new CheckBox();
            ckbProxy = new CheckBox();
            ckbPassword = new CheckBox();
            ckbFullname = new CheckBox();
            ckbPhonenumber = new CheckBox();
            ckbUsername = new CheckBox();
            ckbTowFa = new CheckBox();
            DB37CC26 = new Bunifu.Framework.UI.BunifuCustomLabel();
            ckbAvatar = new CheckBox();
            ckbPasswordEmail = new CheckBox();
            ckbDay = new CheckBox();
            ckbStatus = new CheckBox();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // bunifuDragControl1
            // 
            bunifuDragControl1.Fixed = true;
            bunifuDragControl1.Horizontal = true;
            bunifuDragControl1.TargetControl = null;
            bunifuDragControl1.Vertical = true;
            // 
            // panel1
            // 
            panel1.BackColor = Color.White;
            panel1.Controls.Add(button1);
            panel1.Controls.Add(bunifuCustomLabel1);
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(355, 38);
            panel1.TabIndex = 0;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.BackColor = Color.CornflowerBlue;
            button1.Cursor = Cursors.Hand;
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            button1.ForeColor = Color.White;
            button1.Image = (Image)resources.GetObject("button1.Image");
            button1.Location = new Point(320, 1);
            button1.Margin = new Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new Size(35, 35);
            button1.TabIndex = 78;
            button1.TextImageRelation = TextImageRelation.ImageBeforeText;
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // bunifuCustomLabel1
            // 
            bunifuCustomLabel1.BackColor = Color.CornflowerBlue;
            bunifuCustomLabel1.Cursor = Cursors.SizeAll;
            bunifuCustomLabel1.Dock = DockStyle.Fill;
            bunifuCustomLabel1.Font = new Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            bunifuCustomLabel1.ForeColor = Color.Transparent;
            bunifuCustomLabel1.Location = new Point(0, 0);
            bunifuCustomLabel1.Margin = new Padding(4, 0, 4, 0);
            bunifuCustomLabel1.Name = "bunifuCustomLabel1";
            bunifuCustomLabel1.Size = new Size(355, 38);
            bunifuCustomLabel1.TabIndex = 15;
            bunifuCustomLabel1.Text = "Display configuration";
            bunifuCustomLabel1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom;
            btnCancel.BackColor = Color.FromArgb(192, 0, 0);
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(200, 277);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(107, 33);
            btnCancel.TabIndex = 189;
            btnCancel.Text = "Close";
            btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnAdd
            // 
            btnAdd.Anchor = AnchorStyles.Bottom;
            btnAdd.BackColor = Color.Green;
            btnAdd.Cursor = Cursors.Hand;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.Font = new Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            btnAdd.ForeColor = Color.White;
            btnAdd.Location = new Point(48, 277);
            btnAdd.Margin = new Padding(4, 3, 4, 3);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(107, 33);
            btnAdd.TabIndex = 188;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = false;
            btnAdd.Click += btnAdd_Click;
            // 
            // ckbPort
            // 
            ckbPort.AutoSize = true;
            ckbPort.Cursor = Cursors.Hand;
            ckbPort.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            ckbPort.Location = new Point(200, 163);
            ckbPort.Name = "ckbPort";
            ckbPort.Size = new Size(49, 20);
            ckbPort.TabIndex = 190;
            ckbPort.Text = "Port";
            ckbPort.UseVisualStyleBackColor = true;
            // 
            // ckbBackup
            // 
            ckbBackup.AutoSize = true;
            ckbBackup.Cursor = Cursors.Hand;
            ckbBackup.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            ckbBackup.Location = new Point(200, 85);
            ckbBackup.Name = "ckbBackup";
            ckbBackup.Size = new Size(66, 20);
            ckbBackup.TabIndex = 210;
            ckbBackup.Text = "Backup";
            ckbBackup.UseVisualStyleBackColor = true;
            // 
            // ckbServer
            // 
            ckbServer.AutoSize = true;
            ckbServer.Cursor = Cursors.Hand;
            ckbServer.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            ckbServer.Location = new Point(200, 137);
            ckbServer.Name = "ckbServer";
            ckbServer.Size = new Size(64, 20);
            ckbServer.TabIndex = 209;
            ckbServer.Text = "Server";
            ckbServer.UseVisualStyleBackColor = true;
            // 
            // ckbEmail
            // 
            ckbEmail.AutoSize = true;
            ckbEmail.Cursor = Cursors.Hand;
            ckbEmail.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            ckbEmail.Location = new Point(48, 85);
            ckbEmail.Name = "ckbEmail";
            ckbEmail.Size = new Size(57, 20);
            ckbEmail.TabIndex = 208;
            ckbEmail.Text = "Email";
            ckbEmail.UseVisualStyleBackColor = true;
            // 
            // ckbProxy
            // 
            ckbProxy.AutoSize = true;
            ckbProxy.Cursor = Cursors.Hand;
            ckbProxy.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            ckbProxy.Location = new Point(48, 215);
            ckbProxy.Name = "ckbProxy";
            ckbProxy.Size = new Size(57, 20);
            ckbProxy.TabIndex = 207;
            ckbProxy.Text = "Proxy";
            ckbProxy.UseVisualStyleBackColor = true;
            // 
            // ckbPassword
            // 
            ckbPassword.AutoSize = true;
            ckbPassword.Cursor = Cursors.Hand;
            ckbPassword.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            ckbPassword.Location = new Point(48, 189);
            ckbPassword.Name = "ckbPassword";
            ckbPassword.Size = new Size(81, 20);
            ckbPassword.TabIndex = 206;
            ckbPassword.Text = "Password";
            ckbPassword.UseVisualStyleBackColor = true;
            // 
            // ckbFullname
            // 
            ckbFullname.AutoSize = true;
            ckbFullname.Cursor = Cursors.Hand;
            ckbFullname.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            ckbFullname.Location = new Point(48, 163);
            ckbFullname.Name = "ckbFullname";
            ckbFullname.Size = new Size(78, 20);
            ckbFullname.TabIndex = 205;
            ckbFullname.Text = "Fullname";
            ckbFullname.UseVisualStyleBackColor = true;
            // 
            // ckbPhonenumber
            // 
            ckbPhonenumber.AutoSize = true;
            ckbPhonenumber.Cursor = Cursors.Hand;
            ckbPhonenumber.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            ckbPhonenumber.Location = new Point(48, 111);
            ckbPhonenumber.Name = "ckbPhonenumber";
            ckbPhonenumber.Size = new Size(109, 20);
            ckbPhonenumber.TabIndex = 202;
            ckbPhonenumber.Text = "Phone number";
            ckbPhonenumber.UseVisualStyleBackColor = true;
            // 
            // ckbUsername
            // 
            ckbUsername.AutoSize = true;
            ckbUsername.Cursor = Cursors.Hand;
            ckbUsername.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            ckbUsername.Location = new Point(48, 137);
            ckbUsername.Name = "ckbUsername";
            ckbUsername.Size = new Size(84, 20);
            ckbUsername.TabIndex = 193;
            ckbUsername.Text = "Username";
            ckbUsername.UseVisualStyleBackColor = true;
            // 
            // ckbTowFa
            // 
            ckbTowFa.AutoSize = true;
            ckbTowFa.Cursor = Cursors.Hand;
            ckbTowFa.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            ckbTowFa.Location = new Point(200, 111);
            ckbTowFa.Name = "ckbTowFa";
            ckbTowFa.Size = new Size(48, 20);
            ckbTowFa.TabIndex = 213;
            ckbTowFa.Text = "2FA";
            ckbTowFa.UseVisualStyleBackColor = true;
            // 
            // DB37CC26
            // 
            DB37CC26.BackColor = Color.Transparent;
            DB37CC26.Cursor = Cursors.SizeAll;
            DB37CC26.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            DB37CC26.ForeColor = Color.Black;
            DB37CC26.Location = new Point(0, 41);
            DB37CC26.Name = "DB37CC26";
            DB37CC26.Size = new Size(355, 32);
            DB37CC26.TabIndex = 214;
            DB37CC26.Text = "Please select the columns to display!";
            DB37CC26.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // ckbAvatar
            // 
            ckbAvatar.AutoSize = true;
            ckbAvatar.Cursor = Cursors.Hand;
            ckbAvatar.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            ckbAvatar.Location = new Point(200, 189);
            ckbAvatar.Name = "ckbAvatar";
            ckbAvatar.Size = new Size(63, 20);
            ckbAvatar.TabIndex = 215;
            ckbAvatar.Text = "Avatar";
            ckbAvatar.UseVisualStyleBackColor = true;
            // 
            // ckbPasswordEmail
            // 
            ckbPasswordEmail.AutoSize = true;
            ckbPasswordEmail.Cursor = Cursors.Hand;
            ckbPasswordEmail.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            ckbPasswordEmail.Location = new Point(200, 215);
            ckbPasswordEmail.Name = "ckbPasswordEmail";
            ckbPasswordEmail.Size = new Size(116, 20);
            ckbPasswordEmail.TabIndex = 216;
            ckbPasswordEmail.Text = "Password Email";
            ckbPasswordEmail.UseVisualStyleBackColor = true;
            // 
            // ckbDay
            // 
            ckbDay.AutoSize = true;
            ckbDay.Cursor = Cursors.Hand;
            ckbDay.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            ckbDay.Location = new Point(200, 241);
            ckbDay.Name = "ckbDay";
            ckbDay.Size = new Size(47, 20);
            ckbDay.TabIndex = 218;
            ckbDay.Text = "Day";
            ckbDay.UseVisualStyleBackColor = true;
            // 
            // ckbStatus
            // 
            ckbStatus.AutoSize = true;
            ckbStatus.Cursor = Cursors.Hand;
            ckbStatus.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point);
            ckbStatus.Location = new Point(48, 241);
            ckbStatus.Name = "ckbStatus";
            ckbStatus.Size = new Size(62, 20);
            ckbStatus.TabIndex = 217;
            ckbStatus.Text = "Status";
            ckbStatus.UseVisualStyleBackColor = true;
            // 
            // fDisplayConf
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(355, 333);
            Controls.Add(ckbDay);
            Controls.Add(ckbStatus);
            Controls.Add(ckbPasswordEmail);
            Controls.Add(ckbAvatar);
            Controls.Add(DB37CC26);
            Controls.Add(ckbPort);
            Controls.Add(ckbBackup);
            Controls.Add(ckbServer);
            Controls.Add(ckbEmail);
            Controls.Add(ckbProxy);
            Controls.Add(ckbPassword);
            Controls.Add(ckbFullname);
            Controls.Add(ckbPhonenumber);
            Controls.Add(ckbUsername);
            Controls.Add(ckbTowFa);
            Controls.Add(btnCancel);
            Controls.Add(btnAdd);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4, 3, 4, 3);
            Name = "fDisplayConf";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "fAddFile";
            Load += fDisplayConf_Load;
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Bunifu.Framework.UI.BunifuDragControl bunifuDragControl1;
        private Panel panel1;
        private Button btnCancel;
        private Button btnAdd;
        private Button button1;
        private CheckBox ckbXoaAnhDaDung;
        internal CheckBox ckbPort;
        internal CheckBox ckbBackup;
        internal CheckBox ckbServer;
        internal CheckBox ckbEmail;
        internal CheckBox ckbProxy;
        internal CheckBox ckbPassword;
        internal CheckBox ckbFullname;
        internal CheckBox ckbPhonenumber;
        internal CheckBox ckbUsername;
        internal CheckBox ckbTowFa;
        internal Bunifu.Framework.UI.BunifuCustomLabel DB37CC26;
        private Bunifu.Framework.UI.BunifuCustomLabel bunifuCustomLabel1;
        internal CheckBox ckbAvatar;
        internal CheckBox ckbPasswordEmail;
        internal CheckBox ckbDay;
        internal CheckBox ckbStatus;
    }
}