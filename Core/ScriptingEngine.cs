using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using dnlib.DotNet;

namespace Unmask
{
    /// <summary>
    /// Интерфейс для пользовательских скриптов деобфускации
    /// </summary>
    public interface IDeobfuscationScript
    {
        /// <summary>
        /// Имя скрипта
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Описание скрипта
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Версия скрипта
        /// </summary>
        string Version { get; }
        
        /// <summary>
        /// Выполнение скрипта деобфускации
        /// </summary>
        /// <param name="module">Модуль для обработки</param>
        /// <param name="logger">Логгер для вывода сообщений</param>
        void Execute(ModuleDefMD module, IScriptLogger logger);
    }

    /// <summary>
    /// Интерфейс для логирования в скриптах
    /// </summary>
    public interface IScriptLogger
    {
        void Info(string message);
        void Warning(string message);
        void Error(string message);
        void Success(string message);
    }

    /// <summary>
    /// Движок выполнения пользовательских скриптов
    /// </summary>
    public static class ScriptingEngine
    {
        private static readonly Dictionary<string, IDeobfuscationScript> LoadedScripts = new Dictionary<string, IDeobfuscationScript>();
        private static readonly List<string> ScriptDirectories = new List<string> { "Scripts", "UserScripts", "CustomScripts" };

        /// <summary>
        /// Инициализация движка скриптов
        /// </summary>
        public static void Initialize()
        {
            try
            {
                LoadBuiltInScripts();
                LoadUserScripts();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка инициализации скриптов: {ex.Message}");
            }
        }

        /// <summary>
        /// Получение списка доступных скриптов
        /// </summary>
        public static List<string> GetAvailableScripts()
        {
            return LoadedScripts.Keys.ToList();
        }

        /// <summary>
        /// Получение информации о скрипте
        /// </summary>
        public static IDeobfuscationScript GetScript(string name)
        {
            LoadedScripts.TryGetValue(name, out var script);
            return script;
        }

        /// <summary>
        /// Выполнение скрипта
        /// </summary>
        public static void ExecuteScript(string scriptName, ModuleDefMD module, IScriptLogger logger = null)
        {
            try
            {
                if (LoadedScripts.TryGetValue(scriptName, out var script))
                {
                    logger?.Info($"Выполнение скрипта: {script.Name} v{script.Version}");
                    script.Execute(module, logger ?? new ConsoleLogger());
                    logger?.Success($"Скрипт {script.Name} выполнен успешно");
                }
                else
                {
                    logger?.Error($"Скрипт '{scriptName}' не найден");
                }
            }
            catch (Exception ex)
            {
                logger?.Error($"Ошибка выполнения скрипта '{scriptName}': {ex.Message}");
            }
        }

        /// <summary>
        /// Компиляция и загрузка скрипта из файла
        /// </summary>
        public static bool CompileAndLoadScript(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                var sourceCode = File.ReadAllText(filePath);
                var assembly = CompileScript(sourceCode);
                
                if (assembly != null)
                {
                    LoadScriptsFromAssembly(assembly);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка компиляции скрипта {filePath}: {ex.Message}");
            }
            
            return false;
        }

        /// <summary>
        /// Создание скриптов по умолчанию
        /// </summary>
        public static void CreateDefaultScripts()
        {
            try
            {
                var scriptsDir = "Scripts";
                if (!Directory.Exists(scriptsDir))
                    Directory.CreateDirectory(scriptsDir);

                // Создаем базовые скрипты, если их нет
                CreateDefaultScriptFile(scriptsDir, "CustomStringDecryption.cs", GetCustomStringDecryptionTemplate());
                CreateDefaultScriptFile(scriptsDir, "AdvancedVMRemoval.cs", GetAdvancedVMRemovalTemplate());
                CreateDefaultScriptFile(scriptsDir, "default_deobfuscation.cs", GetDefaultDeobfuscationTemplate());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка создания скриптов по умолчанию: {ex.Message}");
            }
        }

        /// <summary>
        /// Очистка скомпилированных скриптов
        /// </summary>
        public static void ClearCompiledScripts()
        {
            try
            {
                // Очищаем только пользовательские скрипты, оставляем встроенные
                var userScripts = LoadedScripts.Where(kvp => 
                    !IsBuiltInScript(kvp.Key)).ToList();

                foreach (var script in userScripts)
                {
                    LoadedScripts.Remove(script.Key);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка очистки скриптов: {ex.Message}");
            }
        }

        private static bool IsBuiltInScript(string scriptName)
        {
            var builtInScripts = new[] { "BasicDeobfuscation", "StringDecryption", "ControlFlowRecovery", "MetadataRepair" };
            return builtInScripts.Contains(scriptName);
        }

        private static void CreateDefaultScriptFile(string directory, string fileName, string content)
        {
            var filePath = Path.Combine(directory, fileName);
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, content);
            }
        }

