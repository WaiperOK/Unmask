using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace Unmask
{
    public partial class SettingsWindow : Form
    {
        private TabControl tabControl;
        private TabPage generalTab;
        private TabPage protectionTab;
        private TabPage scriptingTab;
        private TabPage outputTab;
        
        private CheckBox autoBackupCheckBox;
        private ComboBox languageComboBox;
        private CheckBox developerModeCheckBox;
        private CheckBox exportSeedsCheckBox;
        
        private CheckBox scriptingEnabledCheckBox;
        private TextBox scriptDirectoryTextBox;
        private TextBox defaultScriptTextBox;
        private NumericUpDown scriptTimeoutNumeric;
        
        private Button okButton;
        private Button cancelButton;
        private Button applyButton;

        public SettingsWindow()
        {
            InitializeComponent();
            LoadSettings();
            UpdateLanguage();
            
            LanguageManager.LanguageChanged += UpdateLanguage;
        }

        private void InitializeComponent()
        {
            this.Size = new Size(500, 400);
            this.Text = "Settings";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(10)
            };

            CreateGeneralTab();
            CreateProtectionTab();
            CreateScriptingTab();
            CreateOutputTab();

            var buttonPanel = new Panel
            {
                Height = 50,
                Dock = DockStyle.Bottom
            };

            okButton = new Button
            {
                Text = "OK",
                Size = new Size(75, 25),
                Location = new Point(320, 12),
                DialogResult = DialogResult.OK
            };
            okButton.Click += OkButton_Click;

            cancelButton = new Button
            {
                Text = "Cancel",
                Size = new Size(75, 25),
                Location = new Point(405, 12),
                DialogResult = DialogResult.Cancel
            };

            applyButton = new Button
            {
                Text = "Apply",
                Size = new Size(75, 25),
                Location = new Point(235, 12)
            };
            applyButton.Click += ApplyButton_Click;

            buttonPanel.Controls.AddRange(new Control[] { okButton, cancelButton, applyButton });

            this.Controls.Add(tabControl);
            this.Controls.Add(buttonPanel);

            tabControl.Controls.AddRange(new TabPage[] { generalTab, protectionTab, scriptingTab, outputTab });
        }

        private void CreateGeneralTab()
        {
            generalTab = new TabPage("General");

            var languageLabel = new Label
            {
                Text = "Language:",
                Location = new Point(20, 30),
                Size = new Size(100, 20)
            };

            languageComboBox = new ComboBox
            {
                Location = new Point(130, 28),
                Size = new Size(150, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            languageComboBox.Items.AddRange(new[] { "English", "Russian" });
            languageComboBox.SelectedIndexChanged += LanguageComboBox_SelectedIndexChanged;

            autoBackupCheckBox = new CheckBox
            {
                Text = "Auto Backup",
                Location = new Point(20, 70),
                Size = new Size(200, 20)
            };

            developerModeCheckBox = new CheckBox
            {
                Text = "Developer Mode",
                Location = new Point(20, 100),
                Size = new Size(200, 20)
            };

            exportSeedsCheckBox = new CheckBox
            {
                Text = "Export Random Seeds",
                Location = new Point(20, 130),
                Size = new Size(200, 20)
            };

            generalTab.Controls.AddRange(new Control[] {
                languageLabel, languageComboBox, autoBackupCheckBox,
                developerModeCheckBox, exportSeedsCheckBox
            });
        }

        private void CreateProtectionTab()
        {
            protectionTab = new TabPage("Protection Modules");

            var scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            var protectionCheckBoxes = new CheckBox[]
            {
                new CheckBox { Text = "Anti-Tamper", Tag = "AntiTamper", Location = new Point(20, 20) },
                new CheckBox { Text = "Anti-Dump", Tag = "AntiDump", Location = new Point(20, 50) },
                new CheckBox { Text = "Anti-Debug", Tag = "AntiDebug", Location = new Point(20, 80) },
                new CheckBox { Text = "Virtual Machines", Tag = "VirtualMachines", Location = new Point(20, 110) },
                new CheckBox { Text = "String Encryption", Tag = "StringEncryption", Location = new Point(20, 140) },
                new CheckBox { Text = "Control Flow", Tag = "ControlFlow", Location = new Point(20, 170) },
                new CheckBox { Text = "Data Structure Recovery", Tag = "DataStructureRecovery", Location = new Point(20, 200) },
                new CheckBox { Text = "Junk Code Removal", Tag = "JunkCodeRemoval", Location = new Point(20, 230) },
                new CheckBox { Text = "Delegate Type Recovery", Tag = "DelegateTypeRecovery", Location = new Point(20, 260) },
                new CheckBox { Text = "Hidden Assembly Reveal", Tag = "HiddenAssemblyReveal", Location = new Point(20, 290) },
                new CheckBox { Text = "In-Memory Decompilation", Tag = "InMemoryDecompilation", Location = new Point(20, 320) },
                new CheckBox { Text = "Scripting Engine", Tag = "ScriptingEngine", Location = new Point(20, 350) }
            };

            foreach (var checkBox in protectionCheckBoxes)
            {
                checkBox.Size = new Size(250, 20);
                scrollPanel.Controls.Add(checkBox);
            }

            protectionTab.Controls.Add(scrollPanel);
        }

        private void CreateScriptingTab()
        {
            scriptingTab = new TabPage("Scripting");

            scriptingEnabledCheckBox = new CheckBox
            {
                Text = "Enable Scripting",
                Location = new Point(20, 20),
                Size = new Size(200, 20)
            };

            var scriptDirLabel = new Label
            {
                Text = "Script Directory:",
                Location = new Point(20, 50),
                Size = new Size(100, 20)
            };

            scriptDirectoryTextBox = new TextBox
            {
                Location = new Point(130, 48),
                Size = new Size(150, 25)
            };

            var browseDirButton = new Button
            {
                Text = "...",
                Location = new Point(290, 48),
                Size = new Size(30, 25)
            };
            browseDirButton.Click += BrowseDirButton_Click;

            var defaultScriptLabel = new Label
            {
                Text = "Default Script:",
                Location = new Point(20, 80),
                Size = new Size(100, 20)
            };

            defaultScriptTextBox = new TextBox
            {
                Location = new Point(130, 78),
                Size = new Size(190, 25)
            };

            var timeoutLabel = new Label
            {
                Text = "Script Timeout (s):",
                Location = new Point(20, 110),
                Size = new Size(100, 20)
            };

            scriptTimeoutNumeric = new NumericUpDown
            {
                Location = new Point(130, 108),
                Size = new Size(100, 25),
                Minimum = 10,
                Maximum = 3600,
                Value = 300
            };

            // Дополнительные настройки скриптинга
            var enableBuiltInCheckBox = new CheckBox
            {
                Text = "Enable Built-in Scripts",
                Location = new Point(20, 140),
                Size = new Size(200, 20),
                Tag = "EnableBuiltInScripts"
            };

            var enableUserCheckBox = new CheckBox
            {
                Text = "Enable User Scripts",
                Location = new Point(20, 165),
                Size = new Size(200, 20),
                Tag = "EnableUserScripts"
            };

            var autoLoadCheckBox = new CheckBox
            {
                Text = "Auto Load Scripts",
                Location = new Point(20, 190),
                Size = new Size(200, 20),
                Tag = "AutoLoadScripts"
            };

            // Список доступных скриптов
            var scriptsLabel = new Label
            {
                Text = "Available Scripts:",
                Location = new Point(20, 220),
                Size = new Size(120, 20)
            };

            var scriptsListBox = new ListBox
            {
                Location = new Point(20, 245),
                Size = new Size(200, 80),
                Tag = "ScriptsList"
            };

            var refreshScriptsButton = new Button
            {
                Text = "Refresh",
                Location = new Point(230, 245),
                Size = new Size(70, 25)
            };
            refreshScriptsButton.Click += RefreshScriptsButton_Click;

            var executeScriptButton = new Button
            {
                Text = "Execute",
                Location = new Point(230, 275),
                Size = new Size(70, 25)
            };
            executeScriptButton.Click += ExecuteScriptButton_Click;

            var editScriptButton = new Button
            {
                Text = "Edit",
                Location = new Point(230, 305),
                Size = new Size(70, 25)
            };
            editScriptButton.Click += EditScriptButton_Click;

            scriptingTab.Controls.AddRange(new Control[] {
                scriptingEnabledCheckBox, scriptDirLabel, scriptDirectoryTextBox, browseDirButton,
                defaultScriptLabel, defaultScriptTextBox, timeoutLabel, scriptTimeoutNumeric,
                enableBuiltInCheckBox, enableUserCheckBox, autoLoadCheckBox,
                scriptsLabel, scriptsListBox, refreshScriptsButton, executeScriptButton, editScriptButton
            });

            // Загружаем список скриптов
            RefreshScriptsList(scriptsListBox);
        }

        private void CreateOutputTab()
        {
            outputTab = new TabPage("Output");

            var outputLabel = new Label
            {
                Text = "Output options will be added here",
                Location = new Point(20, 30),
                Size = new Size(300, 20)
            };

            outputTab.Controls.Add(outputLabel);
        }

        private void LoadSettings()
        {
            var config = SystemCore.Configuration;

            languageComboBox.SelectedItem = config.Language;
            autoBackupCheckBox.Checked = config.AutoBackup;
            developerModeCheckBox.Checked = config.DeveloperMode;
            exportSeedsCheckBox.Checked = config.ExportRandomSeeds;

            scriptingEnabledCheckBox.Checked = config.ScriptingEnabled;
            scriptDirectoryTextBox.Text = config.ScriptDirectory;
            defaultScriptTextBox.Text = config.DefaultScript;
            scriptTimeoutNumeric.Value = config.ScriptTimeout;

            // Загружаем дополнительные настройки скриптинга
            var enableBuiltInCheckBox = FindControlByTag("EnableBuiltInScripts") as CheckBox;
            if (enableBuiltInCheckBox != null)
                enableBuiltInCheckBox.Checked = config.EnableBuiltInScripts;

            var enableUserCheckBox = FindControlByTag("EnableUserScripts") as CheckBox;
            if (enableUserCheckBox != null)
                enableUserCheckBox.Checked = config.EnableUserScripts;

            var autoLoadCheckBox = FindControlByTag("AutoLoadScripts") as CheckBox;
            if (autoLoadCheckBox != null)
                autoLoadCheckBox.Checked = config.AutoLoadScripts;

            foreach (Control control in protectionTab.Controls[0].Controls)
            {
                if (control is CheckBox checkBox && checkBox.Tag is string tag)
                {
                    checkBox.Checked = GetProtectionSetting(tag);
                }
            }
        }

        private bool GetProtectionSetting(string tag)
        {
            var config = SystemCore.Configuration;
            switch (tag)
            {
                case "AntiTamper": return config.ProcessAntiTamper;
                case "AntiDump": return config.ProcessAntiDump;
                case "AntiDebug": return config.ProcessAntiDebug;
                case "VirtualMachines": return config.ProcessVirtualMachines;
                case "StringEncryption": return config.ProcessStringEncryption;
                case "ControlFlow": return config.ProcessControlFlow;
                case "DataStructureRecovery": return config.ProcessDataStructureRecovery;
                case "JunkCodeRemoval": return config.ProcessJunkCodeRemoval;
                case "DelegateTypeRecovery": return config.ProcessDelegateTypeRecovery;
                case "HiddenAssemblyReveal": return config.ProcessHiddenAssemblyReveal;
                case "InMemoryDecompilation": return config.ProcessInMemoryDecompilation;
                case "ScriptingEngine": return config.ProcessScriptingEngine;
                default: return false;
            }
        }

        private void SaveSettings()
        {
            var config = SystemCore.Configuration;

            config.Language = languageComboBox.SelectedItem?.ToString() ?? "English";
            config.AutoBackup = autoBackupCheckBox.Checked;
            config.DeveloperMode = developerModeCheckBox.Checked;
            config.ExportRandomSeeds = exportSeedsCheckBox.Checked;

            config.ScriptingEnabled = scriptingEnabledCheckBox.Checked;
            config.ScriptDirectory = scriptDirectoryTextBox.Text;
            config.DefaultScript = defaultScriptTextBox.Text;
            config.ScriptTimeout = (int)scriptTimeoutNumeric.Value;

            // Загружаем дополнительные настройки скриптинга
            var enableBuiltInCheckBox = FindControlByTag("EnableBuiltInScripts") as CheckBox;
            if (enableBuiltInCheckBox != null)
                config.EnableBuiltInScripts = enableBuiltInCheckBox.Checked;

            var enableUserCheckBox = FindControlByTag("EnableUserScripts") as CheckBox;
            if (enableUserCheckBox != null)
                config.EnableUserScripts = enableUserCheckBox.Checked;

            var autoLoadCheckBox = FindControlByTag("AutoLoadScripts") as CheckBox;
            if (autoLoadCheckBox != null)
                config.AutoLoadScripts = autoLoadCheckBox.Checked;

            foreach (Control control in protectionTab.Controls[0].Controls)
            {
                if (control is CheckBox checkBox && checkBox.Tag is string tag)
                {
                    SetProtectionSetting(tag, checkBox.Checked);
                }
            }

            try
            {
                config.SaveToFile("unmask_config.txt");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save settings: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetProtectionSetting(string tag, bool value)
        {
            var config = SystemCore.Configuration;
            switch (tag)
            {
                case "AntiTamper": config.ProcessAntiTamper = value; break;
                case "AntiDump": config.ProcessAntiDump = value; break;
                case "AntiDebug": config.ProcessAntiDebug = value; break;
                case "VirtualMachines": config.ProcessVirtualMachines = value; break;
                case "StringEncryption": config.ProcessStringEncryption = value; break;
                case "ControlFlow": config.ProcessControlFlow = value; break;
                case "DataStructureRecovery": config.ProcessDataStructureRecovery = value; break;
                case "JunkCodeRemoval": config.ProcessJunkCodeRemoval = value; break;
                case "DelegateTypeRecovery": config.ProcessDelegateTypeRecovery = value; break;
                case "HiddenAssemblyReveal": config.ProcessHiddenAssemblyReveal = value; break;
                case "InMemoryDecompilation": config.ProcessInMemoryDecompilation = value; break;
                case "ScriptingEngine": config.ProcessScriptingEngine = value; break;
            }
        }

        private void UpdateLanguage()
        {
            this.Text = LanguageManager.GetString("Advanced_Settings");
            generalTab.Text = LanguageManager.GetString("General");
            protectionTab.Text = LanguageManager.GetString("Protection_Modules");
            scriptingTab.Text = LanguageManager.GetString("Scripting_Options");
            outputTab.Text = LanguageManager.GetString("Output_Options");

            okButton.Text = "OK";
            cancelButton.Text = "Cancel";
            applyButton.Text = "Apply";
        }

        private void LanguageComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedLanguage = languageComboBox.SelectedItem?.ToString();
            if (selectedLanguage == "Russian")
            {
                LanguageManager.CurrentLanguage = LanguageManager.Language.Russian;
            }
            else
            {
                LanguageManager.CurrentLanguage = LanguageManager.Language.English;
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            this.Close();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void BrowseDirButton_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select Scripts Directory";
                folderDialog.SelectedPath = scriptDirectoryTextBox.Text;
                
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    scriptDirectoryTextBox.Text = folderDialog.SelectedPath;
                }
            }
        }

        private void RefreshScriptsButton_Click(object sender, EventArgs e)
        {
            var scriptsListBox = FindControlByTag("ScriptsList") as ListBox;
            if (scriptsListBox != null)
            {
                RefreshScriptsList(scriptsListBox);
            }
        }

        private void ExecuteScriptButton_Click(object sender, EventArgs e)
        {
            var scriptsListBox = FindControlByTag("ScriptsList") as ListBox;
            if (scriptsListBox?.SelectedItem != null)
            {
                var scriptName = scriptsListBox.SelectedItem.ToString();
                try
                {
                    if (SystemCore.TargetModule != null)
                    {
                        var logger = new ConsoleLogger();
                        ScriptingEngine.ExecuteScript(scriptName, SystemCore.TargetModule, logger);
                        MessageBox.Show($"Script '{scriptName}' executed successfully!", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("No module loaded. Please load a file first.", "Warning", 
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error executing script '{scriptName}': {ex.Message}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a script to execute.", "Warning", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void EditScriptButton_Click(object sender, EventArgs e)
        {
            var scriptsListBox = FindControlByTag("ScriptsList") as ListBox;
            if (scriptsListBox?.SelectedItem != null)
            {
                var scriptName = scriptsListBox.SelectedItem.ToString();
                var scriptPath = System.IO.Path.Combine(scriptDirectoryTextBox.Text, $"{scriptName}.cs");
                
                if (System.IO.File.Exists(scriptPath))
                {
                    try
                    {
                        System.Diagnostics.Process.Start("notepad.exe", scriptPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error opening script file: {ex.Message}", "Error", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show($"Script file not found: {scriptPath}", "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a script to edit.", "Warning", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RefreshScriptsList(ListBox listBox)
        {
            listBox.Items.Clear();
            
            try
            {
                // Добавляем встроенные скрипты
                var builtInScripts = new[] { "BasicDeobfuscation", "StringDecryption", "ControlFlowRecovery", "MetadataRepair" };
                foreach (var script in builtInScripts)
                {
                    listBox.Items.Add($"[Built-in] {script}");
                }
                
                // Добавляем пользовательские скрипты
                var availableScripts = ScriptingEngine.GetAvailableScripts();
                foreach (var scriptName in availableScripts)
                {
                    if (Array.IndexOf(builtInScripts, scriptName) == -1)
                    {
                        listBox.Items.Add($"[User] {scriptName}");
                    }
                }
                
                // Добавляем файлы скриптов из директории
                var scriptDir = scriptDirectoryTextBox.Text;
                if (System.IO.Directory.Exists(scriptDir))
                {
                    var scriptFiles = System.IO.Directory.GetFiles(scriptDir, "*.cs");
                    foreach (var file in scriptFiles)
                    {
                        var fileName = System.IO.Path.GetFileNameWithoutExtension(file);
                        if (availableScripts.IndexOf(fileName) == -1)
                        {
                            listBox.Items.Add($"[File] {fileName}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing scripts list: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private Control FindControlByTag(string tag)
        {
            return FindControlByTagRecursive(this, tag);
        }

        private Control FindControlByTagRecursive(Control parent, string tag)
        {
            foreach (Control control in parent.Controls)
            {
                if (control.Tag?.ToString() == tag)
                    return control;
                
                var found = FindControlByTagRecursive(control, tag);
                if (found != null)
                    return found;
            }
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                LanguageManager.LanguageChanged -= UpdateLanguage;
            }
            base.Dispose(disposing);
        }
    }
} 