using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
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
            var player = new Player(new Point(75, 75), 6, 10, 150, 6);
            var enemy1 = new Enemy(new Point(175, 175), 8, 10, 40, 20, 30,
                new List<Point>(){new Point(175, 175), new Point(175, 75)},
                70, 15, 60);

            var form = new GameForm(new Map(map, 50), player, new List<Enemy>(){enemy1});
            Application.Run(form);
        }
    }
}