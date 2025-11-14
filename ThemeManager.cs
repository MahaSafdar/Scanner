using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp2
{
    public static class ThemeManager
    {
        // Current theme name
        private static string currentTheme = "Midnight"; // Default theme

        // Theme changed event
        public static event EventHandler<ThemeChangedEventArgs> ThemeChanged;

        // Color schemes
        public static class ColorSchemes
        {
            public static class EmeraldEssence
            {
                public static Color Primary = ColorTranslator.FromHtml("#0D2B1D");
                public static Color Background = ColorTranslator.FromHtml("#AEC3B0");
            }

            public static class RusticRadiance
            {
                public static Color Primary = ColorTranslator.FromHtml("#7E102C");
                public static Color Background = ColorTranslator.FromHtml("#E1D4C1");
            }

            public static class ClassicElegance
            {
                public static Color Primary = ColorTranslator.FromHtml("#2E4053");
                public static Color Background = ColorTranslator.FromHtml("#AAB7B8");
            }

            public static class Midnight
            {
                public static Color Primary = Color.Black;
                public static Color Background = Color.FromArgb(255, 7, 17, 26);
            }
        }

        private static bool IsColorDark(Color color)
        {
            // Calculate relative luminance
            double luminance = (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255;
            return luminance < 0.5;
        }

        // Apply theme to form
        public static void ApplyTheme(Form form)
        {
            if (form == null) return;

            // Get common controls that exist across forms
            var sidepanel = form.Controls.Find("sidepanel", true).FirstOrDefault() as Panel;
            var toppanel = form.Controls.Find("toppanel", true).FirstOrDefault() as Panel;
            var dashboardBtn = form.Controls.Find("Dashboardbtn", true).FirstOrDefault() as Button;
            var newScanBtn = form.Controls.Find("NewScanBtn", true).FirstOrDefault() as Button;
            var scanHistoryBtn = form.Controls.Find("ScanHistoryBtn", true).FirstOrDefault() as Button;
            var settingsBtn = form.Controls.Find("SettingsBtn", true).FirstOrDefault() as Button;
            var appTitle = form.Controls.Find("Apptitle", true).FirstOrDefault() as Label;
            var title = form.Controls.Find("Title", true).FirstOrDefault() as Label;

            // Apply current theme
            switch (currentTheme)
            {
                case "Emerald Essence":
                    ApplyColors(form, ColorSchemes.EmeraldEssence.Primary, ColorSchemes.EmeraldEssence.Background);
                    break;

                case "Rustic Radiance":
                    ApplyColors(form, ColorSchemes.RusticRadiance.Primary, ColorSchemes.RusticRadiance.Background);
                    break;

                case "Classic Elegance":
                    ApplyColors(form, ColorSchemes.ClassicElegance.Primary, ColorSchemes.ClassicElegance.Background);
                    break;

                case "Midnight":
                    ApplyColors(form, ColorSchemes.Midnight.Primary, ColorSchemes.Midnight.Background);
                    break;
            }

            void ApplyColors(Form form, Color primary, Color background)
            {
                // Determine text colors based on background brightness
                Color backgroundTextColor = IsColorDark(background) ? Color.White : Color.Black;
                Color primaryTextColor = IsColorDark(primary) ? Color.White : Color.Black;

                // Apply to form
                form.BackColor = background;
                form.ForeColor = backgroundTextColor;

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

                // Apply to buttons
                foreach (var button in new[] { dashboardBtn, newScanBtn, scanHistoryBtn, settingsBtn })
                {
                    if (button != null)
                    {
                        button.BackColor = primary;
                        button.ForeColor = primaryTextColor;
                    }
                }

                // Apply to labels
                if (appTitle != null) appTitle.ForeColor = primaryTextColor;
                if (title != null) title.ForeColor = backgroundTextColor;

                // Apply to all controls recursively
                ApplyThemeToControls(form.Controls, background, backgroundTextColor);
            }
        }

        private static void ApplyThemeToControls(Control.ControlCollection controls, Color background, Color textColor)
        {
            foreach (Control control in controls)
            {
                // Skip certain controls that should maintain their specific colors
                if (control is Panel || control is Button) continue;

                // Apply colors based on control type
                if (control is Label || control is CheckBox || control is RadioButton)
                {
                    control.ForeColor = textColor;
                }
                else if (control is TextBox)
                {
                    control.BackColor = Color.White;
                    control.ForeColor = Color.Black;
                }

                // Recursively apply to child controls
                if (control.HasChildren)
                {
                    ApplyThemeToControls(control.Controls, background, textColor);
                }
            }
        }

        // Change theme
        public static void ChangeTheme(string themeName)
        {
            currentTheme = themeName;
            ThemeChanged?.Invoke(null, new ThemeChangedEventArgs(themeName));
        }

        // Get current theme
        public static string GetCurrentTheme()
        {
            return currentTheme;
        }
    }

    // Event args for theme changed event
    public class ThemeChangedEventArgs : EventArgs
    {
        public string ThemeName { get; }
        public ThemeChangedEventArgs(string themeName)
        {
            ThemeName = themeName;
        }
    }
}