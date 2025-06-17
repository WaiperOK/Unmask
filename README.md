# Unmask - Профессиональный .NET Деобфускатор

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.7.2-blue.svg)
![Version](https://img.shields.io/badge/version-2.0.0-green.svg)

Unmask - это мощный деобфускатор для .NET приложений с поддержкой пользовательских скриптов и современным графическим интерфейсом.

## ✨ Основные возможности

### 🛡️ Защиты, которые может удалить Unmask:
- **Anti-Tamper** - Защита от изменения
- **Anti-Debug** - Защита от отладки  
- **Anti-Dump** - Защита от дампа памяти
- **String Encryption** - Шифрование строк
- **Control Flow** - Запутывание потока управления
- **Proxy Methods** - Прокси-методы
- **Integer Confusion** - Запутывание целых чисел
- **Junk Code** - Мусорный код
- **Virtual Machines** - Виртуальные машины
- **Resource Protection** - Защита ресурсов
- **Metadata Obfuscation** - Обфускация метаданных

### 🚀 Дополнительные функции:
- **Scripting Engine** - Поддержка пользовательских скриптов на C#
- **GUI и Console** - Графический и консольный интерфейсы
- **Multi-language** - Поддержка русского и английского языков
- **Themes** - Темы оформления (Light, Dark, Hacker)
- **Automatic Backup** - Автоматическое резервное копирование
- **Detailed Logging** - Подробное логирование операций

## 🚀 Быстрый старт

### Требования
- Windows 10/11
- .NET Framework 4.7.2 или выше
- Visual Studio 2019+ (для разработки)

### Компиляция
```batch
# Компиляция всех версий
build_all.bat

# Или вручную
dotnet build Unmask.csproj --configuration Release
dotnet build Unmask.Console.csproj --configuration Release
```

### Запуск GUI версии
```batch
# После компиляции
bin\Release\net472\Unmask.exe
```

### Запуск Console версии
```batch
# Базовое использование
bin\Release\net472\Unmask.Console.exe input.exe

# С дополнительными параметрами
bin\Release\net472\Unmask.Console.exe input.exe -output deobfuscated.exe -all

# Только определенные защиты
bin\Release\net472\Unmask.Console.exe input.exe -antidebug -strings -controlflow
```

## 📋 Параметры командной строки

```
Unmask.Console.exe <input_file> [options]

Опции:
  -output <file>          Выходной файл (по умолчанию: input_deobfuscated.exe)
  -all                   Включить все защиты
  -antitamper            Обработать Anti-Tamper
  -antidebug             Обработать Anti-Debug
  -antidump              Обработать Anti-Dump
  -strings               Расшифровать строки
  -controlflow           Восстановить поток управления
  -proxy                 Удалить прокси-методы
  -integers              Исправить запутывание целых чисел
  -junk                  Удалить мусорный код
  -vm                    Удалить виртуальные машины
  -resources             Восстановить ресурсы
  -metadata              Исправить метаданные
  -script <name>         Выполнить пользовательский скрипт
  -config <file>         Использовать файл конфигурации
  -verbose              Подробный вывод
```

## 📝 Пример использования

### 1. Создание тестового файла
```batch
# Компилируем тестовый обфусцированный файл
dotnet build TestObfuscated.csproj --configuration Release
```

### 2. Деобфускация через GUI
1. Запустите `Unmask.exe`
2. Выберите файл `TestObfuscated.exe`
3. Отметьте нужные защиты
4. Нажмите "Обработать"

### 3. Деобфускация через консоль
```batch
# Полная деобфускация
Unmask.Console.exe TestObfuscated.exe -all -output TestObfuscated_clean.exe

# Только строки и поток управления
Unmask.Console.exe TestObfuscated.exe -strings -controlflow
```

## 🔧 Пользовательские скрипты

Unmask поддерживает пользовательские скрипты на C#. Примеры в папке `Scripts/`:

### Создание скрипта
```csharp
using System;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using Unmask;

namespace CustomScripts
{
    public class MyCustomScript : IDeobfuscationScript
    {
        public string Name => "MyScript";
        public string Description => "Описание моего скрипта";
        public string Version => "1.0";

        public void Execute(ModuleDefMD module, IScriptLogger logger)
        {
            logger.Info("Выполнение пользовательского скрипта...");
            
            // Ваш код деобфускации здесь
            
            logger.Success("Скрипт выполнен успешно!");
        }
    }
}
```

### Встроенные скрипты
- `BasicDeobfuscation` - Базовая деобфускация
- `StringDecryption` - Расширенная расшифровка строк
- `ControlFlowRecovery` - Восстановление потока управления
- `MetadataRepair` - Восстановление метаданных
- `AdvancedVMRemoval` - Удаление виртуальных машин

## ⚙️ Конфигурация

Файл `unmask_config.txt` содержит настройки приложения:
```ini
# Основные настройки
Language=Russian
Theme=Dark
AutoBackup=true
DeveloperMode=false

# Настройки защит
ProcessAntiTamper=true
ProcessAntiDebug=true
ProcessStringEncryption=true
ProcessControlFlow=true

# Настройки скриптов
ScriptingEnabled=true
ScriptDirectory=Scripts
DefaultScript=default_deobfuscation.cs
```

## 🎨 Темы оформления

Unmask поддерживает три темы:
- **Light** - Светлая тема
- **Dark** - Темная тема (по умолчанию)
- **Hacker** - Тема в стиле терминала

## 📊 Результаты работы

После деобфускации вы получите:
- Деобфусцированный исполняемый файл
- Лог-файл с подробной информацией (`unmask.log`)
- Резервную копию оригинального файла (если включено)

## 🛠️ Разработка

### Структура проекта
```
Unmask/
├── Core/                  # Основная логика
│   ├── ProtectionProcessor.cs    # Обработчик защит
│   ├── ScriptingEngine.cs        # Движок скриптов
│   ├── SystemCore.cs             # Системное ядро
│   └── ...
├── UI/                    # Пользовательский интерфейс
│   ├── UnmaskMainWindow.cs       # Главное окно
│   ├── SettingsWindow.cs         # Окно настроек
│   └── ...
├── Scripts/               # Пользовательские скрипты
└── Properties/            # Свойства сборки
```

### Добавление новой защиты
1. Добавьте флаг в `ProtectionFlags.cs`
2. Реализуйте логику в `ProtectionProcessor.cs`
3. Добавьте UI элемент в `UnmaskMainWindow.cs`
4. Обновите конфигурацию в `ConfigurationManager.cs`

## 🤝 Содействие

Мы приветствуем вклад в развитие проекта:
1. Fork репозитория
2. Создайте feature branch
3. Внесите изменения
4. Создайте Pull Request

## 📄 Лицензия

Этот проект лицензируется под MIT License - подробности в файле [LICENSE](LICENSE).

## ⚠️ Дисклеймер

Unmask предназначен только для законных целей, таких как:
- Анализ собственного кода
- Исследования в области безопасности
- Обучение и образование

Использование для незаконного взлома программного обеспечения запрещено.

## 📞 Поддержка

- 📧 Email: support@unmask.project
- 💬 Issues: GitHub Issues
- 📖 Wiki: GitHub Wiki

---

**Unmask** - Ваш надежный инструмент для анализа .NET приложений! 🛡️