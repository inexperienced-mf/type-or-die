using System;
using System.Windows.Forms;

namespace myGame
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
//            Application.EnableVisualStyles();
//            Application.SetCompatibleTextRenderingDefault(false);
            var map = "@@@@@\r\n@...@\r\n@.@.@\r\n@...@\r\n@@@@@";
            var form = new GameForm(map);
            Application.Run(form);
        }
    }
}
