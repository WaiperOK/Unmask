using System;
using System.Reflection;
using dnlib.DotNet;

namespace Unmask
{
    /// <summary>
    /// Системное ядро приложения Unmask
    /// </summary>
    public static class SystemCore
    {
        public static ModuleDefMD TargetModule;
        public static Assembly TargetAssembly;
        public static string TargetFilePath;
        public static ConfigurationManager Configuration = new ConfigurationManager();
        public static ProtectionPresets Presets = new ProtectionPresets();

        /// <summary>
        /// Инициализация системы
        /// </summary>
        public static void Initialize()
        {
            Configuration = new ConfigurationManager();
            Presets = new ProtectionPresets();

            // Попытка загрузить конфигурацию
            try 
            { 
                Configuration.LoadFromFile("unmask_config.txt"); 
            }
            catch 
            { 
                // Используем настройки по умолчанию
            }

            // Попытка загрузить пресеты
            try
            {
                if (System.IO.File.Exists(Configuration.PresetsFilePath))
                {
                    Presets.LoadFromFile(Configuration.PresetsFilePath);
                }
            }
            catch
            {
                // Используем пресеты по умолчанию
            }

            // Инициализация скриптинга
            try
            {
                ScriptingEngine.Initialize();
            }
            catch
            {
                // Игнорируем ошибки инициализации скриптов
            }

            if (Configuration.Language == "Russian")
            {
                LanguageManager.CurrentLanguage = LanguageManager.Language.Russian;
            }
            else
            {
                LanguageManager.CurrentLanguage = LanguageManager.Language.English;
            }

            var theme = ThemeManager.GetThemeFromName(Configuration.Theme);
            ThemeManager.SetTheme(theme);
        }

        /// <summary>
        /// Загрузка целевого файла для обработки
        /// </summary>
        public static void LoadTargetFile(string filePath)
        {
            TargetFilePath = System.IO.Path.GetFullPath(filePath);
            TargetAssembly = Assembly.LoadFile(TargetFilePath);
            TargetModule = ModuleDefMD.Load(TargetFilePath);
        }

        /// <summary>
        /// Проверка готовности системы к работе
        /// </summary>
        public static bool IsReady => TargetModule != null && TargetAssembly != null;

        public static void Cleanup()
        {
            TargetModule?.Dispose();
            TargetModule = null;
            TargetAssembly = null;
            TargetFilePath = null;
            // Очистка скриптов не требуется, так как они загружаются в память
        }
    }
} 