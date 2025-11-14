using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public partial class Settings : Form
    {
        private ComboBox colorSchemeComboBox;
        private bool isDisposed = false;
        public Settings()
        {
            InitializeComponent();
            ThemeManager.ThemeChanged += OnThemeChanged;

            InitializeSidePanel();
            InitializeComponents();

            // Apply initial theme
            ThemeManager.ApplyTheme(this);
            ApplyCustomThemeColors();

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
        private void InitializeComponents()
        {
            if (this.Controls.OfType<ComboBox>().FirstOrDefault(c => c.Name == "colorSchemeComboBox") == null)
            {
                colorSchemeComboBox = new ComboBox();
                colorSchemeComboBox.Name = "colorSchemeComboBox";
                colorSchemeComboBox.Items.AddRange(new object[] { "Emerald Essence", "Rustic Radiance", "Classic Elegance", "Midnight" });
                colorSchemeComboBox.SelectedIndexChanged += ColorSchemeComboBox_SelectedIndexChanged;
                colorSchemeComboBox.Location = new Point(310, 99);
                colorSchemeComboBox.Size = new Size(200, 30);
                colorSchemeComboBox.BackColor = Color.White;
                colorSchemeComboBox.ForeColor = Color.Black;
                this.Controls.Add(colorSchemeComboBox);

                // Set initial selection to match current theme
                colorSchemeComboBox.SelectedItem = ThemeManager.GetCurrentTheme();
            }
            else
            {
                colorSchemeComboBox = this.Controls.OfType<ComboBox>().FirstOrDefault(c => c.Name == "colorSchemeComboBox");
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
            if (Pagetitle != null)
                Pagetitle.ForeColor = textColor;
            if (label1 != null)
                label1.ForeColor = textColor;

            // Apply to ComboBox
            if (colorSchemeComboBox != null)
            {
                colorSchemeComboBox.BackColor = Color.White;
                colorSchemeComboBox.ForeColor = Color.Black;
            }
        }
        private void ColorSchemeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTheme = colorSchemeComboBox.SelectedItem.ToString();
            ThemeManager.ChangeTheme(selectedTheme);
            ApplyCustomThemeColors();
        }

        private void UpdateColorScheme(string colorSchemeName)
        {
            switch (colorSchemeName)
            {
                case "Emerald Essence":
                    UpdateEmeraldEssenceColors();
                    break;
                case "Rustic Radiance":
                    UpdateRusticRadianceColors();
                    break;
                case "Classic Elegance":
                    UpdateClassicEleganceColors();
                    break;
                case "Midnight":
                    UpdateMidnightColors();
                    break;
            }
        }

        private void UpdateEmeraldEssenceColors()
        {
            Color color = ColorTranslator.FromHtml("#0D2B1D");
            Color c = ColorTranslator.FromHtml("#E3EFD3");
            Color co = ColorTranslator.FromHtml("#AEC3B0");
            Dashboardbtn.BackColor = color;
            Dashboardbtn.ForeColor = c;
            NewScanBtn.BackColor = color;
            NewScanBtn.ForeColor = c;
            ScanHistoryBtn.BackColor = color;
            ScanHistoryBtn.ForeColor = c;
            SettingsBtn.BackColor = color;
            SettingsBtn.ForeColor = c;
            Sidepanel.BackColor = color;
            Toppanel.BackColor = color;
            Apptitle.ForeColor = c;
            label1.ForeColor = Color.Black;
            this.BackColor = co;
        }

        private void UpdateRusticRadianceColors()
        {
            Color color = ColorTranslator.FromHtml("#7E102C");
            Color c = ColorTranslator.FromHtml("#FFFFFF");
            Color co = ColorTranslator.FromHtml("#E1D4C1");
            Dashboardbtn.BackColor = color;
            Dashboardbtn.ForeColor = c;
            NewScanBtn.BackColor = color;
            NewScanBtn.ForeColor = c;
            ScanHistoryBtn.BackColor = color;
            ScanHistoryBtn.ForeColor = c;
            SettingsBtn.BackColor = color;
            SettingsBtn.ForeColor = c;
            Sidepanel.BackColor = color;
            label1.ForeColor = Color.Black;
            Toppanel.BackColor = color;
            Apptitle.ForeColor = c;
            this.BackColor = co;
        }

        private void UpdateMidnightColors()
        {
            Dashboardbtn.ForeColor = Color.Black;
            Dashboardbtn.ForeColor = Color.White;
            NewScanBtn.BackColor = Color.Black;
            NewScanBtn.ForeColor = Color.White;
            ScanHistoryBtn.BackColor = Color.Black;
            ScanHistoryBtn.ForeColor = Color.White;
            SettingsBtn.BackColor = Color.Black;
            SettingsBtn.ForeColor = Color.White;
            Sidepanel.BackColor = Color.Black;
            label1.ForeColor = Color.White;
            Toppanel.BackColor = Color.Black;
            Apptitle.ForeColor = Color.White;
            this.BackColor = Color.FromArgb(255, 7, 17, 26);
        }

        private void UpdateClassicEleganceColors()
        {
            Color color = ColorTranslator.FromHtml("#2E4053");
            Color c = ColorTranslator.FromHtml("#FFFFFF");
            Color co = ColorTranslator.FromHtml("#AAB7B8");
            Dashboardbtn.BackColor = color;
            Dashboardbtn.ForeColor = c;
            NewScanBtn.BackColor = color;
            NewScanBtn.ForeColor = c;
            ScanHistoryBtn.BackColor = color;
            ScanHistoryBtn.ForeColor = c;
            SettingsBtn.BackColor = color;
            SettingsBtn.ForeColor = c;
            label1.ForeColor = Color.Black;
            Sidepanel.BackColor = color;
            Toppanel.BackColor = color;
            Apptitle.ForeColor = c;
            this.BackColor = co;
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


        private void Dashboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            isDisposed = true;
            ThemeManager.ThemeChanged -= OnThemeChanged;
            Application.Exit();
        }


        private void Settings_Load(object sender, EventArgs e)
        {

        }
    }
}
