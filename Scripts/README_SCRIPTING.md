# 📜 Система скриптинга Unmask

## Обзор

Система скриптинга Unmask позволяет создавать и выполнять пользовательские скрипты деобфускации на C#. Это мощный инструмент для расширения функциональности деобфускатора под конкретные задачи.

## 🚀 Быстрый старт

### 1. Включение скриптинга

В файле `unmask_config.txt`:
```ini
ScriptingEngine=true
ScriptingEnabled=true
ScriptDirectory=Scripts
```

### 2. Создание простого скрипта

Создайте файл `MyScript.cs` в папке `Scripts`:

```csharp
using System;
using dnlib.DotNet;
using Unmask;

namespace CustomScripts
{
    public class MyScript : IDeobfuscationScript
    {
        public string Name => "MyScript";
        public string Description => "Мой первый скрипт";
        public string Version => "1.0";

        public void Execute(ModuleDefMD module, IScriptLogger logger)
        {
            logger.Info("Привет из моего скрипта!");
            
            // Ваша логика деобфускации здесь
            
            logger.Success("Скрипт выполнен успешно!");
        }
    }
}
```

## 📚 API Reference

### Интерфейс IDeobfuscationScript

```csharp
public interface IDeobfuscationScript
{
    string Name { get; }        // Имя скрипта
    string Description { get; } // Описание
    string Version { get; }     // Версия
    
    void Execute(ModuleDefMD module, IScriptLogger logger);
}
```

### Интерфейс IScriptLogger

```csharp
public interface IScriptLogger
{
    void Info(string message);     // Информационное сообщение
    void Warning(string message);  // Предупреждение
    void Error(string message);    // Ошибка
    void Success(string message);  // Успешное выполнение
}
```

## 🛠️ Примеры скриптов

### Удаление NOP инструкций

```csharp
public class RemoveNopsScript : IDeobfuscationScript
{
    public string Name => "RemoveNops";
    public string Description => "Удаление NOP инструкций";
    public string Version => "1.0";

    public void Execute(ModuleDefMD module, IScriptLogger logger)
    {
        int removedCount = 0;
        
        foreach (var type in module.Types)
        {
            foreach (var method in type.Methods)
            {
                if (!method.HasBody) continue;
                
                var instructions = method.Body.Instructions;
                for (int i = instructions.Count - 1; i >= 0; i--)
                {
                    if (instructions[i].OpCode == OpCodes.Nop)
                    {
                        instructions.RemoveAt(i);
                        removedCount++;
                    }
                }
            }
        }
        
        logger.Success($"Удалено {removedCount} NOP инструкций");
    }
}
```

### Расшифровка XOR строк

```csharp
public class XorStringDecryption : IDeobfuscationScript
{
    public string Name => "XorStringDecryption";
    public string Description => "Расшифровка XOR строк";
    public string Version => "1.0";

    public void Execute(ModuleDefMD module, IScriptLogger logger)
    {
        int decryptedCount = 0;
        byte xorKey = 0x42; // Ваш ключ
        
        foreach (var type in module.Types)
        {
            foreach (var method in type.Methods)
            {
                if (!method.HasBody) continue;
                
                foreach (var instruction in method.Body.Instructions)
                {
                    if (instruction.OpCode == OpCodes.Ldstr && 
                        instruction.Operand is string encryptedStr)
                    {
                        var decrypted = DecryptXor(encryptedStr, xorKey);
                        if (IsValidString(decrypted))
                        {
                            instruction.Operand = decrypted;
                            decryptedCount++;
                        }
                    }
                }
            }
        }
        
        logger.Success($"Расшифровано {decryptedCount} строк");
    }
    
    private string DecryptXor(string encrypted, byte key)
    {
        var result = new StringBuilder();
        foreach (char c in encrypted)
        {
            result.Append((char)(c ^ key));
        }
        return result.ToString();
    }
    
    private bool IsValidString(string str)
    {
        return !string.IsNullOrEmpty(str) && 
               str.All(c => !char.IsControl(c) || char.IsWhiteSpace(c));
    }
}
```

### Поиск и удаление мусорных методов

