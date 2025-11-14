using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using ProScanner.Database;
using System.Data;
using PortScanner.Scanners;
using PortScanner.Scanners.Common;
using System.Threading.Tasks;

namespace WinFormsApp2
{
    public partial class Dashboard : Form
    {
        private bool isDisposed = false;
        private System.Windows.Forms.Timer clockTimer;
        private ListView vulnListView;
        private ListView portListView;
        private Label scanSummaryLabel;
        private Label lastScanLabel;
        private int openPortsCount = 0;
        private List<PortInfo> openPorts = new List<PortInfo>();
        private List<VulnerabilityInfo> vulnerabilities = new List<VulnerabilityInfo>();
        private DatabaseManager dbManager = new DatabaseManager();

        public class PortInfo
        {
            public int Port { get; set; }
            public string Service { get; set; }
            public string Version { get; set; }
            public DateTime ScannedDate { get; set; }
        }

        public Dashboard()
        {
            InitializeComponent();

            // Subscribe to theme changes
            ThemeManager.ThemeChanged += OnThemeChanged;

            ConfigureFormAppearance();
            InitializeTimer();
            InitializeSidePanel();
            InitializePortListView();
            InitializeVulnerabilitiesListView();
            InitializeScanSummary();
            LoadLastScanResults();

            // Apply initial theme
            ThemeManager.ApplyTheme(this);
            ApplyCustomThemeColors();

            this.FormClosing += Dashboard_FormClosing;
            NewScanPage.ScanCompleted += OnScanCompleted;
        }


        private void ConfigureFormAppearance()
        {
            this.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
        }


        private void InitializeTimer()
        {
            clockTimer = new System.Windows.Forms.Timer();
            clockTimer.Interval = 1000;
            clockTimer.Tick += UpdateClock;
            clockTimer.Start();
        }

        private void UpdateClock(object sender, EventArgs e)
        {
            Clock.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }

        private void InitializePortListView()
        {
            portListView = new ListView();
            portListView.Bounds = new Rectangle(20, 280, panel1.Width - 40, 150);
            portListView.View = View.Details;
            portListView.FullRowSelect = true;
            portListView.GridLines = true;

            // Add header label
            Label portListHeader = new Label();
            portListHeader.Text = "Open Ports";
            portListHeader.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            portListHeader.AutoSize = true;
            portListHeader.Location = new Point(20, 255);
            panel1.Controls.Add(portListHeader);

            // Add columns
            portListView.Columns.Add("Port", 80);
            portListView.Columns.Add("State", 80);
            portListView.Columns.Add("Service", 150);
            portListView.Columns.Add("Version", 200);

            panel1.Controls.Add(portListView);
            portListView.DoubleClick += PortListView_DoubleClick;
        }

        private void InitializeVulnerabilitiesListView()
        {
            vulnListView = new ListView();
            vulnListView.View = View.Details;
            vulnListView.FullRowSelect = true;
            vulnListView.GridLines = true;
            vulnListView.Bounds = new Rectangle(10, 30, panel1.Width - 20, panel1.Height - 40);

            // Add columns
            vulnListView.Columns.Add("Severity", 80);
            vulnListView.Columns.Add("CVE", 100);
            vulnListView.Columns.Add("Description", 300);
            vulnListView.Columns.Add("Recommendation", 200);

            panel1.Controls.Add(vulnListView);
        }

        private void InitializeScanSummary()
        {

            lastScanLabel = new Label();
            lastScanLabel.Bounds = new Rectangle(20, 210, panel1.Width - 40, 40);
            lastScanLabel.Font = new Font("Segoe UI", 10F);
            lastScanLabel.ForeColor = Color.LightGray;
            lastScanLabel.Text = "Open Ports: 0\nLast Scan: Never";
            panel1.Controls.Add(lastScanLabel);
        }

