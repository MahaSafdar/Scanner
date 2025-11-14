namespace WinFormsApp2
{
    partial class ScanHistory
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScanHistory));
            pageTitle = new Label();
            toppanel = new Panel();
            Apptitle = new Label();
            Applogo = new PictureBox();
            Sidepanel = new Panel();
            NewScanBtn = new Button();
            SettingsBtn = new Button();
            ScanHistoryBtn = new Button();
            Dashboardbtn = new Button();
            toppanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Applogo).BeginInit();
            Sidepanel.SuspendLayout();
            SuspendLayout();
            // 
            // pageTitle
            // 
            pageTitle.AutoSize = true;
            pageTitle.Font = new Font("Verdana", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            pageTitle.ForeColor = SystemColors.ButtonHighlight;
            pageTitle.Location = new Point(233, 9);
            pageTitle.Name = "pageTitle";
            pageTitle.Size = new Size(174, 28);
            pageTitle.TabIndex = 6;
            pageTitle.Text = "Scan History";
            pageTitle.Click += label4_Click;
            // 
            // toppanel
            // 
            toppanel.BackColor = Color.FromArgb(9, 10, 14);
            toppanel.Controls.Add(Apptitle);
            toppanel.Controls.Add(Applogo);
            toppanel.Dock = DockStyle.Top;
            toppanel.Location = new Point(0, 0);
            toppanel.Name = "toppanel";
            toppanel.Size = new Size(201, 169);
            toppanel.TabIndex = 1;
            // 
            // Apptitle
            // 
            Apptitle.AutoSize = true;
            Apptitle.Font = new Font("Verdana", 15F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Apptitle.ForeColor = Color.White;
            Apptitle.Location = new Point(4, 6);
            Apptitle.Name = "Apptitle";
            Apptitle.Size = new Size(197, 31);
            Apptitle.TabIndex = 6;
            Apptitle.Text = "PRO Scanner";
            // 
            // Applogo
            // 
            Applogo.BackColor = Color.Transparent;
            Applogo.BackgroundImageLayout = ImageLayout.None;
            Applogo.Image = (Image)resources.GetObject("Applogo.Image");
            Applogo.InitialImage = (Image)resources.GetObject("Applogo.InitialImage");
            Applogo.Location = new Point(0, 41);
            Applogo.Name = "Applogo";
            Applogo.Size = new Size(201, 122);
            Applogo.SizeMode = PictureBoxSizeMode.Zoom;
            Applogo.TabIndex = 5;
            Applogo.TabStop = false;
            // 
            // Sidepanel
            // 
            Sidepanel.BackColor = Color.Black;
            Sidepanel.Controls.Add(NewScanBtn);
            Sidepanel.Controls.Add(SettingsBtn);
            Sidepanel.Controls.Add(ScanHistoryBtn);
            Sidepanel.Controls.Add(Dashboardbtn);
            Sidepanel.Controls.Add(toppanel);
            Sidepanel.Dock = DockStyle.Left;
            Sidepanel.Location = new Point(0, 0);
            Sidepanel.Name = "Sidepanel";
            Sidepanel.Size = new Size(201, 515);
            Sidepanel.TabIndex = 5;
            // 
            // NewScanBtn
            // 
            NewScanBtn.FlatStyle = FlatStyle.Popup;
            NewScanBtn.ForeColor = SystemColors.ControlLightLight;
            NewScanBtn.Image = (Image)resources.GetObject("NewScanBtn.Image");
            NewScanBtn.Location = new Point(-6, 267);
            NewScanBtn.Name = "NewScanBtn";
            NewScanBtn.RightToLeft = RightToLeft.No;
            NewScanBtn.Size = new Size(207, 43);
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
            SettingsBtn.Location = new Point(-14, 363);
            SettingsBtn.Name = "SettingsBtn";
            SettingsBtn.RightToLeft = RightToLeft.No;
            SettingsBtn.Size = new Size(215, 43);
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
            ScanHistoryBtn.Location = new Point(-14, 315);
            ScanHistoryBtn.Name = "ScanHistoryBtn";
            ScanHistoryBtn.RightToLeft = RightToLeft.No;
            ScanHistoryBtn.Size = new Size(215, 43);
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
            Dashboardbtn.Location = new Point(-3, 219);
            Dashboardbtn.Name = "Dashboardbtn";
            Dashboardbtn.RightToLeft = RightToLeft.No;
            Dashboardbtn.Size = new Size(203, 43);
            Dashboardbtn.TabIndex = 5;
            Dashboardbtn.Text = "DashBoard";
            Dashboardbtn.TextImageRelation = TextImageRelation.ImageBeforeText;
            Dashboardbtn.UseVisualStyleBackColor = true;
            // 
            // ScanHistory
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(7, 17, 26);
            ClientSize = new Size(917, 515);
            Controls.Add(pageTitle);
            Controls.Add(Sidepanel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "ScanHistory";
            Text = "ProScanner";
            Load += ScanHistory_Load;
            toppanel.ResumeLayout(false);
            toppanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)Applogo).EndInit();
            Sidepanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label pageTitle;
        private Panel toppanel;
        private Label label1;
        private Panel Sidepanel;
        private Button NewScanBtn;
        private Button SettingsBtn;
        private Button ScanHistoryBtn;
        private Button Dashboardbtn;
        private Label Apptitle;
        private PictureBox Applogo;
    }
}