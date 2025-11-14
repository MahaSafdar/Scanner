namespace WinFormsApp2
{
    partial class Dashboard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dashboard));
            Pagetitle = new Label();
            Toppanel = new Panel();
            Apptitle = new Label();
            Applogo = new PictureBox();
            Dashboardbtn = new Button();
            Sidepanel = new Panel();
            NewScanBtn = new Button();
            SettingsBtn = new Button();
            ScanHistoryBtn = new Button();
            panel1 = new Panel();
            Vulnerabilities = new Label();
            Clock = new Label();
            Summaryp = new Panel();
            summarytitle = new Label();
            Welcome = new Label();
            Toppanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Applogo).BeginInit();
            Sidepanel.SuspendLayout();
            panel1.SuspendLayout();
            Summaryp.SuspendLayout();
            SuspendLayout();
            // 
            // Pagetitle
            // 
            Pagetitle.AutoSize = true;
            Pagetitle.Font = new Font("Verdana", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Pagetitle.ForeColor = SystemColors.ButtonHighlight;
            Pagetitle.Location = new Point(194, 9);
            Pagetitle.Name = "Pagetitle";
            Pagetitle.Size = new Size(126, 23);
            Pagetitle.TabIndex = 10;
            Pagetitle.Text = "DashBoard";
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
            Apptitle.Location = new Point(1, 7);
            Apptitle.Name = "Apptitle";
            Apptitle.Size = new Size(157, 25);
            Apptitle.TabIndex = 4;
            Apptitle.Text = "PRO Scanner";
            // 
            // Applogo
            // 
            Applogo.BackColor = Color.Transparent;
            Applogo.BackgroundImageLayout = ImageLayout.None;
            Applogo.Image = (Image)resources.GetObject("Applogo.Image");
            Applogo.InitialImage = (Image)resources.GetObject("Applogo.InitialImage");
            Applogo.Location = new Point(-3, 33);
            Applogo.Margin = new Padding(3, 2, 3, 2);
            Applogo.Name = "Applogo";
            Applogo.Size = new Size(176, 92);
            Applogo.SizeMode = PictureBoxSizeMode.Zoom;
            Applogo.TabIndex = 0;
            Applogo.TabStop = false;
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
            Sidepanel.Size = new Size(176, 409);
            Sidepanel.TabIndex = 9;
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
            // panel1
            // 
            panel1.Controls.Add(Vulnerabilities);
            panel1.Location = new Point(194, 92);
            panel1.Margin = new Padding(3, 2, 3, 2);
            panel1.Name = "panel1";
            panel1.Size = new Size(520, 158);
            panel1.TabIndex = 11;
            // 
            // Vulnerabilities
            // 
            Vulnerabilities.AutoSize = true;
            Vulnerabilities.Font = new Font("Verdana", 12F, FontStyle.Bold);
            Vulnerabilities.ForeColor = SystemColors.ControlLightLight;
            Vulnerabilities.Location = new Point(0, 0);
            Vulnerabilities.Name = "Vulnerabilities";
            Vulnerabilities.Size = new Size(165, 18);
            Vulnerabilities.TabIndex = 3;
            Vulnerabilities.Text = "Vulnerabilities List";
            // 
            // Clock
            // 
            Clock.AutoSize = true;
            Clock.Font = new Font("Verdana", 12F, FontStyle.Bold);
            Clock.ForeColor = SystemColors.ControlLightLight;
            Clock.Location = new Point(940, 15);
            Clock.Name = "Clock";
            Clock.Size = new Size(54, 18);
            Clock.TabIndex = 3;
            Clock.Text = "Clock";
            // 
            // Summaryp
            // 
            Summaryp.Controls.Add(summarytitle);
            Summaryp.Location = new Point(194, 272);
            Summaryp.Margin = new Padding(3, 2, 3, 2);
            Summaryp.Name = "Summaryp";
            Summaryp.Size = new Size(873, 112);
            Summaryp.TabIndex = 12;
            // 
            // summarytitle
            // 
            summarytitle.AutoSize = true;
            summarytitle.Font = new Font("Verdana", 12F, FontStyle.Bold);
            summarytitle.ForeColor = SystemColors.ControlLightLight;
            summarytitle.Location = new Point(0, 0);
            summarytitle.Name = "summarytitle";
            summarytitle.Size = new Size(138, 18);
            summarytitle.TabIndex = 3;
            summarytitle.Text = "Scan Summary";
            // 
            // Welcome
            // 
            Welcome.AutoSize = true;
            Welcome.Font = new Font("Verdana", 12F, FontStyle.Bold);
            Welcome.ForeColor = SystemColors.ControlLightLight;
            Welcome.Location = new Point(194, 58);
            Welcome.Name = "Welcome";
            Welcome.Size = new Size(140, 18);
            Welcome.TabIndex = 4;
            Welcome.Text = "Welcome! User";
            // 
            // Dashboard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(7, 17, 26);
            ClientSize = new Size(1078, 409);
            Controls.Add(Welcome);
            Controls.Add(Summaryp);
            Controls.Add(Clock);
            Controls.Add(panel1);
            Controls.Add(Pagetitle);
            Controls.Add(Sidepanel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            Name = "Dashboard";
            Text = "ProScanner";
            Load += Dashboard_Load;
            Toppanel.ResumeLayout(false);
            Toppanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)Applogo).EndInit();
            Sidepanel.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            Summaryp.ResumeLayout(false);
            Summaryp.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        // Add these event handler methods to your Dashboard.cs file
        private void Pagetitle_Click(object sender, EventArgs e)
        {
            // Add your click handling code here
        }

        private void Applogo_Click(object sender, EventArgs e)
        {
            // Add your click handling code here
        }

        #endregion

        private Label Pagetitle;
        private Panel Toppanel;
        private Label Apptitle;
        private PictureBox Applogo;
        private Button Dashboardbtn;
        private Panel Sidepanel;
        private Button NewScanBtn;
        private Button SettingsBtn;
        private Button ScanHistoryBtn;
        private Panel panel1;
        private Label Vulnerabilities;
        private Panel panel2;
        private Label Clock;
        private Panel Summaryp;
        private Label summarytitle;
        private Label Welcome;
        private Label Taskslabel;
    }
}