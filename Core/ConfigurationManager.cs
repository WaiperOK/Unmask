using System;
using System.IO;

namespace Unmask
{
    /// <summary>
    /// Менеджер конфигурации приложения
    /// </summary>
    public class ConfigurationManager
    {
        // Основные настройки
        public bool UseDefaultPresets = false;
        public string PresetsFilePath = "unmask_presets.txt";
        public bool ExportRandomSeeds = true;
        public bool DeveloperMode = false;
        public string Language = "English";
        public bool AutoBackup = true;
        public string Theme = "Dark";
        public bool EnableAnimations = true;
        public bool ShowAdvancedOptions = false;
        public int LogLevel = 2;
        public bool AutoSave = true;
        public string OutputDirectory = "";

        // Настройки очистки
        public bool RemoveInvalidCalls = true;
        public bool RemoveUselessJumps = true;
        public bool RemoveUselessNOPs = true;
        public bool RemoveUnusedLocals = true;
        public bool RemoveUnusedVariables = true;

        // Настройки защит
        public bool ProcessAntiTamper = true;
        public bool ProcessAntiDump = true;
        public bool ProcessAntiDebug = true;
        public bool ProcessAntiDe4Dot = true;
        public bool ProcessWatermarks = true;
        public bool ProcessLocal2Field = true;
        public bool ProcessJumpControlFlow = true;
        public bool ProcessProxyConstants = true;
        public bool ProcessProxyStrings = true;
        public bool ProcessIntConfusion = true;
        public bool ProcessArithmetic = true;
        public bool ProcessStringEncryption = true;
        public bool ProcessOnlineStringDecryption = true;
        public bool ProcessResourceEncryption = true;
        public bool ProcessStackUnfConfusion = true;
        public bool ProcessCallis = true;
        public bool ProcessInvalidMetadata = true;

        // Новые защиты
        public bool ProcessControlFlow = true;
        public bool ProcessRenamer = true;
        public bool ProcessProxyMethods = true;
        public bool ProcessResourceProtections = true;
        public bool ProcessDataStructureRecovery = true;
        public bool ProcessJunkCodeRemoval = true;
        public bool ProcessDelegateTypeRecovery = true;
        public bool ProcessVirtualMachines = true;
        public bool ProcessHiddenAssemblyReveal = true;
        public bool ProcessInMemoryDecompilation = true;
        public bool ProcessScriptingEngine = true;
        public bool ProcessAntiTamperChecks = true;

        // Новые настройки
        public bool ScriptingEnabled = true;
        public string ScriptDirectory = "Scripts";
        public string DefaultScript = "default_deobfuscation.cs";
        public int ScriptTimeout = 300;

        // Настройки скриптинга
        public bool EnableBuiltInScripts = true;
        public bool EnableUserScripts = true;
        public bool AutoLoadScripts = true;
        public string AutoLoadScriptList = "BasicDeobfuscation,StringDecryption,MetadataRepair";

        // Настройки логирования
        public bool LogToFile = true;
        public string LogFileName = "unmask.log";

        /// <summary>
        /// Загрузка конфигурации из файла
        /// </summary>
        public void LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return;

            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//")) continue;

                string[] parts = line.Split('=');
                if (parts.Length != 2) continue;

                string key = parts[0].Trim();
                string value = parts[1].Trim();