        private static string GetCustomStringDecryptionTemplate()
        {
            return @"using System;
using System.Text;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Unmask;

public class CustomStringDecryption : IDeobfuscationScript
{
    public string Name => ""CustomStringDecryption"";
    public string Description => ""Расширенная расшифровка строк с поддержкой XOR, Base64 и ROT13"";
    public string Version => ""1.0"";

    public void Execute(ModuleDefMD module, IScriptLogger logger)
    {
        logger.Info(""Запуск расширенной расшифровки строк..."");
        
        int decryptedCount = 0;
        
        foreach (var type in module.Types)
        {
            foreach (var method in type.Methods)
            {
                if (method.Body?.Instructions == null) continue;
                
                for (int i = 0; i < method.Body.Instructions.Count; i++)
                {
                    var instruction = method.Body.Instructions[i];
                    
                    if (instruction.OpCode == OpCodes.Ldstr && instruction.Operand is string encryptedString)
                    {
                        string decrypted = TryDecryptString(encryptedString);
                        if (decrypted != null && decrypted != encryptedString)
                        {
                            instruction.Operand = decrypted;
                            decryptedCount++;
                        }
                    }
                }
            }
        }
        
        logger.Success($""Расшифровано {decryptedCount} строк"");
    }
    
    private string TryDecryptString(string encrypted)
    {
        // Попытка Base64
        try
        {
            var bytes = Convert.FromBase64String(encrypted);
            return Encoding.UTF8.GetString(bytes);
        }
        catch { }
        
        // Попытка XOR с ключом 42
        try
        {
            var result = new StringBuilder();
            foreach (char c in encrypted)
            {
                result.Append((char)(c ^ 42));
            }
            return result.ToString();
        }
        catch { }
        
        // Попытка ROT13
        try
        {
            var result = new StringBuilder();
            foreach (char c in encrypted)
            {
                if (char.IsLetter(c))
                {
                    char offset = char.IsUpper(c) ? 'A' : 'a';
                    result.Append((char)((c - offset + 13) % 26 + offset));
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }
        catch { }
        
        return null;
    }
}";
        }

        private static string GetAdvancedVMRemovalTemplate()
        {
            return @"using System;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Unmask;

public class AdvancedVMRemoval : IDeobfuscationScript
{
    public string Name => ""AdvancedVMRemoval"";
    public string Description => ""Продвинутое удаление виртуальных машин и восстановление методов"";
    public string Version => ""1.0"";

    public void Execute(ModuleDefMD module, IScriptLogger logger)
    {
        logger.Info(""Запуск продвинутого удаления виртуальных машин..."");
        
        int removedVMs = 0;
        int restoredMethods = 0;
        
        // Поиск и удаление VM типов
        var vmTypes = module.Types.Where(IsVMType).ToList();
        foreach (var vmType in vmTypes)
        {
            module.Types.Remove(vmType);
            removedVMs++;
        }
        
        // Восстановление виртуализированных методов
        foreach (var type in module.Types)
        {
            foreach (var method in type.Methods)
            {
                if (RestoreVirtualizedMethod(method))
                {
                    restoredMethods++;
                }
            }
        }
        
        logger.Success($""Удалено {removedVMs} VM типов, восстановлено {restoredMethods} методов"");
    }
    
    private bool IsVMType(TypeDef type)
    {
        var vmSignatures = new[] { ""VM_"", ""Virtual"", ""Handler"", ""Opcode"", ""Context"" };
        return vmSignatures.Any(sig => type.Name.IndexOf(sig, StringComparison.OrdinalIgnoreCase) >= 0);
    }
    
    private bool RestoreVirtualizedMethod(MethodDef method)
    {
        if (method.Body?.Instructions == null) return false;
        
        bool modified = false;
        var instructions = method.Body.Instructions;
        
        for (int i = 0; i < instructions.Count; i++)
        {
            var instruction = instructions[i];
            
            // Поиск VM вызовов и их замена
            if (instruction.OpCode == OpCodes.Call && IsVMCall(instruction))
            {
                // Заменяем VM вызов на простую инструкцию
                instructions[i] = Instruction.Create(OpCodes.Nop);
                modified = true;
            }
        }
        
        return modified;
    }
    
    private bool IsVMCall(Instruction instruction)
    {
        if (instruction.Operand is MethodDef method)
        {
            var vmSignatures = new[] { ""Execute"", ""Dispatch"", ""Handler"", ""VM_"" };
            return vmSignatures.Any(sig => method.Name.IndexOf(sig, StringComparison.OrdinalIgnoreCase) >= 0);
        }
        return false;
    }
}";
        }

        private static string GetDefaultDeobfuscationTemplate()
        {
            return @"using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Unmask;

public class DefaultDeobfuscation : IDeobfuscationScript
{
    public string Name => ""DefaultDeobfuscation"";
    public string Description => ""Стандартная деобфускация по умолчанию"";
    public string Version => ""1.0"";

    public void Execute(ModuleDefMD module, IScriptLogger logger)
    {
        logger.Info(""Запуск стандартной деобфускации..."");
        
        // Выполняем базовые операции деобфускации
        var basicScript = new BasicDeobfuscationScript();
        basicScript.Execute(module, logger);
        
        var stringScript = new StringDecryptionScript();
        stringScript.Execute(module, logger);
        
        var controlFlowScript = new ControlFlowRecoveryScript();
        controlFlowScript.Execute(module, logger);
        
        var metadataScript = new MetadataRepairScript();
        metadataScript.Execute(module, logger);
        
        logger.Success(""Стандартная деобфускация завершена"");
    }
}";
        }

        private static Assembly CompileScript(string sourceCode)
        {
            try
            {
                var provider = new CSharpCodeProvider();
                var parameters = new CompilerParameters
                {
                    GenerateInMemory = true,
                    GenerateExecutable = false,
                    IncludeDebugInformation = false
                };

                // Добавляем необходимые ссылки
                parameters.ReferencedAssemblies.Add("System.dll");
                parameters.ReferencedAssemblies.Add("System.Core.dll");
                parameters.ReferencedAssemblies.Add("dnlib.dll");
                parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);

                var results = provider.CompileAssemblyFromSource(parameters, sourceCode);
                
                if (results.Errors.HasErrors)
                {
                    foreach (CompilerError error in results.Errors)
                    {
                        Console.WriteLine($"Ошибка компиляции: {error.ErrorText}");
                    }
                    return null;
                }

                return results.CompiledAssembly;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка компиляции скрипта: {ex.Message}");
                return null;
            }
        }

        private static void LoadScriptsFromAssembly(Assembly assembly)
        {
            try
            {
                var scriptTypes = assembly.GetTypes()
                    .Where(t => typeof(IDeobfuscationScript).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var type in scriptTypes)
                {
                    var script = (IDeobfuscationScript)Activator.CreateInstance(type);
                    LoadedScripts[script.Name] = script;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки скриптов из сборки: {ex.Message}");
            }
        }

        private static void LoadBuiltInScripts()
        {
            // Загружаем встроенные скрипты
            LoadedScripts["BasicDeobfuscation"] = new BasicDeobfuscationScript();
            LoadedScripts["StringDecryption"] = new StringDecryptionScript();
            LoadedScripts["ControlFlowRecovery"] = new ControlFlowRecoveryScript();
            LoadedScripts["MetadataRepair"] = new MetadataRepairScript();
        }

        private static void LoadUserScripts()
        {
            foreach (var directory in ScriptDirectories)
            {
                if (Directory.Exists(directory))
                {
                    var scriptFiles = Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories);
                    foreach (var file in scriptFiles)
                    {
                        CompileAndLoadScript(file);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Консольный логгер для скриптов
    /// </summary>
    public class ConsoleLogger : IScriptLogger
    {
        public void Info(string message) => Console.WriteLine($"[INFO] {message}");
        public void Warning(string message) => Console.WriteLine($"[WARNING] {message}");
        public void Error(string message) => Console.WriteLine($"[ERROR] {message}");
        public void Success(string message) => Console.WriteLine($"[SUCCESS] {message}");
    }

    #region Встроенные скрипты

    /// <summary>
    /// Базовый скрипт деобфускации
    /// </summary>
    public class BasicDeobfuscationScript : IDeobfuscationScript
    {
        public string Name => "BasicDeobfuscation";
        public string Description => "Базовая деобфускация с удалением NOP инструкций и оптимизацией";
        public string Version => "1.0";

        public void Execute(ModuleDefMD module, IScriptLogger logger)
        {
            logger.Info("Запуск базовой деобфускации...");
            
            int removedNops = 0;
            int optimizedMethods = 0;

            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody) continue;

                    var instructions = method.Body.Instructions;
                    bool modified = false;

                    // Удаление NOP инструкций
                    for (int i = instructions.Count - 1; i >= 0; i--)
                    {
                        if (instructions[i].OpCode == dnlib.DotNet.Emit.OpCodes.Nop)
                        {
                            instructions.RemoveAt(i);
                            removedNops++;
                            modified = true;
                        }
                    }

                    if (modified)
                    {
                        optimizedMethods++;
                        method.Body.OptimizeBranches();
                        method.Body.OptimizeMacros();
                    }
                }
            }

            logger.Success($"Удалено {removedNops} NOP инструкций в {optimizedMethods} методах");
        }
    }

    /// <summary>
    /// Скрипт расшифровки строк
    /// </summary>
    public class StringDecryptionScript : IDeobfuscationScript
    {
        public string Name => "StringDecryption";
        public string Description => "Расширенная расшифровка строк с поддержкой различных алгоритмов";
        public string Version => "1.0";

        public void Execute(ModuleDefMD module, IScriptLogger logger)
        {
            logger.Info("Запуск расшифровки строк...");
            
            int decryptedStrings = 0;

            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody) continue;

                    var instructions = method.Body.Instructions;
                    for (int i = 0; i < instructions.Count; i++)
                    {
                        var instruction = instructions[i];
                        if (instruction.OpCode == dnlib.DotNet.Emit.OpCodes.Ldstr && 
                            instruction.Operand is string str)
                        {
                            // Попытка расшифровки Base64
                            try
                            {
                                var bytes = Convert.FromBase64String(str);
                                var decrypted = System.Text.Encoding.UTF8.GetString(bytes);
                                if (!string.IsNullOrEmpty(decrypted) && decrypted != str)
                                {
                                    instruction.Operand = decrypted;
                                    decryptedStrings++;
                                }
                            }
                            catch
                            {
                                // Не Base64, пропускаем
                            }
                        }
                    }
                }
            }

            logger.Success($"Расшифровано {decryptedStrings} строк");
        }
    }

    /// <summary>
    /// Скрипт восстановления потока управления
    /// </summary>
    public class ControlFlowRecoveryScript : IDeobfuscationScript
    {
        public string Name => "ControlFlowRecovery";
        public string Description => "Восстановление нормального потока управления";
        public string Version => "1.0";

        public void Execute(ModuleDefMD module, IScriptLogger logger)
        {
            logger.Info("Запуск восстановления потока управления...");
            
            int optimizedMethods = 0;

            foreach (var type in module.Types)
            {
                foreach (var method in type.Methods)
                {
                    if (!method.HasBody) continue;

                    var instructions = method.Body.Instructions;
                    bool modified = false;

                    // Удаление избыточных переходов
                    for (int i = instructions.Count - 1; i >= 0; i--)
                    {
                        var instruction = instructions[i];
                        if (instruction.OpCode == dnlib.DotNet.Emit.OpCodes.Br && 
                            instruction.Operand is dnlib.DotNet.Emit.Instruction target)
                        {
                            if (i + 1 < instructions.Count && instructions[i + 1] == target)
                            {
                                instructions.RemoveAt(i);
                                modified = true;
                            }
                        }
                    }

                    if (modified)
                    {
                        optimizedMethods++;
                        method.Body.OptimizeBranches();
                    }
                }
            }

            logger.Success($"Оптимизировано {optimizedMethods} методов");
        }
    }

    /// <summary>
    /// Скрипт восстановления метаданных
    /// </summary>
    public class MetadataRepairScript : IDeobfuscationScript
    {
        public string Name => "MetadataRepair";
        public string Description => "Восстановление поврежденных метаданных";
        public string Version => "1.0";

        public void Execute(ModuleDefMD module, IScriptLogger logger)
        {
            logger.Info("Запуск восстановления метаданных...");
            
            int repairedTypes = 0;
            int repairedMethods = 0;

            foreach (var type in module.Types)
            {
                // Восстановление имен типов
                if (string.IsNullOrEmpty(type.Name.String) || type.Name.String.All(char.IsDigit))
                {
                    type.Name = $"RestoredType_{type.MDToken.Raw:X8}";
                    repairedTypes++;
                }

                foreach (var method in type.Methods)
                {
                    // Восстановление имен методов
                    if (string.IsNullOrEmpty(method.Name.String) || method.Name.String.All(char.IsDigit))
                    {
                        method.Name = $"RestoredMethod_{method.MDToken.Raw:X8}";
                        repairedMethods++;
                    }

                    // Настройка KeepOldMaxStack для избежания ошибок
                    if (method.HasBody && method.Body != null)
                    {
                        method.Body.KeepOldMaxStack = true;
                    }
                }
            }

            logger.Success($"Восстановлено {repairedTypes} типов и {repairedMethods} методов");
        }
    }

    #endregion
} 