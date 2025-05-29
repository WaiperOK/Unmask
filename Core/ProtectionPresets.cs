using System;
using System.IO;
using System.Text;

namespace Unmask
{
    /// <summary>
    /// Пресеты защит для деобфускации
    /// </summary>
    public class ProtectionPresets
    {
        // Ссылки на классы и методы
        public class MethodReference
        {
            public MethodReference(string namespaceName, string className, string methodName)
            {
                Namespace = namespaceName;
                Class = className;
                Method = methodName;
            }

            public string Namespace { get; set; }
            public string Class { get; set; }
            public string Method { get; set; }
        }

        public class ClassReference
        {
            public ClassReference(string namespaceName, string className)
            {
                Namespace = namespaceName;
                Class = className;
            }

            public string Namespace { get; set; }
            public string Class { get; set; }
        }

        // Anti-Protection пресеты
        public ClassReference AntiTamperException = new ClassReference("System", "BadImageFormatException");
        public MethodReference AntiDumpMarshal = new MethodReference("System.Runtime.InteropServices", "Marshal", "GetHINSTANCE");
        public MethodReference AntiDebugPlatform = new MethodReference("System", "OperatingSystem", "get_Platform");
        public MethodReference AntiDebugExit = new MethodReference("System", "Environment", "Exit");
        public MethodReference AntiDebugEnvironment = new MethodReference("System", "Environment", "GetEnvironmentVariable");
        public MethodReference AntiDebugOSVersion = new MethodReference("System", "Environment", "get_OSVersion");
        public MethodReference AntiDebugAttached = new MethodReference("System.Diagnostics", "Debugger", "get_IsAttached");
        public MethodReference AntiDebugLogging = new MethodReference("System.Diagnostics", "Debugger", "IsLogging");

        // Watermark пресеты
        public string WatermarkSignature = "obfuscator.signature";

        // Mathematical пресеты
        public ClassReference MathClass = new ClassReference("System", "Math");
        public MethodReference MathTruncate = new MethodReference("System", "Math", "Truncate");
        public MethodReference MathAbs = new MethodReference("System", "Math", "Abs");
        public MethodReference MathLog = new MethodReference("System", "Math", "Log");
        public MethodReference MathLog10 = new MethodReference("System", "Math", "Log10");
        public MethodReference MathFloor = new MethodReference("System", "Math", "Floor");
        public MethodReference MathRound = new MethodReference("System", "Math", "Round");
        public MethodReference MathTan = new MethodReference("System", "Math", "Tan");
        public MethodReference MathTanh = new MethodReference("System", "Math", "Tanh");
        public MethodReference MathSqrt = new MethodReference("System", "Math", "Sqrt");
        public MethodReference MathCeiling = new MethodReference("System", "Math", "Ceiling");
        public MethodReference MathCos = new MethodReference("System", "Math", "Cos");
        public MethodReference MathSin = new MethodReference("System", "Math", "Sin");

        // Integer Confusion пресеты
        public double IntConfusionDefault = 1.5707963267949;

        // Proxy пресеты
        public string ProxyCommonName = "ProxyMeth";

        // String Encryption пресеты
        public string OnlineDecryptionURL = "https://communitykeyv1.000webhostapp.com/Decoder4.php?string=";
        public ClassReference StringKeyAlgorithm = new ClassReference("System.Security.Cryptography", "Rfc2898DeriveBytes");
        public ClassReference StringAESAlgorithm = new ClassReference("System.Security.Cryptography", "RijndaelManaged");
        public ClassReference StringSymmetricAlgorithm = new ClassReference("System.Security.Cryptography", "SymmetricAlgorithm");
        public string StringPasswordHash = "p7K95451qB88sZ7J";
        public byte[] StringSalt = Encoding.Default.GetBytes("2GM23j301t60Z96T");
        public byte[] StringIV = Encoding.Default.GetBytes("IzTdhG6S8uwg141S");
        public string StringResourceName;

        // Control Flow пресеты
        public string[] ControlFlowPatterns = {
            "switch_", "case_", "default_", "goto_", "label_"
        };

        // Renamer пресеты
        public string[] RenamerPatterns = {
            "Class", "Method", "Field", "Property", "Event", "Namespace"
        };

