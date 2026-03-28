using System;
using System.Windows.Forms;
using ShahtyorGame.forms;

namespace ShahtyorGame
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles(); //включаем визуал стили Windows
            
            Application.SetCompatibleTextRenderingDefault(false); //режим отображения текста
            
            Application.Run(new MainForm()); // запуск главной формы
        }
    }
}