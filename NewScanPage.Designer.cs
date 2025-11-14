namespace WinFormsApp2
{
    partial class NewScanPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewScanPage));
            Title = new Label();
            toppanel = new Panel();
            Apptitle = new Label();
            Applogo = new PictureBox();
            sidepanel = new Panel();
            NewScanBtn = new Button();
            SettingsBtn = new Button();
            ScanHistoryBtn = new Button();
            Dashboardbtn = new Button();
            startscanbtn = new Button();
            ipRangeCheckBox = new CheckBox();
            ipRangeTextBox = new TextBox();
            ipRangeLabel = new Label();
            singleip = new Label();
            singleipbox = new TextBox();
            QuickScanoption = new RadioButton();
            ARPs = new RadioButton();
            ICMPF = new RadioButton();
            ICMPs = new RadioButton();
            PORT = new RadioButton();
            WKports = new CheckBox();
            CPort = new CheckBox();
            RPports = new CheckBox();
            Dports = new CheckBox();
            Aport = new CheckBox();
            SPort = new TextBox();
            EPort = new TextBox();
            toppanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Applogo).BeginInit();
            sidepanel.SuspendLayout();
            SuspendLayout();
            // 
            // Title
            // 
            Title.AutoSize = true;
            Title.Font = new Font("Verdana", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Title.ForeColor = SystemColors.ButtonHighlight;
            Title.Location = new Point(191, 7);
            Title.Name = "Title";
            Title.Size = new Size(117, 23);
            Title.TabIndex = 4;
            Title.Text = "New Scan";
            Title.UseWaitCursor = true;
            // 
            // toppanel
            // 
            toppanel.BackColor = Color.FromArgb(9, 10, 14);
            toppanel.Controls.Add(Apptitle);
            toppanel.Controls.Add(Applogo);
            toppanel.Dock = DockStyle.Top;
            toppanel.Location = new Point(0, 0);
            toppanel.Margin = new Padding(3, 2, 3, 2);
            toppanel.Name = "toppanel";
            toppanel.Size = new Size(176, 127);
            toppanel.TabIndex = 1;
            toppanel.UseWaitCursor = true;
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
            Apptitle.UseWaitCursor = true;
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
            Applogo.UseWaitCursor = true;
            // 
            // sidepanel
            // 
            sidepanel.BackColor = Color.Black;
            sidepanel.Controls.Add(NewScanBtn);
            sidepanel.Controls.Add(SettingsBtn);
            sidepanel.Controls.Add(ScanHistoryBtn);
            sidepanel.Controls.Add(Dashboardbtn);
            sidepanel.Controls.Add(toppanel);
            sidepanel.Dock = DockStyle.Left;
            sidepanel.Location = new Point(0, 0);
            sidepanel.Margin = new Padding(3, 2, 3, 2);
            sidepanel.Name = "sidepanel";
            sidepanel.Size = new Size(176, 421);
            sidepanel.TabIndex = 3;
            sidepanel.UseWaitCursor = true;
            // 
            // NewScanBtn
            // 
            NewScanBtn.BackgroundImageLayout = ImageLayout.None;
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
            NewScanBtn.UseWaitCursor = true;
            // 
            // SettingsBtn
            // 
            SettingsBtn.BackgroundImageLayout = ImageLayout.None;
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
            SettingsBtn.UseWaitCursor = true;
            // 
            // ScanHistoryBtn
            // 
            ScanHistoryBtn.BackgroundImageLayout = ImageLayout.None;
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
            ScanHistoryBtn.UseWaitCursor = true;
            // 
            // Dashboardbtn
            // 
            Dashboardbtn.BackgroundImageLayout = ImageLayout.None;
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
            Dashboardbtn.UseWaitCursor = true;
            Dashboardbtn.Click += Dashboardbtn_Click_1;
            // 
            // startscanbtn
            // 
            startscanbtn.FlatStyle = FlatStyle.Flat;
            startscanbtn.Font = new Font("Verdana", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            startscanbtn.ForeColor = SystemColors.ControlLightLight;
            startscanbtn.ImageAlign = ContentAlignment.TopCenter;
            startscanbtn.Location = new Point(191, 380);
            startscanbtn.Margin = new Padding(3, 2, 3, 2);
            startscanbtn.Name = "startscanbtn";
            startscanbtn.RightToLeft = RightToLeft.No;
            startscanbtn.Size = new Size(188, 32);
            startscanbtn.TabIndex = 8;
            startscanbtn.Text = "Start Scan";
            startscanbtn.TextImageRelation = TextImageRelation.ImageBeforeText;
            startscanbtn.UseVisualStyleBackColor = true;
            startscanbtn.UseWaitCursor = true;
            startscanbtn.Click += startscanbtn_Click;
            // 
            // ipRangeCheckBox
            // 
            ipRangeCheckBox.AutoSize = true;
            ipRangeCheckBox.ForeColor = SystemColors.ButtonHighlight;
            ipRangeCheckBox.Location = new Point(291, 49);
            ipRangeCheckBox.Margin = new Padding(3, 2, 3, 2);
            ipRangeCheckBox.Name = "ipRangeCheckBox";
            ipRangeCheckBox.Size = new Size(72, 19);
            ipRangeCheckBox.TabIndex = 9;
            ipRangeCheckBox.Text = "IP Range";
            ipRangeCheckBox.UseVisualStyleBackColor = true;
            ipRangeCheckBox.UseWaitCursor = true;
            ipRangeCheckBox.CheckedChanged += ipRangeCheckBox_CheckedChanged_1;
            // 
            // ipRangeTextBox
            // 
            ipRangeTextBox.Location = new Point(400, 70);
            ipRangeTextBox.Margin = new Padding(3, 2, 3, 2);
            ipRangeTextBox.Name = "ipRangeTextBox";
            ipRangeTextBox.Size = new Size(176, 23);
            ipRangeTextBox.TabIndex = 10;
            ipRangeTextBox.UseWaitCursor = true;
            ipRangeTextBox.Visible = false;
            // 
            // ipRangeLabel
            // 
            ipRangeLabel.AutoSize = true;
            ipRangeLabel.ForeColor = SystemColors.ButtonHighlight;
            ipRangeLabel.Location = new Point(399, 49);
            ipRangeLabel.Name = "ipRangeLabel";
            ipRangeLabel.Size = new Size(73, 15);
            ipRangeLabel.TabIndex = 11;
            ipRangeLabel.Text = "Enter End IP:";
            ipRangeLabel.UseWaitCursor = true;
            ipRangeLabel.Visible = false;
            // 
            // singleip
            // 
            singleip.AutoSize = true;
            singleip.Font = new Font("Segoe UI", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            singleip.ForeColor = SystemColors.ControlLightLight;
            singleip.Location = new Point(192, 51);
            singleip.Name = "singleip";
            singleip.Size = new Size(64, 19);
            singleip.TabIndex = 12;
            singleip.Text = "Enter IP:";
            singleip.UseWaitCursor = true;
            // 
            // singleipbox
            // 
            singleipbox.Location = new Point(192, 70);
            singleipbox.Margin = new Padding(3, 2, 3, 2);
            singleipbox.Name = "singleipbox";
            singleipbox.Size = new Size(176, 23);
            singleipbox.TabIndex = 13;
            singleipbox.UseWaitCursor = true;
            singleipbox.TextChanged += singleipbox_TextChanged;
            // 
            // QuickScanoption
            // 
            QuickScanoption.AutoSize = true;
            QuickScanoption.ForeColor = SystemColors.ButtonHighlight;
            QuickScanoption.Location = new Point(192, 100);
            QuickScanoption.Margin = new Padding(3, 2, 3, 2);
            QuickScanoption.Name = "QuickScanoption";
            QuickScanoption.Size = new Size(84, 19);
            QuickScanoption.TabIndex = 18;
            QuickScanoption.TabStop = true;
            QuickScanoption.Text = "Quick Scan";
            QuickScanoption.UseVisualStyleBackColor = true;
            QuickScanoption.UseWaitCursor = true;
            QuickScanoption.CheckedChanged += QuickScanoption_CheckedChanged;
            // 
            // ARPs
            // 
            ARPs.AutoSize = true;
            ARPs.ForeColor = SystemColors.ButtonHighlight;
            ARPs.Location = new Point(192, 123);
            ARPs.Margin = new Padding(3, 2, 3, 2);
            ARPs.Name = "ARPs";
            ARPs.Size = new Size(75, 19);
            ARPs.TabIndex = 19;
            ARPs.TabStop = true;
            ARPs.Text = "ARP Scan";
            ARPs.UseVisualStyleBackColor = true;
            ARPs.UseWaitCursor = true;
            // 
            // ICMPF
            // 
            ICMPF.AutoSize = true;
            ICMPF.ForeColor = SystemColors.ButtonHighlight;
            ICMPF.Location = new Point(378, 100);
            ICMPF.Margin = new Padding(3, 2, 3, 2);
            ICMPF.Name = "ICMPF";
            ICMPF.Size = new Size(161, 19);
            ICMPF.TabIndex = 21;
            ICMPF.TabStop = true;
            ICMPF.Text = "ICMP with Fragmentation";
            ICMPF.UseVisualStyleBackColor = true;
            ICMPF.UseWaitCursor = true;
            // 
            // ICMPs
            // 
            ICMPs.AutoSize = true;
            ICMPs.ForeColor = SystemColors.ButtonHighlight;
            ICMPs.Location = new Point(286, 100);
            ICMPs.Margin = new Padding(3, 2, 3, 2);
            ICMPs.Name = "ICMPs";
            ICMPs.Size = new Size(82, 19);
            ICMPs.TabIndex = 22;
            ICMPs.TabStop = true;
            ICMPs.Text = "ICMP Scan";
            ICMPs.UseVisualStyleBackColor = true;
            ICMPs.UseWaitCursor = true;
            // 
            // PORT
            // 
            PORT.AutoSize = true;
            PORT.ForeColor = SystemColors.ButtonHighlight;
            PORT.Location = new Point(286, 123);
            PORT.Margin = new Padding(3, 2, 3, 2);
            PORT.Name = "PORT";
            PORT.Size = new Size(75, 19);
            PORT.TabIndex = 23;
            PORT.TabStop = true;
            PORT.Text = "Port Scan";
            PORT.UseVisualStyleBackColor = true;
            PORT.UseWaitCursor = true;
            PORT.CheckedChanged += PORT_CheckedChanged;
            // 
            // WKports
            // 
            WKports.AutoSize = true;
            WKports.ForeColor = SystemColors.ButtonHighlight;
            WKports.Location = new Point(192, 154);
            WKports.Margin = new Padding(3, 2, 3, 2);
            WKports.Name = "WKports";
            WKports.Size = new Size(164, 19);
            WKports.TabIndex = 30;
            WKports.Text = "Well-Known Ports(1-1023)";
            WKports.UseVisualStyleBackColor = true;
            WKports.UseWaitCursor = true;
            WKports.Visible = false;
            // 
            // CPort
            // 
            CPort.AutoSize = true;
            CPort.ForeColor = SystemColors.ButtonHighlight;
            CPort.Location = new Point(192, 243);
            CPort.Margin = new Padding(3, 2, 3, 2);
            CPort.Name = "CPort";
            CPort.Size = new Size(104, 19);
            CPort.TabIndex = 31;
            CPort.Text = "Custom Range";
            CPort.UseVisualStyleBackColor = true;
            CPort.UseWaitCursor = true;
            CPort.Visible = false;
            CPort.CheckedChanged += CPort_CheckedChanged;
            // 
            // RPports
            // 
            RPports.AutoSize = true;
            RPports.ForeColor = SystemColors.ButtonHighlight;
            RPports.Location = new Point(192, 177);
            RPports.Margin = new Padding(3, 2, 3, 2);
            RPports.Name = "RPports";
            RPports.Size = new Size(178, 19);
            RPports.TabIndex = 32;
            RPports.Text = "Registered Ports(1024-49151)";
            RPports.UseVisualStyleBackColor = true;
            RPports.UseWaitCursor = true;
            RPports.Visible = false;
            // 
            // Dports
            // 
            Dports.AutoSize = true;
            Dports.ForeColor = SystemColors.ButtonHighlight;
            Dports.Location = new Point(192, 198);
            Dports.Margin = new Padding(3, 2, 3, 2);
            Dports.Name = "Dports";
            Dports.Size = new Size(176, 19);
            Dports.TabIndex = 33;
            Dports.Text = "Dynamic Ports(49152-65535)";
            Dports.UseVisualStyleBackColor = true;
            Dports.UseWaitCursor = true;
            Dports.Visible = false;
            // 
            // Aport
            // 
            Aport.AutoSize = true;
            Aport.ForeColor = SystemColors.ButtonHighlight;
            Aport.Location = new Point(192, 220);
            Aport.Margin = new Padding(3, 2, 3, 2);
            Aport.Name = "Aport";
            Aport.Size = new Size(119, 19);
            Aport.TabIndex = 34;
            Aport.Text = "All Ports(1-65535)";
            Aport.UseVisualStyleBackColor = true;
            Aport.UseWaitCursor = true;
            Aport.Visible = false;
            // 
            // SPort
            // 
            SPort.Location = new Point(191, 266);
            SPort.Margin = new Padding(3, 2, 3, 2);
            SPort.Name = "SPort";
            SPort.PlaceholderText = "Start";
            SPort.Size = new Size(50, 23);
            SPort.TabIndex = 36;
            SPort.UseWaitCursor = true;
            SPort.Visible = false;
            // 
            // EPort
            // 
            EPort.Location = new Point(253, 266);
            EPort.Margin = new Padding(3, 2, 3, 2);
            EPort.Name = "EPort";
            EPort.PlaceholderText = "End";
            EPort.Size = new Size(50, 23);
            EPort.TabIndex = 35;
            EPort.UseWaitCursor = true;
            EPort.Visible = false;
            // 
            // NewScanPage
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(7, 17, 26);
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(817, 421);
            Controls.Add(SPort);
            Controls.Add(EPort);
            Controls.Add(Aport);
            Controls.Add(Dports);
            Controls.Add(RPports);
            Controls.Add(CPort);
            Controls.Add(WKports);
            Controls.Add(PORT);
            Controls.Add(ICMPs);
            Controls.Add(ICMPF);
            Controls.Add(ARPs);
            Controls.Add(QuickScanoption);
            Controls.Add(singleipbox);
            Controls.Add(singleip);
            Controls.Add(ipRangeLabel);
            Controls.Add(ipRangeTextBox);
            Controls.Add(ipRangeCheckBox);
            Controls.Add(startscanbtn);
            Controls.Add(Title);
            Controls.Add(sidepanel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 2, 3, 2);
            MaximizeBox = false;
            Name = "NewScanPage";
            Text = "ProScanner";
            UseWaitCursor = true;
            Load += NewScanPage_Load;
            toppanel.ResumeLayout(false);
            toppanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)Applogo).EndInit();
            sidepanel.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label Title;
        private Panel toppanel;
        private Panel sidepanel;
        private Button NewScanBtn;
        private Button SettingsBtn;
        private Button ScanHistoryBtn;
        private Button Dashboardbtn;
        private Button startscanbtn;
        private CheckBox ipRangeCheckBox;
        private TextBox ipRangeTextBox;
        private Label ipRangeLabel;
        private Label singleip;
        private TextBox singleipbox;
        private RadioButton QuickScanoption;
        private RadioButton ARPs;
        private RadioButton ICMPF;
        private RadioButton ICMPs;
        private RadioButton PORT;
        private CheckBox WKports;
        private CheckBox CPort;
        private CheckBox RPports;
        private CheckBox Dports;
        private CheckBox Aport;
        private TextBox SPort;
        private TextBox EPort;
        private Label Apptitle;
        private PictureBox Applogo;
    }
}