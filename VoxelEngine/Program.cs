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
            // Строчка для масштаба
            SetProcessDPIAware();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //FormGame form = new FormGame
            //{
            //    Size = new System.Drawing.Size(1280, 720)
            //};
            //Application.Run(form);

            //Application.Run(new FormAudio());
            Application.Run(new Network.FormNetwork());
            
        }


        // Метод для масштаба
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
