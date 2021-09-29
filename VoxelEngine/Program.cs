using System;
using System.Windows.Forms;

namespace VoxelEngine
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormGame());
            //Application.Run(new FormAudio());
        }
    }
}
