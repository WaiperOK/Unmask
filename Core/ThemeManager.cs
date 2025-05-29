using System;
using System.Drawing;
using System.Windows.Forms;

namespace Unmask
{
    public static class ThemeManager
    {
        public enum Theme
        {
            Light,
            Dark,
            Hacker
        }

        public static Theme CurrentTheme { get; set; } = Theme.Dark;
        public static event Action ThemeChanged;

        public static class Colors
        {
            public static Color Background => CurrentTheme switch
            {
                Theme.Light => Color.White,
                Theme.Dark => Color.FromArgb(32, 32, 32),
                Theme.Hacker => Color.Black,
                _ => Color.White
            };

            public static Color Surface => CurrentTheme switch
            {
                Theme.Light => Color.FromArgb(248, 249, 250),
                Theme.Dark => Color.FromArgb(48, 48, 48),
                Theme.Hacker => Color.FromArgb(10, 10, 10),
                _ => Color.FromArgb(248, 249, 250)
            };

            public static Color Primary => CurrentTheme switch
            {
                Theme.Light => Color.FromArgb(0, 123, 255),
                Theme.Dark => Color.FromArgb(100, 149, 237),
                Theme.Hacker => Color.FromArgb(0, 255, 0),
                _ => Color.FromArgb(0, 123, 255)
            };

            public static Color Secondary => CurrentTheme switch
            {
                Theme.Light => Color.FromArgb(108, 117, 125),
                Theme.Dark => Color.FromArgb(173, 181, 189),
                Theme.Hacker => Color.FromArgb(0, 200, 0),
                _ => Color.FromArgb(108, 117, 125)
            };

            public static Color Text => CurrentTheme switch
            {
                Theme.Light => Color.FromArgb(33, 37, 41),
                Theme.Dark => Color.FromArgb(248, 249, 250),
                Theme.Hacker => Color.FromArgb(0, 255, 0),
                _ => Color.FromArgb(33, 37, 41)
            };

            public static Color TextSecondary => CurrentTheme switch
            {
                Theme.Light => Color.FromArgb(108, 117, 125),
                Theme.Dark => Color.FromArgb(173, 181, 189),
                Theme.Hacker => Color.FromArgb(0, 200, 0),
                _ => Color.FromArgb(108, 117, 125)
            };

            public static Color Border => CurrentTheme switch
            {
                Theme.Light => Color.FromArgb(222, 226, 230),
                Theme.Dark => Color.FromArgb(64, 64, 64),
                Theme.Hacker => Color.FromArgb(0, 128, 0),
                _ => Color.FromArgb(222, 226, 230)
            };

            public static Color Success => CurrentTheme switch
            {
                Theme.Light => Color.FromArgb(40, 167, 69),
                Theme.Dark => Color.FromArgb(72, 187, 120),
                Theme.Hacker => Color.FromArgb(0, 255, 0),
                _ => Color.FromArgb(40, 167, 69)
            };

            public static Color Warning => CurrentTheme switch
            {
                Theme.Light => Color.FromArgb(255, 193, 7),
                Theme.Dark => Color.FromArgb(251, 211, 141),
                Theme.Hacker => Color.FromArgb(255, 255, 0),
                _ => Color.FromArgb(255, 193, 7)
            };

            public static Color Error => CurrentTheme switch
            {
                Theme.Light => Color.FromArgb(220, 53, 69),
                Theme.Dark => Color.FromArgb(248, 113, 113),
                Theme.Hacker => Color.FromArgb(255, 0, 0),
                _ => Color.FromArgb(220, 53, 69)
            };

            public static Color LogBackground => CurrentTheme switch
            {
                Theme.Light => Color.White,
                Theme.Dark => Color.FromArgb(24, 24, 24),
                Theme.Hacker => Color.Black,
                _ => Color.White
            };

            public static Color LogText => CurrentTheme switch
            {
                Theme.Light => Color.Black,
                Theme.Dark => Color.FromArgb(0, 255, 0),
                Theme.Hacker => Color.FromArgb(0, 255, 0),
                _ => Color.Black
            };

            public static Color ButtonHover => CurrentTheme switch
            {
                Theme.Light => Color.FromArgb(0, 105, 217),
                Theme.Dark => Color.FromArgb(120, 169, 255),
                Theme.Hacker => Color.FromArgb(0, 200, 0),
                _ => Color.FromArgb(0, 105, 217)
            };

