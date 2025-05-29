using System;
using System.Collections.Generic;
using System.Globalization;

namespace Unmask
{
    public static class LanguageManager
    {
        public enum Language
        {
            English,
            Russian
        }

        private static Language _currentLanguage = Language.English;
        private static Dictionary<string, Dictionary<Language, string>> _translations;

        static LanguageManager()
        {
            InitializeTranslations();
        }

        public static Language CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                _currentLanguage = value;
                LanguageChanged?.Invoke();
            }
        }

        public static event Action LanguageChanged;

        public static string GetString(string key)
        {
            if (_translations.TryGetValue(key, out var translations))
            {
                if (translations.TryGetValue(_currentLanguage, out var translation))
                {
                    return translation;
                }
            }
            return key;
        }

        private static void InitializeTranslations()
        {
            _translations = new Dictionary<string, Dictionary<Language, string>>
            {
                ["MainWindow_Title"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Unmask - Advanced .NET Deobfuscator",
                    [Language.Russian] = "Unmask - Продвинутый деобфускатор .NET"
                },
                ["File_Select"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Select File",
                    [Language.Russian] = "Выбрать файл"
                },
                ["File_Browse"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Browse...",
                    [Language.Russian] = "Обзор..."
                },
                ["Protection_Settings"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Protection Settings",
                    [Language.Russian] = "Настройки защит"
                },
                ["Select_All"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Select All",
                    [Language.Russian] = "Выбрать все"
                },
                ["Deselect_All"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Deselect All",
                    [Language.Russian] = "Снять все"
                },
                ["Start_Processing"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Start Processing",
                    [Language.Russian] = "Начать обработку"
                },
                ["Settings"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Settings",
                    [Language.Russian] = "Настройки"
                },
                ["Language"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Language",
                    [Language.Russian] = "Язык"
                },
                ["Language_EN_RU"] = new Dictionary<Language, string>
                {
                    [Language.English] = "EN/RU",
                    [Language.Russian] = "EN/RU"
                },
                ["Theme"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Theme",
                    [Language.Russian] = "Тема"
                },
                ["Progress"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Progress",
                    [Language.Russian] = "Прогресс"
                },
                ["Log"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Operation Log",
                    [Language.Russian] = "Журнал операций"
                },
                ["AntiTamper"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Anti-Tamper",
                    [Language.Russian] = "Анти-модификация"
                },
                ["AntiDump"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Anti-Dump",
                    [Language.Russian] = "Анти-дамп"
                },
                ["AntiDebug"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Anti-Debug",
                    [Language.Russian] = "Анти-отладка"
                },
                ["StringEncryption"] = new Dictionary<Language, string>
                {
                    [Language.English] = "String Encryption",
                    [Language.Russian] = "Шифрование строк"
                },
                ["ControlFlow"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Control Flow",
                    [Language.Russian] = "Поток управления"
                },
                ["VirtualMachines"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Virtual Machines",
                    [Language.Russian] = "Виртуальные машины"
                },
                ["DataStructureRecovery"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Data Structure Recovery",
                    [Language.Russian] = "Восстановление структур данных"
                },
                ["JunkCodeRemoval"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Junk Code Removal",
                    [Language.Russian] = "Удаление мусорного кода"
                },
                ["DelegateTypeRecovery"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Delegate Type Recovery",
                    [Language.Russian] = "Восстановление типов-делегатов"
                },
                ["HiddenAssemblyReveal"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Hidden Assembly Reveal",
                    [Language.Russian] = "Раскрытие скрытых сборок"
                },
                ["InMemoryDecompilation"] = new Dictionary<Language, string>
                {
                    [Language.English] = "In-Memory Decompilation",
                    [Language.Russian] = "Декомпиляция в памяти"
                },
                ["ScriptingEngine"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Scripting Engine",
                    [Language.Russian] = "Модуль скриптинга"
                },
                ["AntiTamperChecks"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Anti-Tamper Checks",
                    [Language.Russian] = "Проверки анти-tamper"
                },
                ["Processing_Complete"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Processing completed successfully!",
                    [Language.Russian] = "Обработка завершена успешно!"
                },
                ["Error_Occurred"] = new Dictionary<Language, string>
                {
                    [Language.English] = "An error occurred during processing",
                    [Language.Russian] = "Произошла ошибка во время обработки"
                },
                ["File_Not_Selected"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Please select a file first",
                    [Language.Russian] = "Сначала выберите файл"
                },
                ["Advanced_Settings"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Advanced Settings",
                    [Language.Russian] = "Расширенные настройки"
                },
                ["General"] = new Dictionary<Language, string>
                {
                    [Language.English] = "General",
                    [Language.Russian] = "Общие"
                },
                ["Protection_Modules"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Protection Modules",
                    [Language.Russian] = "Модули защит"
                },
                ["Scripting_Options"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Scripting Options",
                    [Language.Russian] = "Настройки скриптинга"
                },
                ["Output_Options"] = new Dictionary<Language, string>
                {
                    [Language.English] = "Output Options",
                    [Language.Russian] = "Настройки вывода"
                }
            };
        }
    }
} 