        private async void LoadLastScanResults()
        {
            try
            {
                var lastScan = await dbManager.GetLastScanResultsAsync();
                if (lastScan != null)
                {
                    OnScanCompleted(lastScan.ScanResults, lastScan.Vulnerabilities);
                    lastScanLabel.Text = $"Open Ports: {openPortsCount}\nLast Scan: {lastScan.ScanDate:g}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading last scan results: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnScanCompleted(string scanResults, List<VulnerabilityInfo> vulns)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnScanCompleted(scanResults, vulns)));
                return;
            }

            vulnListView.Items.Clear();

            Color textColor = IsColorDark(this.BackColor) ? Color.White : Color.Black;

            foreach (var vuln in vulns)
            {
                var item = new ListViewItem(new[]
                {
            vuln.Severity,
            vuln.CVE,
            vuln.Description,
            vuln.Recommendation
        });

                // Color code based on severity and theme
                Color severityColor = vuln.Severity switch
                {
                    "Critical" => IsColorDark(this.BackColor) ? Color.FromArgb(80, 0, 0) : Color.FromArgb(255, 200, 200),
                    "High" => IsColorDark(this.BackColor) ? Color.FromArgb(80, 40, 0) : Color.FromArgb(255, 220, 180),
                    "Medium" => IsColorDark(this.BackColor) ? Color.FromArgb(80, 80, 0) : Color.FromArgb(255, 255, 180),
                    "Low" => IsColorDark(this.BackColor) ? Color.FromArgb(0, 80, 0) : Color.FromArgb(200, 255, 200),
                    _ => vulnListView.BackColor
                };

                item.BackColor = severityColor;
                item.ForeColor = textColor;

                vulnListView.Items.Add(item);
            }

            summarytitle.Text = $"Scan Summary - Found {vulns.Count} vulnerabilities";
        }

        private async void SaveScanResults(string scanResults, List<VulnerabilityInfo> vulns)
        {
            try
            {
                await dbManager.SaveScanResultsAsync(new ScanData
                {
                    ScanDate = DateTime.Now,
                    ScanResults = scanResults,
                    Vulnerabilities = vulns,
                    OpenPorts = openPorts
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving scan results: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PortListView_DoubleClick(object sender, EventArgs e)
        {
            if (portListView.SelectedItems.Count > 0)
            {
                var item = portListView.SelectedItems[0];
                var details = $"Port Details:\n\n" +
                            $"Port: {item.SubItems[0].Text}\n" +
                            $"State: {item.SubItems[1].Text}\n" +
                            $"Service: {item.SubItems[2].Text}\n" +
                            $"Version: {item.SubItems[3].Text}";

                MessageBox.Show(details, "Port Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void VulnListView_DoubleClick(object sender, EventArgs e)
        {
            if (vulnListView.SelectedItems.Count > 0)
            {
                var item = vulnListView.SelectedItems[0];
                var details = $"Vulnerability Details:\n\n" +
                            $"Severity: {item.SubItems[0].Text}\n" +
                            $"CVE: {item.SubItems[1].Text}\n" +
                            $"Description: {item.SubItems[2].Text}\n" +
                            $"Recommendation: {item.SubItems[3].Text}";

                MessageBox.Show(details, "Vulnerability Details",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void InitializeSidePanel()
        {
            Dashboardbtn.Click += Dashboardbtn_Click;
            NewScanBtn.Click += NewScanBtn_Click;
            ScanHistoryBtn.Click += ScanHistoryBtn_Click;
            SettingsBtn.Click += SettingsBtn_Click;
        }

        private void Dashboardbtn_Click(object sender, EventArgs e)
        {
            Dashboard dashboardForm = new Dashboard();
            this.Hide();
            dashboardForm.ShowDialog();
            this.Show();
        }

        private void NewScanBtn_Click(object sender, EventArgs e)
        {
            NewScanPage newScanPageForm = new NewScanPage();
            this.Hide();
            newScanPageForm.ShowDialog();
            this.Show();
        }
        
        private void ScanHistoryBtn_Click(object sender, EventArgs e)
        {
            ScanHistory scanHistoryForm = new ScanHistory();
            this.Hide();
            scanHistoryForm.ShowDialog();
            this.Show();
        }
        private void ApplyCustomThemeColors()
        {
            if (this.IsDisposed) return;

            // Get current theme colors
            Color primary = Color.Black;
            Color background = Color.FromArgb(7, 17, 26);

            switch (ThemeManager.GetCurrentTheme())
            {
                case "Emerald Essence":
                    primary = ThemeManager.ColorSchemes.EmeraldEssence.Primary;
                    background = ThemeManager.ColorSchemes.EmeraldEssence.Background;
                    break;
                case "Rustic Radiance":
                    primary = ThemeManager.ColorSchemes.RusticRadiance.Primary;
                    background = ThemeManager.ColorSchemes.RusticRadiance.Background;
                    break;
                case "Classic Elegance":
                    primary = ThemeManager.ColorSchemes.ClassicElegance.Primary;
                    background = ThemeManager.ColorSchemes.ClassicElegance.Background;
                    break;
                case "Midnight":
                    primary = ThemeManager.ColorSchemes.Midnight.Primary;
                    background = ThemeManager.ColorSchemes.Midnight.Background;
                    break;
            }

            // Determine text colors based on background brightness
            Color textColor = IsColorDark(background) ? Color.White : Color.Black;
            Color primaryTextColor = IsColorDark(primary) ? Color.White : Color.Black;

            // Calculate ListView background color
            Color listViewBackColor = IsColorDark(background)
                ? Color.FromArgb(
                    Math.Min(background.R + 20, 255),
                    Math.Min(background.G + 20, 255),
                    Math.Min(background.B + 20, 255))
                : Color.FromArgb(
                    Math.Max(background.R - 20, 0),
                    Math.Max(background.G - 20, 0),
                    Math.Max(background.B - 20, 0));

            // Apply to form
            this.BackColor = background;

            // Apply to panels and their children
            if (Sidepanel != null)
            {
                Sidepanel.BackColor = primary;
                Sidepanel.ForeColor = primaryTextColor;
            }
            if (Toppanel != null)
            {
                Toppanel.BackColor = primary;
                Toppanel.ForeColor = primaryTextColor;
            }
            if (panel1 != null)
            {
                panel1.BackColor = listViewBackColor;
            }
            if (Summaryp != null)
            {
                Summaryp.BackColor = listViewBackColor;
            }

            // Apply to navigation buttons
            if (Dashboardbtn != null)
            {
                Dashboardbtn.BackColor = primary;
                Dashboardbtn.ForeColor = primaryTextColor;
            }
            if (NewScanBtn != null)
            {
                NewScanBtn.BackColor = primary;
                NewScanBtn.ForeColor = primaryTextColor;
            }
            if (ScanHistoryBtn != null)
            {
                ScanHistoryBtn.BackColor = primary;
                ScanHistoryBtn.ForeColor = primaryTextColor;
            }
            if (SettingsBtn != null)
            {
                SettingsBtn.BackColor = primary;
                SettingsBtn.ForeColor = primaryTextColor;
            }

            // Apply to labels
            if (Apptitle != null) Apptitle.ForeColor = primaryTextColor;
            if (Pagetitle != null) Pagetitle.ForeColor = textColor;
            if (Clock != null) Clock.ForeColor = textColor;
            if (Welcome != null) Welcome.ForeColor = textColor;
            if (Vulnerabilities != null) Vulnerabilities.ForeColor = textColor;
            if (summarytitle != null) summarytitle.ForeColor = textColor;
            if (lastScanLabel != null) lastScanLabel.ForeColor = textColor;

            // Apply to ListViews
            if (portListView != null)
            {
                portListView.BackColor = listViewBackColor;
                portListView.ForeColor = textColor;
            }

            if (vulnListView != null)
            {
                vulnListView.BackColor = listViewBackColor;
                vulnListView.ForeColor = textColor;
            }
        }

        private void SettingsBtn_Click(object sender, EventArgs e)
        {
            Settings newSettingsForm = new Settings();
            this.Hide();
            newSettingsForm.ShowDialog();
            this.Show();
        }
        private void OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            if (!isDisposed)
            {
                ApplyCustomThemeColors();
            }
        }

        private bool IsColorDark(Color color)
        {
            double luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            return luminance < 0.5;
        }
        private void Dashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            isDisposed = true;
            ThemeManager.ThemeChanged -= OnThemeChanged;
            NewScanPage.ScanCompleted -= OnScanCompleted;

            if (e.CloseReason == CloseReason.UserClosing)
            {
                Environment.Exit(0);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Add any custom painting if needed
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {

        }
    }

    // Required supporting classes
    public class ScanData
    {
        public DateTime ScanDate { get; set; }
        public string ScanResults { get; set; }
        public List<VulnerabilityInfo> Vulnerabilities { get; set; }
        public List<Dashboard.PortInfo> OpenPorts { get; set; }
    }

    public class DatabaseManager
    {
        public async Task SaveScanResultsAsync(ScanData scanData)
        {
            // Implement database saving logic
            await Task.CompletedTask;
        }

        public async Task<ScanData> GetLastScanResultsAsync()
        {
            // Implement database retrieval logic
            await Task.CompletedTask;
            return null;
        }

    }

}