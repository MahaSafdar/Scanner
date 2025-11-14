namespace WinFormsApp2
{
    partial class Settings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            Pagetitle = new Label();
            Toppanel = new Panel();
            Apptitle = new Label();
            Applogo = new PictureBox();
            NewScanBtn = new Button();
            SettingsBtn = new Button();
            ScanHistoryBtn = new Button();
            Dashboardbtn = new Button();
            Sidepanel = new Panel();
            label1 = new Label();
            Toppanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Applogo).BeginInit();
            Sidepanel.SuspendLayout();
            SuspendLayout();
            // 
            // Pagetitle
            // 
            Pagetitle.AutoSize = true;
            Pagetitle.FlatStyle = FlatStyle.Flat;
            Pagetitle.Font = new Font("Verdana", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Pagetitle.ForeColor = SystemColors.ButtonHighlight;
            Pagetitle.ImageAlign = ContentAlignment.MiddleLeft;
            Pagetitle.Location = new Point(196, 7);
            Pagetitle.Name = "Pagetitle";
            Pagetitle.Size = new Size(195, 23);
            Pagetitle.TabIndex = 8;
            Pagetitle.Text = "Scanner Settings";
            // 
            // Toppanel
            // 
            Toppanel.BackColor = Color.FromArgb(9, 10, 14);
            Toppanel.Controls.Add(Apptitle);
            Toppanel.Controls.Add(Applogo);
            Toppanel.Dock = DockStyle.Top;
            Toppanel.Location = new Point(0, 0);
            Toppanel.Margin = new Padding(3, 2, 3, 2);
            Toppanel.Name = "Toppanel";
            Toppanel.Size = new Size(176, 127);
            Toppanel.TabIndex = 1;
            // 
            // Apptitle
            // 
            Apptitle.AutoSize = true;
            Apptitle.Font = new Font("Verdana", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Apptitle.ForeColor = Color.White;
            Apptitle.Location = new Point(4, 4);
            Apptitle.Name = "Apptitle";
            Apptitle.Size = new Size(157, 25);
            Apptitle.TabIndex = 6;
            Apptitle.Text = "PRO Scanner";
            // 
            // Applogo
            // 
            Applogo.BackColor = Color.Transparent;
            Applogo.BackgroundImageLayout = ImageLayout.None;
            Applogo.Image = (Image)resources.GetObject("Applogo.Image");
            Applogo.InitialImage = (Image)resources.GetObject("Applogo.InitialImage");
            Applogo.Location = new Point(0, 31);
            Applogo.Margin = new Padding(3, 2, 3, 2);
            Applogo.Name = "Applogo";
            Applogo.Size = new Size(176, 92);
            Applogo.SizeMode = PictureBoxSizeMode.Zoom;
            Applogo.TabIndex = 5;
            Applogo.TabStop = false;
            // 
            // NewScanBtn
            // 
            NewScanBtn.FlatStyle = FlatStyle.Popup;
            NewScanBtn.ForeColor = SystemColors.ControlLightLight;
            NewScanBtn.Image = (Image)resources.GetObject("NewScanBtn.Image");
            NewScanBtn.Location = new Point(-5, 200);
            NewScanBtn.Margin = new Padding(3, 2, 3, 2);
            NewScanBtn.Name = "NewScanBtn";
            NewScanBtn.RightToLeft = RightToLeft.No;
            NewScanBtn.Size = new Size(181, 32);
            NewScanBtn.TabIndex = 6;
            NewScanBtn.Text = "New Scan";
            NewScanBtn.TextImageRelation = TextImageRelation.ImageBeforeText;
            NewScanBtn.UseVisualStyleBackColor = true;
            // 
            // SettingsBtn
            // 
            SettingsBtn.FlatStyle = FlatStyle.Popup;
            SettingsBtn.ForeColor = SystemColors.ControlLightLight;
            SettingsBtn.Image = (Image)resources.GetObject("SettingsBtn.Image");
            SettingsBtn.ImageAlign = ContentAlignment.TopCenter;
            SettingsBtn.Location = new Point(-12, 272);
            SettingsBtn.Margin = new Padding(3, 2, 3, 2);
            SettingsBtn.Name = "SettingsBtn";
            SettingsBtn.RightToLeft = RightToLeft.No;
            SettingsBtn.Size = new Size(188, 32);
            SettingsBtn.TabIndex = 6;
            SettingsBtn.Text = " Settings";
            SettingsBtn.TextImageRelation = TextImageRelation.ImageBeforeText;
            SettingsBtn.UseVisualStyleBackColor = true;
            // 
            // ScanHistoryBtn
            // 
            ScanHistoryBtn.FlatStyle = FlatStyle.Popup;
            ScanHistoryBtn.ForeColor = SystemColors.ControlLightLight;
            ScanHistoryBtn.Image = (Image)resources.GetObject("ScanHistoryBtn.Image");
            ScanHistoryBtn.Location = new Point(-12, 236);
            ScanHistoryBtn.Margin = new Padding(3, 2, 3, 2);
            ScanHistoryBtn.Name = "ScanHistoryBtn";
            ScanHistoryBtn.RightToLeft = RightToLeft.No;
            ScanHistoryBtn.Size = new Size(188, 32);
            ScanHistoryBtn.TabIndex = 6;
            ScanHistoryBtn.Text = "Scan History";
            ScanHistoryBtn.TextImageRelation = TextImageRelation.ImageBeforeText;
            ScanHistoryBtn.UseVisualStyleBackColor = true;
            // 
            // Dashboardbtn
            // 
            Dashboardbtn.FlatStyle = FlatStyle.Popup;
            Dashboardbtn.ForeColor = SystemColors.ControlLightLight;
            Dashboardbtn.Image = (Image)resources.GetObject("Dashboardbtn.Image");
            Dashboardbtn.ImageAlign = ContentAlignment.MiddleLeft;
            Dashboardbtn.Location = new Point(-3, 164);
            Dashboardbtn.Margin = new Padding(3, 2, 3, 2);
            Dashboardbtn.Name = "Dashboardbtn";
            Dashboardbtn.RightToLeft = RightToLeft.No;
            Dashboardbtn.Size = new Size(178, 32);
            Dashboardbtn.TabIndex = 5;
            Dashboardbtn.Text = "DashBoard";
            Dashboardbtn.TextImageRelation = TextImageRelation.ImageBeforeText;
            Dashboardbtn.UseVisualStyleBackColor = true;
            // 
            // Sidepanel
            // 
            Sidepanel.BackColor = Color.Black;
            Sidepanel.Controls.Add(NewScanBtn);
            Sidepanel.Controls.Add(SettingsBtn);
            Sidepanel.Controls.Add(ScanHistoryBtn);
            Sidepanel.Controls.Add(Dashboardbtn);
            Sidepanel.Controls.Add(Toppanel);
            Sidepanel.Dock = DockStyle.Left;
            Sidepanel.Location = new Point(0, 0);
            Sidepanel.Margin = new Padding(3, 2, 3, 2);
            Sidepanel.Name = "Sidepanel";
            Sidepanel.Size = new Size(176, 386);
            Sidepanel.TabIndex = 7;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ControlLightLight;
            label1.Location = new Point(196, 74);
            label1.Name = "label1";
            label1.Size = new Size(57, 20);
            label1.TabIndex = 9;
            label1.Text = "Theme:";
            // 
            // Settings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(7, 17, 26);
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(802, 386);
            Controls.Add(label1);
            Controls.Add(Pagetitle);
            Controls.Add(Sidepanel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            Name = "Settings";
            Text = "ProScanner";
            Load += Settings_Load;
            Toppanel.ResumeLayout(false);
            Toppanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)Applogo).EndInit();
            Sidepanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label Pagetitle;
        private Panel Toppanel;
        private Button NewScanBtn;
        private Button SettingsBtn;
        private Button ScanHistoryBtn;
        private Button Dashboardbtn;
        private Panel Sidepanel;
        private Label Apptitle;
        private PictureBox Applogo;
        private Label label1;
    }
}