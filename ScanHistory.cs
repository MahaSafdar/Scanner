using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.Json;
using PortScanner.Scanners.Common;

namespace WinFormsApp2
{
    public partial class ScanHistory : Form
    {
        private ListView historyListView;
        private List<ScanHistoryItem> scanHistory;
        private bool isDisposed = false;
        public class ScanHistoryItem
        {
            public DateTime ScanDate { get; set; }
            public string ScanType { get; set; }
            public string Results { get; set; }
            public List<VulnerabilityInfo> Vulnerabilities { get; set; }
        }

        public ScanHistory()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += OnThemeChanged;

            InitializeSidePanel();
            InitializeHistoryListView();
            LoadScanHistory();

            // Apply initial theme
            ThemeManager.ApplyTheme(this);
            ApplyCustomThemeColors();

            NewScanPage.ScanCompleted += OnScanCompleted;
            this.FormClosing += Dashboard_FormClosing;
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

            // Apply to form
            this.BackColor = background;

            // Apply to panels
            if (Sidepanel != null)
            {
                Sidepanel.BackColor = primary;
                Sidepanel.ForeColor = primaryTextColor;
            }
            if (toppanel != null)
            {
                toppanel.BackColor = primary;
                toppanel.ForeColor = primaryTextColor;
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
            if (Apptitle != null)
                Apptitle.ForeColor = primaryTextColor;
            if (pageTitle != null)
                pageTitle.ForeColor = textColor;

            // Apply to ListView with theme-appropriate colors
            if (historyListView != null)
            {
                // Use a slightly darker/lighter version of the background color for the ListView
                Color listViewBackColor;
                if (IsColorDark(background))
                {
                    // For dark themes, make it slightly lighter
                    listViewBackColor = Color.FromArgb(
                        Math.Min(background.R + 20, 255),
                        Math.Min(background.G + 20, 255),
                        Math.Min(background.B + 20, 255)
                    );
                }
                else
                {
                    // For light themes, make it slightly darker
                    listViewBackColor = Color.FromArgb(
                        Math.Max(background.R - 20, 0),
                        Math.Max(background.G - 20, 0),
                        Math.Max(background.B - 20, 0)
                    );
                }

                historyListView.BackColor = listViewBackColor;
                historyListView.ForeColor = textColor;
            }
        }


        private void InitializeHistoryListView()
        {
            historyListView = new ListView();
            historyListView.Bounds = new Rectangle(200, 50, this.Width - 250, this.Height - 100);
            historyListView.View = View.Details;
            historyListView.FullRowSelect = true;
            historyListView.GridLines = true;


            historyListView.Columns.Add("Date & Time", 150);
            historyListView.Columns.Add("Scan Type", 120);
            historyListView.Columns.Add("Open Ports", 100);
            historyListView.Columns.Add("Vulnerabilities", 100);
            historyListView.Columns.Add("Status", 100);

            historyListView.DoubleClick += HistoryListView_DoubleClick;
            this.Controls.Add(historyListView);
        }
        private void LoadScanHistory()
        {
            try
            {
                string filePath = Path.Combine(Application.StartupPath, "scanhistory.json");
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    scanHistory = JsonSerializer.Deserialize<List<ScanHistoryItem>>(json);
                }
                else
                {
                    scanHistory = new List<ScanHistoryItem>();
                }

                UpdateHistoryDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading scan history: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                scanHistory = new List<ScanHistoryItem>();
            }
        }

        private void UpdateHistoryDisplay()
        {
            historyListView.Items.Clear();

            foreach (var scan in scanHistory.OrderByDescending(s => s.ScanDate))
            {
                var openPorts = CountOpenPorts(scan.Results);
                var vulnCount = scan.Vulnerabilities?.Count ?? 0;

                var item = new ListViewItem(new[]
                {
                    scan.ScanDate.ToString("g"),
                    scan.ScanType,
                    openPorts.ToString(),
                    vulnCount.ToString(),
                    "Completed"
                });

                if (vulnCount > 0)
                {
                    item.BackColor = Color.FromArgb(40, 0, 0);
                }

                historyListView.Items.Add(item);
            }
        }