        // Resource Protection пресеты
        public string[] ResourceProtectionSignatures = {
            "encrypted_", "protected_", "obfuscated_", "hidden_"
        };

        // Proxy Methods пресеты
        public string[] ProxyMethodPatterns = {
            "Invoke_", "Call_", "Execute_", "Run_", "Process_"
        };

        /// <summary>
        /// Загрузка пресетов из файла
        /// </summary>
        public void LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return;

            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//")) continue;

                ProcessPresetLine(line);
            }
        }

        /// <summary>
        /// Сохранение пресетов в файл
        /// </summary>
        public void SaveToFile(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("[AntiProtection]");
                writer.WriteLine($"AntiTamperException = {AntiTamperException.Namespace}.{AntiTamperException.Class}");
                writer.WriteLine($"AntiDumpMarshal = {AntiDumpMarshal.Namespace}.{AntiDumpMarshal.Class}.{AntiDumpMarshal.Method}");
                writer.WriteLine($"AntiDebugAttached = {AntiDebugAttached.Namespace}.{AntiDebugAttached.Class}.{AntiDebugAttached.Method}");
                writer.WriteLine();

                writer.WriteLine("[Watermarks]");
                writer.WriteLine($"WatermarkSignature = {WatermarkSignature}");
                writer.WriteLine();

                writer.WriteLine("[Mathematics]");
                writer.WriteLine($"IntConfusionDefault = {IntConfusionDefault}");
                writer.WriteLine($"MathClass = {MathClass.Namespace}.{MathClass.Class}");
                writer.WriteLine();

                writer.WriteLine("[Proxies]");
                writer.WriteLine($"ProxyCommonName = {ProxyCommonName}");
                writer.WriteLine();

                writer.WriteLine("[StringEncryption]");
                writer.WriteLine($"OnlineDecryptionURL = {OnlineDecryptionURL}");
                writer.WriteLine($"StringPasswordHash = {StringPasswordHash}");
                writer.WriteLine($"StringSalt = {Convert.ToBase64String(StringSalt)}");
                writer.WriteLine($"StringIV = {Convert.ToBase64String(StringIV)}");
                writer.WriteLine();

                writer.WriteLine("[ControlFlow]");
                writer.WriteLine($"ControlFlowPatterns = {string.Join(",", ControlFlowPatterns)}");
                writer.WriteLine();

                writer.WriteLine("[Renamer]");
                writer.WriteLine($"RenamerPatterns = {string.Join(",", RenamerPatterns)}");
                writer.WriteLine();

                writer.WriteLine("[ResourceProtection]");
                writer.WriteLine($"ResourceProtectionSignatures = {string.Join(",", ResourceProtectionSignatures)}");
                writer.WriteLine();

                writer.WriteLine("[ProxyMethods]");
                writer.WriteLine($"ProxyMethodPatterns = {string.Join(",", ProxyMethodPatterns)}");
            }
        }

        private void ProcessPresetLine(string line)
        {
            string[] parts = line.Split('=');
            if (parts.Length != 2) return;

            string key = parts[0].Trim();
            string value = parts[1].Trim();

            switch (key)
            {
                case "WatermarkSignature":
                    WatermarkSignature = value;
                    break;
                case "IntConfusionDefault":
                    if (double.TryParse(value, out double doubleValue))
                        IntConfusionDefault = doubleValue;
                    break;
                case "ProxyCommonName":
                    ProxyCommonName = value;
                    break;
                case "OnlineDecryptionURL":
                    OnlineDecryptionURL = value;
                    break;
                case "StringPasswordHash":
                    StringPasswordHash = value;
                    break;
                case "StringSalt":
                    try { StringSalt = Convert.FromBase64String(value); } catch { }
                    break;
                case "StringIV":
                    try { StringIV = Convert.FromBase64String(value); } catch { }
                    break;
                case "ControlFlowPatterns":
                    ControlFlowPatterns = value.Split(',');
                    break;
                case "RenamerPatterns":
                    RenamerPatterns = value.Split(',');
                    break;
                case "ResourceProtectionSignatures":
                    ResourceProtectionSignatures = value.Split(',');
                    break;
                case "ProxyMethodPatterns":
                    ProxyMethodPatterns = value.Split(',');
                    break;
            }
        }
    }
} 