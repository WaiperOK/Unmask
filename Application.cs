using System;
using System.Windows.Forms;

namespace Unmask
{
    static class Application
    {
        /// <summary>
        /// Главная точка входа приложения Unmask
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Инициализация системы
            SystemCore.Initialize();

            // Настройка Windows Forms
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            // Запуск главного окна
            try
            {
                System.Windows.Forms.Application.Run(new UnmaskMainWindow());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Критическая ошибка приложения:\n{ex.Message}\n\nПриложение будет закрыто.", 
                    "Ошибка Unmask", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
} 