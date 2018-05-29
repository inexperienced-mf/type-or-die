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
            var map1 =
                @"
@@@@@
@...@
@.@.@
@...@
@@@@@";
            var level1 = new Game
            {
                Map = new Map(map1, 70),
                Player = new Player(new Point(105, 105), 2, 10, 150, 6),
                Enemies = new List<Enemy> {new Enemy(new Point(245, 245), 3, 10, 60, 20, 30,
                    new List<Point>() {new Point(245, 245), new Point(245, 105)},
                    70, 15, 60)},
                Exit = new Point(245, 245)
            };

            var map2 = @"
@@@@@@@@@
@..@.@..@
@@.@...@@
@@...@..@
@@@@@@@@@";
            var level2 = new Game
            {
                Map = new Map(map2, 130),
                Player = new Player(new Point(160, 160), 2, 10, 150, 6),
                Enemies = new List<Enemy> {new Enemy(new Point(325, 455), 3, 10, 60, 20, 30,
                    new List<Point>() {new Point(325, 455), new Point(585, 455), new Point(585, 195)},
                    70, 15, 60)},
                Exit = new Point(975, 195)
            };
            
            var levels = new List<Game>{level1, level2};
            var form = new GameForm(levels);
            
            Application.Run(form);
        }
    }
}