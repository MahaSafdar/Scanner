using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using System.Linq;
using System.Data.SqlClient;
using System.Configuration;
using gradproject;
using System.Diagnostics;
using PortScanner;
using PortScanner.Scanners;
using Microsoft.Extensions.Logging;
using System.Text;
using gradproject.models;
using System.Net.Sockets;
using System.Security.Principal;
using System.Numerics;
using PortScanner.Scanners.Common;


namespace WinFormsApp2
{

    public partial class NewScanPage : Form

    {
        private readonly RegisteredPortHandler _portHandler;
        private bool isDisposed = false;
        public static event Action<string, List<VulnerabilityInfo>> ScanCompleted;
        private Label localIPLabel;
        private ProgressBar scanProgressBar;
        private Label progressLabel;
        private Label scanningLabel;
        private Label portScanWarningLabel;
        private System.Windows.Forms.Timer scanningLabelTimer;
        private Button cancelButton;
        private Label vulnerabilityScanLabel;
        private CheckBox yesVulnerabilityButton;
        private CheckBox noVulnerabilityButton;


        private void CreateVulnerabilityScanControls()
        {
            vulnerabilityScanLabel = new Label
            {
                Text = "Do you want to perform a vulnerability scan?",
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(428, 310),
                Font = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Italic),
                Visible = false
            };

            yesVulnerabilityButton = new CheckBox
            {
                Text = "Yes",
                ForeColor = Color.White,
                Location = new Point(428, 330),
                AutoSize = true,
                Visible = false
            };

            noVulnerabilityButton = new CheckBox
            {
                Text = "No",
                ForeColor = Color.White,
                Location = new Point(488, 330),
                AutoSize = true,
                Checked = true,
                Visible = false
            };

            this.Controls.Add(vulnerabilityScanLabel);
            this.Controls.Add(yesVulnerabilityButton);
            this.Controls.Add(noVulnerabilityButton);
        }


        private void CreateProgressControls()
        {

            // Progress Bar
            scanProgressBar = new ProgressBar();
            scanProgressBar.Location = new Point(428, 515);  // Adjusted position
            scanProgressBar.Size = new Size(200, 25);        // Smaller size
            scanProgressBar.Style = ProgressBarStyle.Continuous;
            scanProgressBar.ForeColor = Color.LightBlue;
            scanProgressBar.BackColor = Color.White;
            scanProgressBar.Minimum = 0;
            scanProgressBar.Maximum = 100;
            scanProgressBar.Value = 0;
            scanProgressBar.Visible = false;  // Hidden by default

            // Percentage Label
            progressLabel = new Label();
            progressLabel.Location = new Point(635, 518);  // Next to progress bar
            progressLabel.Size = new Size(50, 20);
            progressLabel.ForeColor = Color.White;
            progressLabel.Text = "0%";
            progressLabel.Visible = false;    // Hidden by default

            // Add new scanning label
            scanningLabel = new Label();
            scanningLabel.Location = new Point(428, 490);  // Above start scan button
            scanningLabel.AutoSize = true;
            scanningLabel.ForeColor = Color.White;
            scanningLabel.Font = new Font(scanningLabel.Font.FontFamily, scanningLabel.Font.Size, FontStyle.Italic);
            scanningLabel.Text = "Scanning...";
            scanningLabel.Visible = false;

            // Add port scan warning label
            portScanWarningLabel = new Label();
            portScanWarningLabel.Location = new Point(690, 518);  // Right of percentage label
            portScanWarningLabel.AutoSize = true;
            portScanWarningLabel.ForeColor = Color.White;
            portScanWarningLabel.Font = new Font(portScanWarningLabel.Font.FontFamily, portScanWarningLabel.Font.Size, FontStyle.Italic);
            portScanWarningLabel.Text = "Port scanning might take a while...";
            portScanWarningLabel.Visible = false;

            this.Controls.Add(scanProgressBar);
            this.Controls.Add(progressLabel);
            this.Controls.Add(scanningLabel);
            this.Controls.Add(portScanWarningLabel);

            scanningLabelTimer = new System.Windows.Forms.Timer();
            scanningLabelTimer.Interval = 500; // 0.5 seconds
            scanningLabelTimer.Tick += ScanningLabelTimer_Tick;
        }