                SetConfigValue(key, value);
            }
        }

        /// <summary>
        /// Сохранение конфигурации в файл
        /// </summary>
        public void SaveToFile(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("[General]");
                writer.WriteLine($"UseDefaultPresets = {(UseDefaultPresets ? 1 : 0)}");
                writer.WriteLine($"PresetsFile = {PresetsFilePath}");
                writer.WriteLine($"DeveloperMode = {(DeveloperMode ? 1 : 0)}");
                writer.WriteLine($"ExportRandomSeeds = {(ExportRandomSeeds ? 1 : 0)}");
                writer.WriteLine($"Language = {Language}");
                writer.WriteLine($"AutoBackup = {(AutoBackup ? 1 : 0)}");
                writer.WriteLine($"Theme = {Theme}");
                writer.WriteLine($"EnableAnimations = {(EnableAnimations ? 1 : 0)}");
                writer.WriteLine($"ShowAdvancedOptions = {(ShowAdvancedOptions ? 1 : 0)}");
                writer.WriteLine($"LogLevel = {LogLevel}");
                writer.WriteLine($"AutoSave = {(AutoSave ? 1 : 0)}");
                writer.WriteLine($"OutputDirectory = {OutputDirectory}");
                writer.WriteLine();

                writer.WriteLine("[AntiProtections]");
                writer.WriteLine($"AntiTamper = {(ProcessAntiTamper ? 1 : 0)}");
                writer.WriteLine($"AntiDump = {(ProcessAntiDump ? 1 : 0)}");
                writer.WriteLine($"AntiDebug = {(ProcessAntiDebug ? 1 : 0)}");
                writer.WriteLine($"AntiDe4Dot = {(ProcessAntiDe4Dot ? 1 : 0)}");
                writer.WriteLine($"VirtualMachines = {(ProcessVirtualMachines ? 1 : 0)}");
                writer.WriteLine();

                writer.WriteLine("[ControlFlow]");
                writer.WriteLine($"Watermarks = {(ProcessWatermarks ? 1 : 0)}");
                writer.WriteLine($"JumpControlFlow = {(ProcessJumpControlFlow ? 1 : 0)}");
                writer.WriteLine($"ControlFlow = {(ProcessControlFlow ? 1 : 0)}");
                writer.WriteLine($"DataStructureRecovery = {(ProcessDataStructureRecovery ? 1 : 0)}");
                writer.WriteLine();

                writer.WriteLine("[Proxies]");
                writer.WriteLine($"ProxyConstants = {(ProcessProxyConstants ? 1 : 0)}");
                writer.WriteLine($"ProxyStrings = {(ProcessProxyStrings ? 1 : 0)}");
                writer.WriteLine($"ProxyMethods = {(ProcessProxyMethods ? 1 : 0)}");
                writer.WriteLine($"DelegateTypeRecovery = {(ProcessDelegateTypeRecovery ? 1 : 0)}");
                writer.WriteLine();

                writer.WriteLine("[Advanced]");
                writer.WriteLine($"ScriptingEnabled = {(ProcessScriptingEngine ? 1 : 0)}");
                writer.WriteLine($"InMemoryDecompilation = {(ProcessInMemoryDecompilation ? 1 : 0)}");
                writer.WriteLine($"JunkCodeRemoval = {(ProcessJunkCodeRemoval ? 1 : 0)}");
                writer.WriteLine($"HiddenAssemblyReveal = {(ProcessHiddenAssemblyReveal ? 1 : 0)}");
                writer.WriteLine($"AntiTamperChecks = {(ProcessAntiTamperChecks ? 1 : 0)}");
                writer.WriteLine();

                writer.WriteLine("[Scripting]");
                writer.WriteLine($"ScriptingEnabled = {(ScriptingEnabled ? 1 : 0)}");
                writer.WriteLine($"ScriptDirectory = {ScriptDirectory}");
                writer.WriteLine($"DefaultScript = {DefaultScript}");
                writer.WriteLine($"ScriptTimeout = {ScriptTimeout}");
                writer.WriteLine();

                writer.WriteLine("[Mathematics]");
                writer.WriteLine($"IntConfusion = {(ProcessIntConfusion ? 1 : 0)}");
                writer.WriteLine($"Arithmetic = {(ProcessArithmetic ? 1 : 0)}");
                writer.WriteLine();

                writer.WriteLine("[Encryption]");
                writer.WriteLine($"StringEncryption = {(ProcessStringEncryption ? 1 : 0)}");
                writer.WriteLine($"OnlineStringDecryption = {(ProcessOnlineStringDecryption ? 1 : 0)}");
                writer.WriteLine($"ResourceEncryption = {(ProcessResourceEncryption ? 1 : 0)}");
                writer.WriteLine($"ResourceProtections = {(ProcessResourceProtections ? 1 : 0)}");
                writer.WriteLine();

                writer.WriteLine("[Miscellaneous]");
                writer.WriteLine($"StackUnfConfusion = {(ProcessStackUnfConfusion ? 1 : 0)}");
                writer.WriteLine($"Callis = {(ProcessCallis ? 1 : 0)}");
                writer.WriteLine($"InvalidMetadata = {(ProcessInvalidMetadata ? 1 : 0)}");
                writer.WriteLine($"Local2Field = {(ProcessLocal2Field ? 1 : 0)}");
                writer.WriteLine($"Renamer = {(ProcessRenamer ? 1 : 0)}");
                writer.WriteLine();

                writer.WriteLine("[Cleanup]");
                writer.WriteLine($"InvalidCalls = {(RemoveInvalidCalls ? 1 : 0)}");
                writer.WriteLine($"UselessJumps = {(RemoveUselessJumps ? 1 : 0)}");
                writer.WriteLine($"UselessNOPs = {(RemoveUselessNOPs ? 1 : 0)}");
                writer.WriteLine($"UnusedLocals = {(RemoveUnusedLocals ? 1 : 0)}");
                writer.WriteLine($"UnusedVariables = {(RemoveUnusedVariables ? 1 : 0)}");
            }
        }

        private void SetConfigValue(string key, string value)
        {
            bool boolValue = value == "1" || value.ToLower() == "true";

            switch (key)
            {
                case "UseDefaultPresets": UseDefaultPresets = boolValue; break;
                case "PresetsFile": PresetsFilePath = value; break;
                case "DeveloperMode": DeveloperMode = boolValue; break;
                case "ExportRandomSeeds": ExportRandomSeeds = boolValue; break;
                case "Language": Language = value; break;
                case "AutoBackup": AutoBackup = boolValue; break;
                case "Theme": Theme = value; break;
                case "EnableAnimations": EnableAnimations = boolValue; break;
                case "ShowAdvancedOptions": ShowAdvancedOptions = boolValue; break;
                case "LogLevel": if (int.TryParse(value, out int level)) LogLevel = level; break;
                case "AutoSave": AutoSave = boolValue; break;
                case "OutputDirectory": OutputDirectory = value; break;
                case "AntiTamper": ProcessAntiTamper = boolValue; break;
                case "AntiDump": ProcessAntiDump = boolValue; break;
                case "AntiDebug": ProcessAntiDebug = boolValue; break;
                case "AntiDe4Dot": ProcessAntiDe4Dot = boolValue; break;
                case "VirtualMachines": ProcessVirtualMachines = boolValue; break;
                case "Watermarks": ProcessWatermarks = boolValue; break;
                case "JumpControlFlow": ProcessJumpControlFlow = boolValue; break;
                case "ControlFlow": ProcessControlFlow = boolValue; break;
                case "DataStructureRecovery": ProcessDataStructureRecovery = boolValue; break;
                case "ProxyConstants": ProcessProxyConstants = boolValue; break;
                case "ProxyStrings": ProcessProxyStrings = boolValue; break;
                case "ProxyMethods": ProcessProxyMethods = boolValue; break;
                case "DelegateTypeRecovery": ProcessDelegateTypeRecovery = boolValue; break;
                case "ScriptingEnabled": ProcessScriptingEngine = boolValue; break;
                case "InMemoryDecompilation": ProcessInMemoryDecompilation = boolValue; break;
                case "JunkCodeRemoval": ProcessJunkCodeRemoval = boolValue; break;
                case "HiddenAssemblyReveal": ProcessHiddenAssemblyReveal = boolValue; break;
                case "AntiTamperChecks": ProcessAntiTamperChecks = boolValue; break;
                case "ScriptDirectory": ScriptDirectory = value; break;
                case "DefaultScript": DefaultScript = value; break;
                case "ScriptTimeout": if (int.TryParse(value, out int timeout)) ScriptTimeout = timeout; break;
                case "IntConfusion": ProcessIntConfusion = boolValue; break;
                case "Arithmetic": ProcessArithmetic = boolValue; break;
                case "StringEncryption": ProcessStringEncryption = boolValue; break;
                case "OnlineStringDecryption": ProcessOnlineStringDecryption = boolValue; break;
                case "ResourceEncryption": ProcessResourceEncryption = boolValue; break;
                case "ResourceProtections": ProcessResourceProtections = boolValue; break;
                case "StackUnfConfusion": ProcessStackUnfConfusion = boolValue; break;
                case "Callis": ProcessCallis = boolValue; break;
                case "InvalidMetadata": ProcessInvalidMetadata = boolValue; break;
                case "Local2Field": ProcessLocal2Field = boolValue; break;
                case "Renamer": ProcessRenamer = boolValue; break;
                case "InvalidCalls": RemoveInvalidCalls = boolValue; break;
                case "UselessJumps": RemoveUselessJumps = boolValue; break;
                case "UselessNOPs": RemoveUselessNOPs = boolValue; break;
                case "UnusedLocals": RemoveUnusedLocals = boolValue; break;
                case "UnusedVariables": RemoveUnusedVariables = boolValue; break;
                case "EnableBuiltInScripts": EnableBuiltInScripts = boolValue; break;
                case "EnableUserScripts": EnableUserScripts = boolValue; break;
                case "AutoLoadScripts": AutoLoadScripts = boolValue; break;
                case "AutoLoadScriptList": AutoLoadScriptList = value; break;
                case "LogToFile": LogToFile = boolValue; break;
                case "LogFileName": LogFileName = value; break;
            }
        }
    }
} 