```csharp
public class JunkMethodRemoval : IDeobfuscationScript
{
    public string Name => "JunkMethodRemoval";
    public string Description => "Удаление мусорных методов";
    public string Version => "1.0";

    public void Execute(ModuleDefMD module, IScriptLogger logger)
    {
        var junkMethods = new List<MethodDef>();
        
        foreach (var type in module.Types)
        {
            foreach (var method in type.Methods)
            {
                if (IsJunkMethod(method))
                {
                    junkMethods.Add(method);
                }
            }
        }
        
        foreach (var junkMethod in junkMethods)
        {
            junkMethod.DeclaringType.Methods.Remove(junkMethod);
        }
        
        logger.Success($"Удалено {junkMethods.Count} мусорных методов");
    }
    
    private bool IsJunkMethod(MethodDef method)
    {
        if (!method.HasBody) return false;
        
        var instructions = method.Body.Instructions;
        
        // Метод содержит только return
        if (instructions.Count == 1 && instructions[0].OpCode == OpCodes.Ret)
            return true;
            
        // Метод содержит только pop + return
        if (instructions.Count == 2 && 
            instructions[0].OpCode == OpCodes.Pop &&
            instructions[1].OpCode == OpCodes.Ret)
            return true;
            
        // Метод с подозрительным именем
        if (method.Name.Length == 1 || method.Name.All(char.IsDigit))
            return true;
            
        return false;
    }
}
```

## ⚙️ Настройки скриптинга

### Конфигурация в unmask_config.txt

```ini
# === СИСТЕМА СКРИПТИНГА ===
ScriptingEngine=true              # Включить движок скриптов
ScriptingEnabled=true             # Разрешить выполнение скриптов
ScriptDirectory=Scripts           # Папка со скриптами
AutoLoadScripts=true              # Автозагрузка скриптов
EnableBuiltInScripts=true         # Встроенные скрипты
EnableUserScripts=true            # Пользовательские скрипты

# === НАСТРОЙКИ СКРИПТОВ ===
AutoLoadScriptList=BasicDeobfuscation,StringDecryption,MetadataRepair
ScriptTimeout=300                 # Таймаут выполнения (сек)
ScriptDebugOutput=true            # Отладочный вывод
AllowScriptCompilation=true       # Разрешить компиляцию
SandboxScripts=true               # Песочница для скриптов
```

### Встроенные скрипты

1. **BasicDeobfuscation** - Базовая деобфускация
2. **StringDecryption** - Расшифровка строк
3. **ControlFlowRecovery** - Восстановление потока управления
4. **MetadataRepair** - Восстановление метаданных

## 🔧 Продвинутые техники

### Работа с инструкциями

```csharp
// Поиск паттернов инструкций
for (int i = 0; i < instructions.Count - 2; i++)
{
    if (instructions[i].OpCode == OpCodes.Ldc_I4 &&
        instructions[i + 1].OpCode == OpCodes.Ldc_I4 &&
        instructions[i + 2].OpCode == OpCodes.Add)
    {
        // Найден паттерн: ldc.i4 + ldc.i4 + add
        var val1 = GetInt32Value(instructions[i]);
        var val2 = GetInt32Value(instructions[i + 1]);
        var result = val1 + val2;
        
        // Заменяем на одну инструкцию
        instructions[i] = Instruction.CreateLdcI4(result);
        instructions.RemoveAt(i + 2);
        instructions.RemoveAt(i + 1);
    }
}
```

### Анализ типов и методов

```csharp
// Поиск методов по сигнатуре
var suspiciousMethods = module.Types
    .SelectMany(t => t.Methods)
    .Where(m => m.Parameters.Count == 1 && 
                m.Parameters[0].Type.FullName == "System.String" &&
                m.ReturnType.FullName == "System.String")
    .ToList();

foreach (var method in suspiciousMethods)
{
    logger.Info($"Найден подозрительный метод: {method.FullName}");
}
```

### Работа с ресурсами

