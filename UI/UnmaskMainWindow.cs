using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Unmask
{
    /// <summary>
    /// Главное окно приложения Unmask
    /// </summary>
    public partial class UnmaskMainWindow : Form
    {
        private TextBox targetFilePathBox;
        private Button selectFileButton;
        private Button processButton;
        private Button settingsButton;
        private Button languageButton;
        private Button themeButton;
        private RichTextBox operationLogBox;
        private ProgressBar operationProgressBar;
        private Label statusLabel;
        private GroupBox protectionGroupBox;
        private CheckBox[] protectionCheckBoxes;
        private Button selectAllButton;
        private Button deselectAllButton;
        private string selectedTargetFile = "";

        public UnmaskMainWindow()
        {
            InitializeInterface();
            SetupProtectionCheckboxes();
            UpdateLanguage();
            
            LanguageManager.LanguageChanged += UpdateLanguage;
            ThemeManager.ThemeChanged += UpdateTheme;
            
            UpdateTheme();
        }

        private void InitializeInterface()
        {
            this.Text = "Unmask - Advanced .NET Deobfuscator";
            this.Size = new Size(1000, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(900, 700);
            this.Icon = LoadApplicationIcon();

            var filePanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(10)
            };

            var fileLabel = new Label
            {
                Text = "Target File:",
                Location = new Point(10, 15),
                Size = new Size(100, 20)
            };

            targetFilePathBox = new TextBox
            {
                Location = new Point(120, 12),
                Size = new Size(500, 25),
                ReadOnly = true,
                BackColor = Color.White
            };

            selectFileButton = new Button
            {
                Text = "Browse...",
                Location = new Point(630, 10),
                Size = new Size(80, 30),
                UseVisualStyleBackColor = true
            };
            selectFileButton.Click += SelectFileButton_Click;

            settingsButton = new Button
            {
                Text = "Settings",
                Location = new Point(720, 10),
                Size = new Size(80, 30),
                UseVisualStyleBackColor = true
            };
            settingsButton.Click += SettingsButton_Click;

            languageButton = new Button
            {
                Text = "EN/RU",
                Location = new Point(810, 10),
                Size = new Size(60, 30),
                UseVisualStyleBackColor = true
            };
            languageButton.Click += LanguageButton_Click;

            themeButton = new Button
            {
                Text = "Theme",
                Location = new Point(880, 10),
                Size = new Size(60, 30),
                UseVisualStyleBackColor = true
            };
            themeButton.Click += ThemeButton_Click;

            filePanel.Controls.AddRange(new Control[] { 
                fileLabel, targetFilePathBox, selectFileButton, settingsButton, languageButton, themeButton 
            });

            protectionGroupBox = new GroupBox
            {
                Text = "Protection Settings",
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            var buttonPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40
            };

            selectAllButton = new Button
            {
                Text = "Select All",
                Location = new Point(10, 8),
                Size = new Size(100, 25),
                UseVisualStyleBackColor = true
            };
            selectAllButton.Click += (s, e) => SetAllProtections(true);

            deselectAllButton = new Button
            {
                Text = "Deselect All",
                Location = new Point(120, 8),
                Size = new Size(100, 25),
                UseVisualStyleBackColor = true
            };
            deselectAllButton.Click += (s, e) => SetAllProtections(false);

            buttonPanel.Controls.AddRange(new Control[] { selectAllButton, deselectAllButton });

            var statusPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                Padding = new Padding(10)
            };

            statusLabel = new Label
            {
                Text = "Ready",
                Location = new Point(10, 10),
                Size = new Size(400, 20),
                ForeColor = Color.DarkGreen
            };

            operationProgressBar = new ProgressBar
            {
                Location = new Point(10, 35),
                Size = new Size(960, 20),
                Style = ProgressBarStyle.Continuous
            };

            processButton = new Button
            {
                Text = "Start Processing",
                Location = new Point(10, 65),
                Size = new Size(150, 35),
                UseVisualStyleBackColor = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            processButton.Click += ProcessButton_Click;

            statusPanel.Controls.AddRange(new Control[] { statusLabel, operationProgressBar, processButton });

            var logPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 200,
                Padding = new Padding(10)
            };

            var logLabel = new Label
            {
                Text = "Operation Log:",
                Location = new Point(10, 5),
                Size = new Size(150, 20)
            };

            operationLogBox = new RichTextBox
            {
                Location = new Point(10, 25),
                Size = new Size(960, 165),
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.LightGreen,
                Font = new Font("Consolas", 9),
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            logPanel.Controls.AddRange(new Control[] { logLabel, operationLogBox });

            this.Controls.Add(protectionGroupBox);
            this.Controls.Add(buttonPanel);
            this.Controls.Add(logPanel);
            this.Controls.Add(statusPanel);
            this.Controls.Add(filePanel);

            LogOperation("Unmask system initialized", Color.Cyan);
            LogOperation("Select a file to process", Color.Yellow);
        }

        private void SetupProtectionCheckboxes()
        {
            string[] protectionKeys = {
                "AntiTamper", "AntiDump", "AntiDebug", "AntiDe4Dot",
                "StringEncryption", "ControlFlow", "VirtualMachines", 
                "DataStructureRecovery", "JunkCodeRemoval", "DelegateTypeRecovery",
                "HiddenAssemblyReveal", "InMemoryDecompilation", "ScriptingEngine",
                "AntiTamperChecks", "Watermarks", "JumpControlFlow", 
                "ProxyConstants", "ProxyStrings", "ProxyMethods", 
                "IntConfusion", "Arithmetic", "OnlineStringDecryption",
                "ResourceEncryption", "ResourceProtections", "StackUnfConfusion",
                "Callis", "InvalidMetadata", "LocalField", "Renamer"
            };

            protectionCheckBoxes = new CheckBox[protectionKeys.Length];

            int x = 20, y = 50;
            int columnWidth = 250;
            int rowHeight = 25;
            int columnsPerRow = 3;

            for (int i = 0; i < protectionKeys.Length; i++)
            {
                protectionCheckBoxes[i] = new CheckBox
                {
                    Text = LanguageManager.GetString(protectionKeys[i]),
                    Tag = protectionKeys[i],
                    Location = new Point(x + (i % columnsPerRow) * columnWidth, y + (i / columnsPerRow) * rowHeight),
                    Size = new Size(columnWidth - 10, 20),
                    Checked = true
                };

                protectionGroupBox.Controls.Add(protectionCheckBoxes[i]);
            }
        }

        private Icon LoadApplicationIcon()
        {
            try
            {
                if (File.Exists("icon.ico"))
                    return new Icon("icon.ico");
                return SystemIcons.Application;
            }
            catch
            {
                return SystemIcons.Application;
            }
        }

        private void SelectFileButton_Click(object sender, EventArgs e)
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = ".NET Assemblies (*.exe;*.dll)|*.exe;*.dll|All files (*.*)|*.*";
                openFileDialog.Title = LanguageManager.GetString("File_Select");

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedTargetFile = openFileDialog.FileName;
                    targetFilePathBox.Text = selectedTargetFile;
                    LogOperation($"Selected file: {Path.GetFileName(selectedTargetFile)}", Color.White);
                    UpdateStatus("File selected", Color.DarkGreen);
                }
            }
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            using (var settingsWindow = new SettingsWindow())
            {
                settingsWindow.ShowDialog(this);
            }
        }

        private void LanguageButton_Click(object sender, EventArgs e)
        {
            if (LanguageManager.CurrentLanguage == LanguageManager.Language.English)
            {
                LanguageManager.CurrentLanguage = LanguageManager.Language.Russian;
            }
            else
            {
                LanguageManager.CurrentLanguage = LanguageManager.Language.English;
            }
        }

        private void ThemeButton_Click(object sender, EventArgs e)
        {
            var currentTheme = ThemeManager.CurrentTheme;
            var nextTheme = currentTheme switch
            {
                ThemeManager.Theme.Light => ThemeManager.Theme.Dark,
                ThemeManager.Theme.Dark => ThemeManager.Theme.Hacker,
                ThemeManager.Theme.Hacker => ThemeManager.Theme.Light,
                _ => ThemeManager.Theme.Light
            };
            
            ThemeManager.SetTheme(nextTheme);
            SystemCore.Configuration.Theme = ThemeManager.GetThemeName(nextTheme);
            
            if (SystemCore.Configuration.AutoSave)
            {
                SystemCore.Configuration.SaveToFile("unmask_config.txt");
            }
        }

        private async void ProcessButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedTargetFile))
            {
                MessageBox.Show(LanguageManager.GetString("File_Not_Selected"), "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            processButton.Enabled = false;
            operationProgressBar.Value = 0;
            
            try
            {
                await ProcessFileAsync();
                LogOperation(LanguageManager.GetString("Processing_Complete"), Color.Green);
                UpdateStatus("Processing completed successfully", Color.DarkGreen);
            }
            catch (Exception ex)
            {
                LogOperation($"{LanguageManager.GetString("Error_Occurred")}: {ex.Message}", Color.Red);
                UpdateStatus("Processing failed", Color.Red);
            }
            finally
            {
                processButton.Enabled = true;
                operationProgressBar.Value = 0;
            }
        }

        private async Task ProcessFileAsync()
        {
            UpdateStatus("Loading target file...", Color.Blue);
            operationProgressBar.Value = 10;

            SystemCore.LoadTargetFile(selectedTargetFile);
            LogOperation($"Loaded module: {SystemCore.TargetModule.Name}", Color.Cyan);

            operationProgressBar.Value = 20;
            await ApplySelectedProtections();

            operationProgressBar.Value = 90;
            await SaveProcessedFile();

            operationProgressBar.Value = 100;
        }

        private async Task ApplySelectedProtections()
        {
            var flags = new ProtectionFlags();
            int selectedCount = 0;

            foreach (var checkBox in protectionCheckBoxes)
            {
                if (checkBox.Checked)
                {
                    selectedCount++;
                    var tag = checkBox.Tag.ToString();
                    SetProtectionFlag(flags, tag, true);
                }
            }

            LogOperation($"Applying {selectedCount} protection removals...", Color.Yellow);

            var processor = new ProtectionProcessor();
            await Task.Run(() => processor.ProcessProtections(SystemCore.TargetModule, flags));

            LogOperation("Protection processing completed", Color.Green);
        }

        private void SetProtectionFlag(ProtectionFlags flags, string tag, bool value)
        {
            switch (tag)
            {
                case "AntiTamper": flags.AntiTamper = value; break;
                case "AntiDump": flags.AntiDump = value; break;
                case "AntiDebug": flags.AntiDebug = value; break;
                case "AntiDe4Dot": flags.AntiDe4Dot = value; break;
                case "StringEncryption": flags.EncryptedStrings = value; break;
                case "ControlFlow": flags.ControlFlow = value; break;
                case "VirtualMachines": flags.VirtualMachines = value; break;
                case "DataStructureRecovery": flags.DataStructureRecovery = value; break;
                case "JunkCodeRemoval": flags.JunkCodeRemoval = value; break;
                case "DelegateTypeRecovery": flags.DelegateTypeRecovery = value; break;
                case "HiddenAssemblyReveal": flags.HiddenAssemblyReveal = value; break;
                case "InMemoryDecompilation": flags.InMemoryDecompilation = value; break;
                case "ScriptingEngine": flags.ScriptingEngine = value; break;
                case "AntiTamperChecks": flags.AntiTamperChecks = value; break;
                case "Watermarks": flags.Watermarks = value; break;
                case "JumpControlFlow": flags.JumpControlFlow = value; break;
                case "ProxyConstants": flags.ProxyConstants = value; break;
                case "ProxyStrings": flags.ProxyStrings = value; break;
                case "ProxyMethods": flags.ProxyMethods = value; break;
                case "IntConfusion": flags.IntConfusion = value; break;
                case "Arithmetic": flags.Arithmetic = value; break;
                case "OnlineStringDecryption": flags.OnlineStringDecryption = value; break;
                case "ResourceEncryption": flags.ResourceEncryption = value; break;
                case "ResourceProtections": flags.ResourceProtections = value; break;
                case "StackUnfConfusion": flags.StackUnfConfusion = value; break;
                case "Callis": flags.Callis = value; break;
                case "InvalidMetadata": flags.InvalidMetadata = value; break;
                case "LocalField": flags.LocalField = value; break;
                case "Renamer": flags.Renamer = value; break;
            }
        }

        private async Task SaveProcessedFile()
        {
            UpdateStatus("Saving processed file...", Color.Blue);

            var outputPath = Path.Combine(
                Path.GetDirectoryName(selectedTargetFile),
                Path.GetFileNameWithoutExtension(selectedTargetFile) + "_deobfuscated" + Path.GetExtension(selectedTargetFile)
            );

            await Task.Run(() => SystemCore.TargetModule.Write(outputPath));
            LogOperation($"Saved to: {outputPath}", Color.Cyan);
        }

        private void SetAllProtections(bool isChecked)
        {
            foreach (var checkBox in protectionCheckBoxes)
            {
                checkBox.Checked = isChecked;
            }
        }

        private void UpdateStatus(string message, Color color)
        {
            if (statusLabel.InvokeRequired)
            {
                statusLabel.Invoke(new Action(() => {
                    statusLabel.Text = message;
                    statusLabel.ForeColor = color;
                }));
            }
            else
            {
                statusLabel.Text = message;
                statusLabel.ForeColor = color;
            }
        }

        private void LogOperation(string message, Color color)
        {
            if (operationLogBox.InvokeRequired)
            {
                operationLogBox.Invoke(new Action(() => AppendLog(message, color)));
            }
            else
            {
                AppendLog(message, color);
            }
        }

        private void AppendLog(string message, Color color)
        {
            operationLogBox.SelectionStart = operationLogBox.TextLength;
            operationLogBox.SelectionLength = 0;
            operationLogBox.SelectionColor = color;
            operationLogBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
            operationLogBox.ScrollToCaret();
        }

        private void UpdateLanguage()
        {
            this.Text = LanguageManager.GetString("MainWindow_Title");
            protectionGroupBox.Text = LanguageManager.GetString("Protection_Settings");
            selectAllButton.Text = LanguageManager.GetString("Select_All");
            deselectAllButton.Text = LanguageManager.GetString("Deselect_All");
            processButton.Text = LanguageManager.GetString("Start_Processing");
            settingsButton.Text = LanguageManager.GetString("Settings");
            selectFileButton.Text = LanguageManager.GetString("File_Browse");
            languageButton.Text = LanguageManager.GetString("Language_EN_RU");
            themeButton.Text = LanguageManager.GetString("Theme");

            foreach (var checkBox in protectionCheckBoxes)
            {
                if (checkBox.Tag is string tag)
                {
                    checkBox.Text = LanguageManager.GetString(tag);
                }
            }
        }

        private void UpdateTheme()
        {
            ThemeManager.ApplyTheme(this);
            
            if (operationLogBox != null)
            {
                operationLogBox.BackColor = ThemeManager.Colors.LogBackground;
                operationLogBox.ForeColor = ThemeManager.Colors.LogText;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                LanguageManager.LanguageChanged -= UpdateLanguage;
                ThemeManager.ThemeChanged -= UpdateTheme;
                SystemCore.Cleanup();
            }
            base.Dispose(disposing);
        }
    }
} 