            public static Color ProgressBar => CurrentTheme switch
            {
                Theme.Light => Color.FromArgb(0, 123, 255),
                Theme.Dark => Color.FromArgb(100, 149, 237),
                Theme.Hacker => Color.FromArgb(0, 255, 0),
                _ => Color.FromArgb(0, 123, 255)
            };
        }

        public static class Fonts
        {
            public static Font Default => new Font("Segoe UI", 9F, FontStyle.Regular);
            public static Font Bold => new Font("Segoe UI", 9F, FontStyle.Bold);
            public static Font Large => new Font("Segoe UI", 12F, FontStyle.Regular);
            public static Font Console => CurrentTheme switch
            {
                Theme.Hacker => new Font("Consolas", 9F, FontStyle.Regular),
                _ => new Font("Consolas", 9F, FontStyle.Regular)
            };
        }

        public static void ApplyTheme(Control control)
        {
            if (control == null) return;

            control.BackColor = Colors.Background;
            control.ForeColor = Colors.Text;

            switch (control)
            {
                case Form form:
                    ApplyFormTheme(form);
                    break;
                case Button button:
                    ApplyButtonTheme(button);
                    break;
                case TextBox textBox:
                    ApplyTextBoxTheme(textBox);
                    break;
                case RichTextBox richTextBox:
                    ApplyRichTextBoxTheme(richTextBox);
                    break;
                case GroupBox groupBox:
                    ApplyGroupBoxTheme(groupBox);
                    break;
                case CheckBox checkBox:
                    ApplyCheckBoxTheme(checkBox);
                    break;
                case Label label:
                    ApplyLabelTheme(label);
                    break;
                case ProgressBar progressBar:
                    ApplyProgressBarTheme(progressBar);
                    break;
                case Panel panel:
                    ApplyPanelTheme(panel);
                    break;
            }

            foreach (Control child in control.Controls)
            {
                ApplyTheme(child);
            }
        }

        private static void ApplyFormTheme(Form form)
        {
            form.BackColor = Colors.Background;
            form.ForeColor = Colors.Text;
            form.Font = Fonts.Default;
        }

        private static void ApplyButtonTheme(Button button)
        {
            button.BackColor = Colors.Primary;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = Colors.Border;
            button.FlatAppearance.MouseOverBackColor = Colors.ButtonHover;
            button.Font = Fonts.Default;
            button.Cursor = Cursors.Hand;
        }

        private static void ApplyTextBoxTheme(TextBox textBox)
        {
            textBox.BackColor = Colors.Surface;
            textBox.ForeColor = Colors.Text;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = Fonts.Default;
        }

        private static void ApplyRichTextBoxTheme(RichTextBox richTextBox)
        {
            richTextBox.BackColor = Colors.LogBackground;
            richTextBox.ForeColor = Colors.LogText;
            richTextBox.BorderStyle = BorderStyle.FixedSingle;
            richTextBox.Font = Fonts.Console;
        }

        private static void ApplyGroupBoxTheme(GroupBox groupBox)
        {
            groupBox.BackColor = Colors.Background;
            groupBox.ForeColor = Colors.Text;
            groupBox.Font = Fonts.Bold;
        }

        private static void ApplyCheckBoxTheme(CheckBox checkBox)
        {
            checkBox.BackColor = Colors.Background;
            checkBox.ForeColor = Colors.Text;
            checkBox.Font = Fonts.Default;
        }

        private static void ApplyLabelTheme(Label label)
        {
            label.BackColor = Colors.Background;
            label.ForeColor = Colors.Text;
            label.Font = Fonts.Default;
        }

        private static void ApplyProgressBarTheme(ProgressBar progressBar)
        {
            progressBar.BackColor = Colors.Surface;
            progressBar.ForeColor = Colors.ProgressBar;
        }

        private static void ApplyPanelTheme(Panel panel)
        {
            panel.BackColor = Colors.Background;
        }

        public static void SetTheme(Theme theme)
        {
            CurrentTheme = theme;
            ThemeChanged?.Invoke();
        }

        public static string GetThemeName(Theme theme)
        {
            return theme switch
            {
                Theme.Light => "Light",
                Theme.Dark => "Dark",
                Theme.Hacker => "Hacker",
                _ => "Light"
            };
        }

        public static Theme GetThemeFromName(string name)
        {
            return name switch
            {
                "Light" => Theme.Light,
                "Dark" => Theme.Dark,
                "Hacker" => Theme.Hacker,
                _ => Theme.Light
            };
        }
    }
} 