```csharp
// Обработка встроенных ресурсов
foreach (var resource in module.Resources.OfType<EmbeddedResource>())
{
    var data = resource.CreateReader().ToArray();
    
    if (IsEncryptedResource(data))
    {
        var decrypted = DecryptResource(data);
        var newResource = new EmbeddedResource(resource.Name, decrypted);
        
        var index = module.Resources.IndexOf(resource);
        module.Resources[index] = newResource;
        
        logger.Info($"Расшифрован ресурс: {resource.Name}");
    }
}
```

## 🛡️ Безопасность

### Песочница скриптов

При включенной опции `SandboxScripts=true`:
- Ограничен доступ к файловой системе
- Запрещены сетевые операции
- Ограничено время выполнения
- Контролируется использование памяти

### Валидация скриптов

```csharp
// Проверка безопасности скрипта перед выполнением
private bool IsScriptSafe(string scriptCode)
{
    var dangerousPatterns = new[]
    {
        "System.IO.File",
        "System.Net",
        "System.Diagnostics.Process",
        "System.Reflection.Assembly.Load"
    };
    
    return !dangerousPatterns.Any(pattern => 
        scriptCode.Contains(pattern));
}
```

## 📝 Лучшие практики

### 1. Структура скрипта

```csharp
namespace CustomScripts
{
    /// <summary>
    /// Описание скрипта
    /// </summary>
    public class MyScript : IDeobfuscationScript
    {
        public string Name => "MyScript";
        public string Description => "Подробное описание";
        public string Version => "1.0";

        public void Execute(ModuleDefMD module, IScriptLogger logger)
        {
            logger.Info($"Запуск {Name} v{Version}");
            
            try
            {
                // Основная логика
                ProcessModule(module, logger);
            }
            catch (Exception ex)
            {
                logger.Error($"Ошибка: {ex.Message}");
                throw;
            }
        }
        
        private void ProcessModule(ModuleDefMD module, IScriptLogger logger)
        {
            // Реализация
        }
    }
}
```

### 2. Обработка ошибок

```csharp
public void Execute(ModuleDefMD module, IScriptLogger logger)
{
    int processedCount = 0;
    int errorCount = 0;
    
    foreach (var type in module.Types)
    {
        try
        {
            ProcessType(type);
            processedCount++;
        }
        catch (Exception ex)
        {
            logger.Warning($"Ошибка обработки типа {type.Name}: {ex.Message}");
            errorCount++;
        }
    }
    
    logger.Info($"Обработано: {processedCount}, ошибок: {errorCount}");
}
```

### 3. Производительность

```csharp
// Используйте кэширование для повторяющихся операций
private readonly Dictionary<string, bool> _cache = new Dictionary<string, bool>();

private bool IsTargetMethod(MethodDef method)
{
    var key = method.FullName;
    if (_cache.TryGetValue(key, out var cached))
        return cached;
    
    var result = ExpensiveCheck(method);
    _cache[key] = result;
    return result;
}
```

## 🔍 Отладка скриптов

### Включение отладочного вывода

```ini
ScriptDebugOutput=true
LogLevel=Debug
```

### Отладочные сообщения

```csharp
public void Execute(ModuleDefMD module, IScriptLogger logger)
{
    logger.Info("=== НАЧАЛО ОТЛАДКИ ===");
    
    foreach (var type in module.Types)
    {
        logger.Info($"Обработка типа: {type.FullName}");
        logger.Info($"  Методов: {type.Methods.Count}");
        logger.Info($"  Полей: {type.Fields.Count}");
        
        // Детальная обработка...
    }
    
    logger.Info("=== КОНЕЦ ОТЛАДКИ ===");
}
```

## 📊 Примеры использования

### Автоматическая деобфускация

```bash
# Запуск с автоматическим выполнением скриптов
Unmask.exe input.exe output.exe --scripts=auto

# Запуск конкретных скриптов
Unmask.exe input.exe output.exe --scripts=MyScript,AnotherScript
```

### Интеграция в CI/CD

```yaml
# GitHub Actions пример
- name: Deobfuscate with custom scripts
  run: |
    Unmask.exe obfuscated.exe clean.exe --scripts=ProductionCleanup
```

---

**Система скриптинга Unmask** предоставляет мощные возможности для создания специализированных инструментов деобфускации! 🚀 