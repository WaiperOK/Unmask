using System;
using System.IO;

namespace Unmask
{
    /// <summary>
    /// Консольная версия приложения Unmask
    /// </summary>
    class ConsoleApplication
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Unmask Console - Advanced .NET Deobfuscator");
            Console.WriteLine("============================================");
            Console.WriteLine();

            if (args.Length < 2)
            {
                ShowUsage();
                return;
            }

            string inputFile = args[0];
            string outputFile = args[1];

            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"Ошибка: Входной файл '{inputFile}' не найден!");
                return;
            }

            try
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Загрузка модуля: {Path.GetFileName(inputFile)}");
                
                // Инициализация системы
                SystemCore.Initialize();
                
                // Загрузка целевого файла
                SystemCore.LoadTargetFile(inputFile);
                
                // Создание процессора защит
                var processor = new ProtectionProcessor();
                
                // Создание флагов защит на основе конфигурации
                var protectionFlags = CreateProtectionFlags();
                
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Применение {GetProtectionCount()} защит...");
                
                // Применение всех включенных защит
                processor.ProcessProtections(SystemCore.TargetModule, protectionFlags);
                
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Обработка защит завершена");
                
                // Сохранение результата
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Сохранение результата в: {outputFile}");
                SystemCore.TargetModule.Write(outputFile);
                
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Обработка завершена успешно!");
                Console.WriteLine($"Результат сохранен: {outputFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Произошла ошибка при обработке: {ex.Message}");
                Console.WriteLine($"Детали ошибки: {ex}");
            }
            finally
            {
                SystemCore.Cleanup();
            }
        }

        static ProtectionFlags CreateProtectionFlags()
        {
            var config = SystemCore.Configuration;
            return new ProtectionFlags
            {
                AntiTamper = config.ProcessAntiTamper,
                AntiDump = config.ProcessAntiDump,
                AntiDebug = config.ProcessAntiDebug,
                AntiDe4Dot = config.ProcessAntiDe4Dot,
                Watermarks = config.ProcessWatermarks,
                LocalField = config.ProcessLocal2Field,
                JumpControlFlow = config.ProcessJumpControlFlow,
                ControlFlow = config.ProcessControlFlow,
                ProxyConstants = config.ProcessProxyConstants,
                ProxyStrings = config.ProcessProxyStrings,
                ProxyMethods = config.ProcessProxyMethods,
                IntConfusion = config.ProcessIntConfusion,
                Arithmetic = config.ProcessArithmetic,
                EncryptedStrings = config.ProcessStringEncryption,
                OnlineStringDecryption = config.ProcessOnlineStringDecryption,
                ResourceEncryption = config.ProcessResourceEncryption,
                ResourceProtections = config.ProcessResourceProtections,
                StackUnfConfusion = config.ProcessStackUnfConfusion,
                Callis = config.ProcessCallis,
                InvalidMetadata = config.ProcessInvalidMetadata,
                VirtualMachines = config.ProcessVirtualMachines,
                DataStructureRecovery = config.ProcessDataStructureRecovery,
                JunkCodeRemoval = config.ProcessJunkCodeRemoval,
                Renamer = config.ProcessRenamer,
                ScriptingEngine = config.ProcessScriptingEngine
            };
        }

        static void ShowUsage()
        {
            Console.WriteLine("Использование:");
            Console.WriteLine("  Unmask.Console.exe <входной_файл> <выходной_файл>");
            Console.WriteLine();
            Console.WriteLine("Примеры:");
            Console.WriteLine("  Unmask.Console.exe obfuscated.exe deobfuscated.exe");
            Console.WriteLine("  Unmask.Console.exe protected.dll clean.dll");
            Console.WriteLine();
            Console.WriteLine("Поддерживаемые форматы: .exe, .dll");
        }

        static int GetProtectionCount()
        {
            int count = 0;
            var config = SystemCore.Configuration;
            
            if (config.ProcessAntiTamper) count++;
            if (config.ProcessAntiDump) count++;
            if (config.ProcessAntiDebug) count++;
            if (config.ProcessAntiDe4Dot) count++;
            if (config.ProcessWatermarks) count++;
            if (config.ProcessLocal2Field) count++;
            if (config.ProcessJumpControlFlow) count++;
            if (config.ProcessControlFlow) count++;
            if (config.ProcessProxyConstants) count++;
            if (config.ProcessProxyStrings) count++;
            if (config.ProcessProxyMethods) count++;
            if (config.ProcessIntConfusion) count++;
            if (config.ProcessArithmetic) count++;
            if (config.ProcessStringEncryption) count++;
            if (config.ProcessOnlineStringDecryption) count++;
            if (config.ProcessResourceEncryption) count++;
            if (config.ProcessStackUnfConfusion) count++;
            if (config.ProcessCallis) count++;
            if (config.ProcessInvalidMetadata) count++;
            if (config.ProcessVirtualMachines) count++;
            if (config.ProcessDataStructureRecovery) count++;
            if (config.ProcessDelegateTypeRecovery) count++;
            if (config.ProcessRenamer) count++;
            if (config.ProcessJunkCodeRemoval) count++;
            
            return count;
        }
    }
} 