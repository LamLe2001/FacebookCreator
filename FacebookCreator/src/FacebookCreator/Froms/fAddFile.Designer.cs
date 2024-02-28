namespace FacebookCreator.Froms
{
    partial class fAddFile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fAddFile));
            panel1 = new Panel();
            rtbData = new RichTextBox();
            btnClose = new Button();
            btnSave = new Button();
            lblTotal = new Label();
            label6 = new Label();
            panel2 = new Panel();
            btnExit = new Button();
            lblHeader = new Bunifu.Framework.UI.BunifuCustomLabel();
            lblError = new Label();
            label3 = new Label();
            lblSuccess = new Label();
            label1 = new Label();
            lbNotifi = new Label();
            ckbCatchAll = new CheckBox();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(rtbData);
            panel1.Location = new Point(1, 76);
            panel1.Name = "panel1";
            panel1.Size = new Size(553, 345);
            panel1.TabIndex = 0;
            // 
            // rtbData
            // 
            rtbData.Location = new Point(0, 0);
            rtbData.Name = "rtbData";
            rtbData.Size = new Size(553, 345);
            rtbData.TabIndex = 0;
            rtbData.Text = "";
            rtbData.TextChanged += rtbData_TextChanged;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Bottom;
            btnClose.BackColor = Color.Crimson;
            btnClose.Cursor = Cursors.Hand;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Tahoma", 8.25F, FontStyle.Bold, GraphicsUnit.Point);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(322, 443);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(107, 29);
            btnClose.TabIndex = 177;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSave.BackColor = Color.FromArgb(53, 120, 229);
            btnSave.Cursor = Cursors.Hand;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(144, 443);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(107, 29);
            btnSave.TabIndex = 176;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.ForeColor = Color.FromArgb(0, 192, 192);
            lblTotal.Location = new Point(536, 58);
            lblTotal.Margin = new Padding(4, 0, 4, 0);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(13, 15);
            lblTotal.TabIndex = 179;
            lblTotal.Text = "0";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(499, 58);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(35, 15);
            label6.TabIndex = 178;
            label6.Text = "Total:";
            // 
            // panel2
            // 
            panel2.Controls.Add(btnExit);
            panel2.Controls.Add(lblHeader);
            panel2.Location = new Point(1, 1);
            panel2.Name = "panel2";
            panel2.Size = new Size(553, 48);
            panel2.TabIndex = 180;
            // 
            // btnExit
            // 
            btnExit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExit.BackColor = Color.CornflowerBlue;
            btnExit.Cursor = Cursors.Hand;
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            btnExit.ForeColor = Color.White;
            btnExit.Image = (Image)resources.GetObject("btnExit.Image");
            btnExit.Location = new Point(514, 6);
            btnExit.Margin = new Padding(4, 3, 4, 3);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(35, 35);
            btnExit.TabIndex = 79;
            btnExit.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnExit.UseVisualStyleBackColor = false;
            btnExit.Click += btnExit_Click;
            // 
            // lblHeader
            // 
            lblHeader.BackColor = Color.CornflowerBlue;
            lblHeader.Cursor = Cursors.SizeAll;
            lblHeader.Dock = DockStyle.Fill;
            lblHeader.Font = new Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            lblHeader.ForeColor = Color.Transparent;
            lblHeader.Location = new Point(0, 0);
            lblHeader.Margin = new Padding(4, 0, 4, 0);
            lblHeader.Name = "lblHeader";
            lblHeader.Size = new Size(553, 48);
            lblHeader.TabIndex = 16;
            lblHeader.Text = "Import Data";
            lblHeader.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblError
            // 
            lblError.AutoSize = true;
            lblError.ForeColor = Color.Red;
            lblError.Location = new Point(470, 58);
            lblError.Margin = new Padding(4, 0, 4, 0);
            lblError.Name = "lblError";
            lblError.Size = new Size(13, 15);
            lblError.TabIndex = 195;
            lblError.Text = "0";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(434, 58);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(35, 15);
            label3.TabIndex = 194;
            label3.Text = "Error:";
            // 
            // lblSuccess
            // 
            lblSuccess.AutoSize = true;
            lblSuccess.ForeColor = Color.FromArgb(0, 192, 0);
            lblSuccess.Location = new Point(404, 58);
            lblSuccess.Margin = new Padding(4, 0, 4, 0);
            lblSuccess.Name = "lblSuccess";
            lblSuccess.Size = new Size(13, 15);
            lblSuccess.TabIndex = 193;
            lblSuccess.Text = "0";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(353, 58);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(51, 15);
            label1.TabIndex = 192;
            label1.Text = "Success:";
            // 
            // lbNotifi
            // 
            lbNotifi.AutoSize = true;
            lbNotifi.Location = new Point(6, 56);
            lbNotifi.Name = "lbNotifi";
            lbNotifi.Size = new Size(37, 15);
            lbNotifi.TabIndex = 196;
            lbNotifi.Text = "Notifi";
            // 
            // ckbCatchAll
            // 
            ckbCatchAll.AutoSize = true;
            ckbCatchAll.Location = new Point(6, 427);
            ckbCatchAll.Name = "ckbCatchAll";
            ckbCatchAll.Size = new Size(74, 19);
            ckbCatchAll.TabIndex = 198;
            ckbCatchAll.Text = "Catch-all";
            ckbCatchAll.UseVisualStyleBackColor = true;
            ckbCatchAll.Visible = false;
            // 
            // fAddFile
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(556, 484);
            Controls.Add(ckbCatchAll);
            Controls.Add(lbNotifi);
            Controls.Add(lblError);
            Controls.Add(label3);
            Controls.Add(lblSuccess);
            Controls.Add(label1);
            Controls.Add(panel2);
            Controls.Add(lblTotal);
            Controls.Add(label6);
            Controls.Add(btnClose);
            Controls.Add(btnSave);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "fAddFile";
            Text = "Telegram Creator";
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private RichTextBox rtbData;
        private Button btnClose;
        private Button btnSave;
        private Label lblTotal;
        private Label label6;
        private Panel panel2;
        private Button btnExit;
        private Label lblError;
        private Label label3;
        private Label lblSuccess;
        private Label label1;
        private Label lbNotifi;
        private Bunifu.Framework.UI.BunifuCustomLabel lblHeader;
        private CheckBox ckbCatchAll;
    }
}