        private void ScanningLabelTimer_Tick(object sender, EventArgs e)
        {
            scanningLabel.Visible = !scanningLabel.Visible;
        }
        public NewScanPage()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += OnThemeChanged;
            InitializeSidePanel();
            InitializeIPDisplay();
            CreateProgressControls();
            CreateCancelButton();
            CreateVulnerabilityScanControls();
            this.UseWaitCursor = false;
            ThemeManager.ApplyTheme(this);
            ApplyCustomThemeColors();
            SPort.KeyPress += SPort_KeyPress;
            EPort.KeyPress += EPort_KeyPress;
            _portHandler = new RegisteredPortHandler(AppDomain.CurrentDomain.BaseDirectory);

            // Hide range controls initially
            ipRangeTextBox.Visible = false;
            ipRangeLabel.Visible = false;




            this.FormClosing += Dashboard_FormClosing;
            this.Load += NewScanPage_Load;



        }
        private void NewScanPage_Load(object sender, EventArgs e)
        {
            InitializeSidePanel();
            InitializeIPDisplay();
            CreateProgressControls();
            CreateCancelButton();
            CreateVulnerabilityScanControls();  // If you've added this method

            // Initial control visibility
            ipRangeTextBox.Visible = false;
            ipRangeLabel.Visible = false;
            WKports.Visible = false;
            RPports.Visible = false;
            Dports.Visible = false;
            Aport.Visible = false;
            CPort.Visible = false;


            // Ensure proper button visibility
            startscanbtn.Visible = true;
            cancelButton.Visible = false;
        }
        private async Task<string> PerformCustomPortScan(string ipAddress, int startPort, int endPort)
        {
            var config = new ScannerConfiguration
            {
                MaxConcurrentScans = 1023,
                ConnectionTimeout = 1000,
                BatchSize = 10,
                RetryAttempts = 2,
                RetryDelay = 500
            };

            var scanner = new PortScannerImpl();
            var results = new StringBuilder();

            results.AppendLine($"=== Custom Port Scan Results ===");
            results.AppendLine($"IP Address: {ipAddress}");
            results.AppendLine($"Port Range: {startPort}-{endPort}");
            results.AppendLine("----------------------------------------\n");

            try
            {
                var scanResults = await scanner.ScanAsync(
                    ipAddress,
                    startPort,
                    endPort,
                    yesVulnerabilityButton.Checked
                );

                results.Append(scanResults);
            }
            catch (Exception ex)
            {
                results.AppendLine($"Error during scan: {ex.Message}");
            }

            return results.ToString();
        }


        private void InitializeIPDisplay()
        {
            // Create a label to show "Your IP:"
            Label ipTitleLabel = new Label
            {
                Text = "Your IP:",
                ForeColor = Color.White,  // Already white
                AutoSize = true,
                Location = new Point(500, 20),  // Adjusted position to be more visible
                Font = new Font("Segoe UI", 11, FontStyle.Bold)  // Made font bigger and bold
            };

            // Create a label to show the actual IP
            localIPLabel = new Label
            {
                Text = GetLocalIPAddress(),
                ForeColor = Color.White,  // Already white
                AutoSize = true,
                Location = new Point(600, 20),  // Adjusted to be next to the title
                Font = new Font("Segoe UI", 11)  // Made font bigger
            };

            // Add the labels to the form
            this.Controls.Add(ipTitleLabel);
            this.Controls.Add(localIPLabel);

            // Bring labels to front to ensure visibility
            ipTitleLabel.BringToFront();
            localIPLabel.BringToFront();
        }

        private string GetLocalIPAddress()
        {
            try
            {
                // Get all IP addresses of the local machine
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

                // Get the first IPv4 address
                var ipAddress = host.AddressList
                    .FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

                return ipAddress?.ToString() ?? "IP not found";
            }
            catch (Exception)
            {
                return "Unable to determine IP";
            }
        }

        private void Dashboardbtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            Dashboard dashboardForm = new Dashboard();
            dashboardForm.ShowDialog();
        }

        private void NewScanBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            NewScanPage newScanPageForm = new NewScanPage();
            newScanPageForm.ShowDialog();
        }

        private void ScanHistoryBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            ScanHistory newScanHistoryform = new ScanHistory();
            newScanHistoryform.ShowDialog();
        }

        private void SettingsBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            Settings newSettingsform = new Settings();
            newSettingsform.ShowDialog();
        }

        private void InitializeSidePanel()
        {
            Dashboardbtn.Click += Dashboardbtn_Click;
            NewScanBtn.Click += NewScanBtn_Click;
            ScanHistoryBtn.Click += ScanHistoryBtn_Click;
            SettingsBtn.Click += SettingsBtn_Click;
        }
        private void OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            if (!isDisposed)
            {
                ApplyCustomThemeColors();
            }
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
            if (sidepanel != null)
            {
                sidepanel.BackColor = primary;
                sidepanel.ForeColor = primaryTextColor;
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

            // Apply other form-specific controls
            // For NewScanPage:
            if (Title != null) Title.ForeColor = textColor;
            if (singleip != null) singleip.ForeColor = textColor;
            if (ipRangeLabel != null) ipRangeLabel.ForeColor = textColor;

            // Apply to all radio buttons and checkboxes
            foreach (Control control in this.Controls)
            {
                if (control is RadioButton radioButton)
                {
                    radioButton.ForeColor = textColor;
                }
                if (control is CheckBox checkbox)
                {
                    checkbox.ForeColor = textColor;
                }
            }

            // Apply to buttons
            if (startscanbtn != null)
            {
                startscanbtn.BackColor = primary;
                startscanbtn.ForeColor = primaryTextColor;
            }
            if (cancelButton != null)
            {
                cancelButton.BackColor = primary;
                cancelButton.ForeColor = primaryTextColor;
            }
        }

        // Add this helper method to your form class
        private bool IsColorDark(Color color)
        {
            double luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            return luminance < 0.5;
        }

        private void Dashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            isDisposed = true;
            ThemeManager.ThemeChanged -= OnThemeChanged;
            Application.Exit();
        }

        private void ipRangeCheckBox_CheckedChanged_1(object sender, EventArgs e)
        {
            ipRangeTextBox.Visible = ipRangeCheckBox.Checked;
            ipRangeLabel.Visible = ipRangeCheckBox.Checked;
        }

        // Add this method to handle port scan option visibility
        private void HandlePortScanOptions(bool visible)
        {
            WKports.Visible = visible;
            RPports.Visible = visible;
            CPort.Visible = visible;
            Aport.Visible = visible;
            Dports.Visible = visible;
            yesVulnerabilityButton.Visible = visible;
            noVulnerabilityButton.Visible = visible;
        }

        private (IPAddress?, IPAddress?) GetIPRange()
        {
            try
            {
                // First validate that singleipbox has a value
                if (string.IsNullOrWhiteSpace(singleipbox.Text))
                {
                    MessageBox.Show("Please enter a start IP address.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return (null, null);
                }

                // Try to parse start IP
                if (!IPAddress.TryParse(singleipbox.Text.Trim(), out IPAddress? startIP))
                {
                    MessageBox.Show("Invalid start IP address format.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return (null, null);
                }

                // If range is checked, validate end IP
                if (ipRangeCheckBox.Checked)
                {
                    if (string.IsNullOrWhiteSpace(ipRangeTextBox.Text))
                    {
                        MessageBox.Show("Please enter an end IP address.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return (null, null);
                    }

                    if (!IPAddress.TryParse(ipRangeTextBox.Text.Trim(), out IPAddress? endIP))
                    {
                        MessageBox.Show("Invalid end IP address format.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return (null, null);
                    }

                    return (startIP, endIP);
                }

                // If no range, return same IP for both
                return (startIP, startIP);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing IP addresses: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return (null, null);
            }
        }

        private void QuickScanoption_CheckedChanged(object sender, EventArgs e)
        {
            if (QuickScanoption.Checked)
            {

                HandlePortScanOptions(false);


            }
        }





        private async void startscanbtn_Click(object sender, EventArgs e)
        {
            DisableUserInteraction();
            startscanbtn.Visible = false;
            cancelButton.Visible = true;

            scanProgressBar.Value = 0;
            scanProgressBar.Visible = true;
            progressLabel.Visible = true;
            scanningLabel.Visible = true;

            string scanType = GetSelectedScanType();

            if (PORT.Checked)
            {
                await Task.Delay(5000);
                portScanWarningLabel.Visible = true;
            }
            await Task.Delay(5000);

            var progress = new Progress<string>(status =>
            {
                try
                {
                    if (status.StartsWith("Progress:") && scanType != "Port Scan")
                    {
                        string percentStr = status.Split('|')[0].Replace("Progress:", "").Replace("%", "").Trim();
                        if (int.TryParse(percentStr, out int percent))
                        {
                            this.Invoke(() =>
                            {
                                Console.WriteLine($"Progress update: {percent}%"); // Debug line
                                scanProgressBar.Value = percent;
                                progressLabel.Text = $"{percent}%";
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Progress error: {ex.Message}"); // Debug line
                }
            });

            (IPAddress startIP, IPAddress endIP) = GetIPRange();
            if (startIP == null || endIP == null)
            {
                MessageBox.Show("Invalid IP range specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DateTime startTime = DateTime.Now;
            string results = "";

            if (scanType == "Quick Scan")
            {
                var localIP = IPAddress.Parse(localIPLabel.Text);

                results = "=== ARP Scan Results ===\n";
                results += await ARP.PerformARPScan(startIP, endIP, progress);
                results += "\n\n\n";

                results += "=== ICMP Scan Results ===\n";
                results += await ICMP.PerformICMPScan(startIP, endIP, progress);
                results += "\n\n\n";

                var config = new ScannerConfiguration
                {
                    MaxConcurrentScans = 25,
                    ConnectionTimeout = 1000,
                    BatchSize = 10,
                    RetryAttempts = 1,
                    RetryDelay = 100
                };

                var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
                var logger = loggerFactory.CreateLogger<PortScannerc>();
                var serviceProber = new ServiceProber(
                    probeFilePath: "nmap-service-probes",
                    servicesFilePath: "nmap-services",
                    enableDebug: false,
                    logger: logger,
                    capturedPackets: null
                );

                using var portScanner = new PortScannerc(config, logger);
                var portScanResults = await portScanner.ScanAsync(
                    localIP.ToString(),
                    Enumerable.Range(1, 1023),
                    serviceProber,
                    progress,
                    CancellationToken.None
                );

                foreach (var result in portScanResults.Where(r => r.IsOpen))
                {
                    results += "=== Port Scan Results (Local Machine) ===\r\n";
                    results += $"Port {result.Port}: Open \r\n";
                    results += $"Service: {result.ServiceName} \r\n";
                    if (!string.IsNullOrEmpty(result.ServiceVersion))
                        results += $"Version: {result.ServiceVersion} \r\n";
                    results += "\r\n";
                    results += "-------------------\r\n";
                }
            }
            else if (scanType == "ARP Scan")
            {
                results = await ARP.PerformARPScan(startIP, endIP, progress);
            }
            else if (scanType == ICMPF.Text)
            {
                if (startscanbtn.Visible) return;
                results = await ICMPFragmentation.PerformFragmentedICMPScan(startIP, endIP, progress);
            }
            else if (scanType == ICMPs.Text)
            {
                if (startscanbtn.Visible) return;
                results = await ICMP.PerformICMPScan(startIP, endIP, progress);
            }

            var scanner = new PortScannerImpl();
            if (scanType == "Port Scan" || scanType.StartsWith("Well-Known Ports") ||
                scanType.StartsWith("Registered Ports") || scanType.StartsWith("Dynamic Ports") ||
                scanType.StartsWith("All Ports") || scanType.StartsWith("Custom Range"))
            {
                int startPort = 1, endPort = 65535;

                var config = new ScannerConfiguration
                {
                    MaxConcurrentScans = 1023,
                    ConnectionTimeout = 1000,
                    BatchSize = 10,
                    RetryAttempts = 2,
                    RetryDelay = 500
                };

                if (scanType.StartsWith("Well-Known Ports"))
                {
                    startPort = 1;
                    endPort = 1023;
                }
                else if (scanType.StartsWith("Registered Ports"))
                {
                    startPort = 1024;
                    endPort = 49151;
                }
                else if (scanType.StartsWith("Dynamic Ports"))
                {
                    startPort = 49152;
                    endPort = 65535;
                }
                else if (scanType.StartsWith("Custom Range"))
                {
                    if (int.TryParse(SPort.Text, out startPort) && int.TryParse(EPort.Text, out endPort))
                    {
                        if (startPort <= 0 || startPort > 65535 || endPort < startPort || endPort > 65535)
                        {
                            MessageBox.Show("Invalid port range. Start port must be between 1-65535 and end port must be >= start port.",
                                "Invalid Port Range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please enter valid port numbers.",
                            "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                results = await scanner.ScanAsync(
                    startIP.ToString(),
                    startPort,
                    endPort,
                    yesVulnerabilityButton.Checked
                );
            }

            DateTime endTime = DateTime.Now;
            TimeSpan scanDuration = endTime - startTime;
            ShowScanResults(scanType, startTime, scanDuration, results);

            scanProgressBar.Value = 100;
            progressLabel.Text = "100%";
            await Task.Delay(100);
            scanProgressBar.Visible = false;
            progressLabel.Visible = false;
            scanningLabel.Visible = false;
            portScanWarningLabel.Visible = false;

            scanningLabelTimer.Start();
            scanningLabelTimer.Stop();
            scanningLabel.Visible = false;
            portScanWarningLabel.Visible = false;

            string scanResults = results;
            List<VulnerabilityInfo> vulnerabilities = new List<VulnerabilityInfo>();

            var actualScanType = GetSelectedScanType();
            ScanCompleted?.Invoke(results, new List<VulnerabilityInfo>());
        }   

        private IEnumerable<int> GetPortRange()
        {
            if (WKports.Checked)
                return Enumerable.Range(1, 1023);
            else if (RPports.Checked)
                return Enumerable.Range(1024, 49151 - 1024 + 1);
            else if (Aport.Checked)
                return Enumerable.Range(1, 65535);
            else if (Dports.Checked)
                return Enumerable.Range(49152, 65535 - 49152 + 1);
            else if (CPort.Checked && !string.IsNullOrWhiteSpace(SPort.Text) && !string.IsNullOrWhiteSpace(EPort.Text))
            {
                if (int.TryParse(SPort.Text, out int startPort) && int.TryParse(EPort.Text, out int endPort))
                {
                    if (startPort > 0 && startPort <= 65535 && endPort >= startPort && endPort <= 65535)
                    {
                        return Enumerable.Range(startPort, endPort - startPort + 1);
                    }
                    MessageBox.Show("Port range must be between 1-65535 with start port less than or equal to end port.",
                        "Invalid Port Range", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Please enter valid port numbers.",
                        "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            return Enumerable.Empty<int>();
        }

        private string GetSelectedScanType()
        {
            if (PORT.Checked)
            {
                if (WKports.Checked) return "Well-Known Ports";
                if (RPports.Checked) return "Registered Ports";
                if (Dports.Checked) return "Dynamic Ports";
                if (Aport.Checked) return "All Ports";
                if (CPort.Checked) return "Custom Range";
                return "Port Scan";
            }
            if (ARPs.Checked) return "ARP Scan";
            if (ICMPF.Checked) return "ICMP Fragmentation Scan";
            if (ICMPs.Checked) return "ICMP Scan";
            return "Quick Scan";
        }

        private void PORT_CheckedChanged(object sender, EventArgs e)
        {
            HandlePortScanOptions(PORT.Checked);
            ipRangeCheckBox.Visible = false;
            ipRangeTextBox.Visible = false;
            ipRangeLabel.Visible = false;
            WKports.Visible = true;
            RPports.Visible = true;
            Dports.Visible = true;
            Aport.Visible = true;
            CPort.Visible = true;
            yesVulnerabilityButton.Visible = true;
            noVulnerabilityButton.Visible = true;

        }

        private void ShowScanResults(string scanType, DateTime startTime, TimeSpan scanDuration, String results)
        {

            foreach (Control control in this.Controls)
            {
                control.Visible = false;
                scanningLabel.Visible = false;
                portScanWarningLabel.Visible = false;
            }


            Panel resultsPanel = new Panel
            {
                BackColor = Color.White,
                Size = new Size(this.Width - 40, this.Height - 40),
                Location = new Point(20, 20),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Add the panel to the form
            this.Controls.Add(resultsPanel);

            // Create labels to display scan information
            Label scanTypeLabel = new Label
            {
                Text = $"Scan Type: {scanType}",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            Label dateLabel = new Label
            {
                Text = $"Date & Time: {startTime}",
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 60),
                AutoSize = true
            };

            Label durationLabel = new Label
            {
                Text = $"Duration: {scanDuration.TotalMinutes} Approximately",
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 100),
                AutoSize = true
            };
            TextBox resultsTextBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("arial", 12),

                Location = new Point(20, 140),
                Size = new Size(resultsPanel.Width - 60, resultsPanel.Height - 220),
                MinimumSize = new Size(100, 100),
                Visible = true,
                Text = results ?? "No results available"
            };
            resultsPanel.Controls.Add(resultsTextBox);

            resultsPanel.Controls.Add(scanTypeLabel);
            resultsPanel.Controls.Add(dateLabel);
            resultsPanel.Controls.Add(durationLabel);

            Button closeButton = new Button
            {
                Text = "Close",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, resultsPanel.Height - 70),
                AutoSize = true
            };

            closeButton.Click += (s, args) =>
            {
                RestoreUserInteraction();
                this.Controls.Remove(resultsPanel);
                resultsPanel.Dispose();


                foreach (Control control in this.Controls)
                {
                    control.Visible = true;
                    if (PORT.Checked)
                    {

                        WKports.Visible = PORT.Checked;
                        RPports.Visible = PORT.Checked;
                        Dports.Visible = PORT.Checked;
                        Aport.Visible = PORT.Checked;
                        CPort.Visible = PORT.Checked;
                        ipRangeCheckBox.Checked = false;
                        ipRangeTextBox.Visible = false;
                        ipRangeCheckBox.Visible = false;
                        portScanWarningLabel.Visible = false;
                        yesVulnerabilityButton.Visible = false;
                        noVulnerabilityButton.Visible = false;
                        vulnerabilityScanLabel.Visible = false;
                        scanningLabel.Visible = false;
                        progressLabel.Visible = false;
                        scanProgressBar.Visible = false;
                    }

                    else
                        WKports.Visible = PORT.Checked;
                    RPports.Visible = PORT.Checked;
                    Dports.Visible = PORT.Checked;
                    Aport.Visible = PORT.Checked;
                    CPort.Visible = PORT.Checked;
                }

                scanProgressBar.Value = 0;
                progressLabel.Text = "0%";
                scanProgressBar.Visible = false;
                progressLabel.Visible = false;
                scanningLabel.Visible = false;
                portScanWarningLabel.Visible = false;
                noVulnerabilityButton.Visible = false;
                yesVulnerabilityButton.Visible = false;
                vulnerabilityScanLabel.Visible = false;



                ipRangeCheckBox_CheckedChanged_1(ipRangeCheckBox, EventArgs.Empty);
            };

            resultsPanel.Controls.Add(closeButton);
        }


        private void DisableUserInteraction()
        {

            foreach (Control control in this.Controls)
            {



                if (control != cancelButton)
                {
                    control.Enabled = false;
                    portScanWarningLabel.Enabled = true;
                    scanningLabel.Enabled = true;
                    progressLabel.Enabled = true;
                }


            }




        }

        private void RestoreUserInteraction()
        {

            foreach (Control control in this.Controls)
            {
                control.Enabled = true;
            }
            portScanWarningLabel.Visible = false;
            this.Enabled = true;


        }

        private void CreateCancelButton()
        {
            cancelButton = new Button
            {
                Text = "Cancel Scan",
                Location = new Point(startscanbtn.Location.X, startscanbtn.Location.Y),

                UseVisualStyleBackColor = true,
                Size = startscanbtn.Size,
                Visible = true
            };

            cancelButton.Click += CancelButton_Click;
            this.Controls.Add(cancelButton);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {

            CancelScan();



        }

        private void CancelScan()
        {
            RestoreUserInteraction();


            scanProgressBar.Value = 0;
            scanProgressBar.Visible = false;
            progressLabel.Visible = false;
            scanningLabel.Visible = false;
            portScanWarningLabel.Visible = false;
            scanningLabelTimer.Stop();

            startscanbtn.Visible = true;
            cancelButton.Visible = false;



        }

        private void CPort_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = CPort.Checked;
            SPort.Visible = isChecked;
            EPort.Visible = isChecked;
            yesVulnerabilityButton.Visible = isChecked;
            noVulnerabilityButton.Visible = isChecked;
            vulnerabilityScanLabel.Visible = isChecked;

            if (isChecked)
            {
                // Set default values or clear
                SPort.Clear();
                EPort.Clear();
                SPort.PlaceholderText = "Start (1-65535)";
                EPort.PlaceholderText = "End (1-65535)";
            }
        }
        private bool ValidatePortRange(out int startPort, out int endPort)
        {
            startPort = 0;
            endPort = 0;

            if (string.IsNullOrWhiteSpace(SPort.Text) || string.IsNullOrWhiteSpace(EPort.Text))
            {
                MessageBox.Show("Please enter both start and end ports.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(SPort.Text, out startPort) || !int.TryParse(EPort.Text, out endPort))
            {
                MessageBox.Show("Please enter valid numbers for port range.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (startPort < 1 || startPort > 65535 || endPort < 1 || endPort > 65535)
            {
                MessageBox.Show("Ports must be between 1 and 65535.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (startPort > endPort)
            {
                MessageBox.Show("Start port must be less than or equal to end port.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
        private void SPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void EPort_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void singleipbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void Dashboardbtn_Click_1(object sender, EventArgs e)
        {

        }
    }
}