        private int CountOpenPorts(string results)
        {
            if (string.IsNullOrEmpty(results)) return 0;

            return results.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                         .Count(line => line.Contains("Open") || line.Contains("open"));
        }

        private void OnScanCompleted(string scanResults, List<VulnerabilityInfo> vulns)
        {
            try
            {
                var newScan = new ScanHistoryItem
                {
                    ScanDate = DateTime.Now,
                    ScanType = DetermineScanType(scanResults),
                    Results = scanResults,
                    Vulnerabilities = vulns ?? new List<VulnerabilityInfo>()
                };

                scanHistory.Add(newScan);
                SaveScanHistory();

                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => UpdateHistoryDisplay()));
                }
                else
                {
                    UpdateHistoryDisplay();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving scan results: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string DetermineScanType(string results)
        {
            if (results.Contains("=== Port Scan Results ===")) return "Port Scan";
            if (results.Contains("=== ARP Scan Results ===")) return "ARP Scan";
            if (results.Contains("=== ICMP Scan Results ===")) return "ICMP Scan";
            if (results.Contains("ICMP Fragmentation")) return "ICMP Fragmentation Scan";
            return "Quick Scan";
        }

        private void SaveScanHistory()
        {
            try
            {
                string filePath = Path.Combine(Application.StartupPath, "scanhistory.json");
                string json = JsonSerializer.Serialize(scanHistory);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving scan history: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HistoryListView_DoubleClick(object sender, EventArgs e)
        {
            if (historyListView.SelectedItems.Count > 0)
            {
                int index = historyListView.SelectedItems[0].Index;
                var scan = scanHistory[scanHistory.Count - 1 - index];

                var detailsForm = new Form
                {
                    Text = $"Scan Details - {scan.ScanDate:g}",
                    Size = new Size(800, 600),
                    StartPosition = FormStartPosition.CenterParent,
                    BackColor = Color.FromArgb(20, 23, 34),
                    ForeColor = Color.White
                };

                var textBox = new TextBox
                {
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical,
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    Text = GetDetailedResults(scan),
                    BackColor = Color.FromArgb(30, 33, 44),
                    ForeColor = Color.White
                };

                detailsForm.Controls.Add(textBox);
                detailsForm.ShowDialog();
            }
        }

        private string GetDetailedResults(ScanHistoryItem scan)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Scan Type: {scan.ScanType}");
            sb.AppendLine($"Scan Date: {scan.ScanDate:g}");
            sb.AppendLine();
            sb.AppendLine("SCAN RESULTS");
            sb.AppendLine("============");
            sb.AppendLine(scan.Results);

            if (scan.Vulnerabilities?.Any() == true)
            {
                sb.AppendLine();
                sb.AppendLine("VULNERABILITIES");
                sb.AppendLine("==============");
                foreach (var vuln in scan.Vulnerabilities)
                {
                    sb.AppendLine($"Severity: {vuln.Severity}");
                    sb.AppendLine($"CVE: {vuln.CVE}");
                    sb.AppendLine($"Description: {vuln.Description}");
                    sb.AppendLine($"Recommendation: {vuln.Recommendation}");
                    sb.AppendLine();
                }
            }

            return sb.ToString();
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
            this.Hide();
            using (Dashboard dashboardForm = new Dashboard())
            {
                dashboardForm.ShowDialog();
            }
            this.Show();
        }

        private void NewScanBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (NewScanPage newScanPageForm = new NewScanPage())
            {
                newScanPageForm.ShowDialog();
            }
            this.Show();
        }

        private void ScanHistoryBtn_Click(object sender, EventArgs e)
        {
            // Already on scan history, do nothing
            return;
        }

        private void SettingsBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            using (Settings newSettingsForm = new Settings())
            {
                newSettingsForm.ShowDialog();
            }
            this.Show();
        }

        private void Dashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            isDisposed = true;
            ThemeManager.ThemeChanged -= OnThemeChanged;
            NewScanPage.ScanCompleted -= OnScanCompleted;
            Application.Exit();
        }

        private void label4_Click(object sender, EventArgs e)
        {
            // Handle if needed
        }

        private void ScanHistory_Load(object sender, EventArgs e)
        {
            // Additional load handling if needed
        }
    }
}