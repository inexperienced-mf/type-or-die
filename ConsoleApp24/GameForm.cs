using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace myGame
{
    internal class GameForm : Form
    {
        private readonly Dictionary<string, Bitmap> bitmaps = new Dictionary<string, Bitmap>();
        private Game game;
        private readonly Timer timer;
        private Label pos;
        private Label InputCommand;
        private Label RemainingCommand;

        public GameForm(Map map, Player player, List<Enemy> enemies)
        {
            game = new Game(map, player, enemies);
            ClientSize = new Size(850, 600);
            timer = new Timer { Interval = 5 };
            pos = new Label() { Size = new Size(ClientSize.Width, 20) };
            InputCommand = new Label() { Top = 25, ForeColor = Color.Black };
            RemainingCommand = new Label() { Top = 45, ForeColor = Color.Gray};
            Controls.Add(pos);
            Controls.Add(InputCommand);
            Controls.Add(RemainingCommand);
            DirectoryInfo imagesDirectory = new DirectoryInfo("images");
            foreach (var e in imagesDirectory.GetFiles("*.png"))
                bitmaps[e.Name] = (Bitmap)Image.FromFile(e.FullName);
            timer.Tick += (sender, args) =>
            {
                game.Tick();
                Invalidate();
                UpdateStatus();
            };
            FormBorderStyle = FormBorderStyle.FixedSingle;
            DoubleBuffered = true;
            timer.Start();
        }

        private void UpdateStatus()
        {
            pos.Text = game.Player.Position + " " +
                game.Player.HealthPoints + " " +
                (game.Enemies.Count != 0 ? game.Enemies
                    .Select(e => e.HealthPoints.ToString())
                    .Aggregate((x, y) => x + " " + y) : string.Empty);
            var caster = game.Player.Caster;
            if (caster.State == State.Typing)
            {
                InputCommand.Text = caster.SelectedCast.Command;
                RemainingCommand.Text = caster.SelectedCast.Command.Substring(0, caster.CurrentChar);
            }
            else
                InputCommand.Text = RemainingCommand.Text = string.Empty;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            HandleMoving(e, true);
            if (e.KeyCode == Keys.Escape)
                game.IsPaused = !game.IsPaused;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            game.Player.Caster.Register(e.KeyChar);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            HandleMoving(e, false);
        }

        private readonly Dictionary<Keys, Direction> translate = new Dictionary<Keys, Direction>
        {
            {Keys.Up, Direction.Up},
            {Keys.Down, Direction.Down},
            {Keys.Left, Direction.Left},
            {Keys.Right, Direction.Right},
        };

        private void DrawMap(Graphics g)
        {
            for (var i = 0; i < game.Map.WidthInCells; i++)
                for (var j = 0; j < game.Map.HeightInCells; j++)
                {
                    if (game.Map.CellIsWall[i, j])
                        g.DrawImage(bitmaps["Obstacle.png"],
                            i * game.Map.CellSize + 10, j * game.Map.CellSize + 70);
                    else
                        g.DrawImage(bitmaps["Grass.png"],
                            i * game.Map.CellSize + 10, j * game.Map.CellSize + 70);
                }
        }

        private void DrawEnemies(Graphics g)
        {
            foreach (var enemy in game.Enemies)
            {
                g.FillRectangle(
                    new SolidBrush(Color.Red),
                    new Rectangle(enemy.Position.X + 10 - enemy.Radius,
                        enemy.Position.Y + 70 - enemy.Radius,
                        enemy.Radius * 2 + 1, enemy.Radius * 2 + 1));
            }
        }

        private void DrawPlayer(Graphics g)
        {
            g.FillRectangle(
                new SolidBrush(Color.Blue),
                new Rectangle(game.Player.Position.X + 10 - game.Player.Radius,
                    game.Player.Position.Y + 70 - game.Player.Radius,
                    game.Player.Radius * 2 + 1, game.Player.Radius * 2 + 1));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            DrawMap(g);
            DrawEnemies(g);
            DrawPlayer(g);
        }

        private void HandleMoving(KeyEventArgs e, bool down)
        {
            if (translate.ContainsKey(e.KeyCode))
            {
                if (down)
                {
                    if (game.Player.WantedDirection == Direction.None)
                        game.Player.WantedDirection = translate[e.KeyCode];
                }
                else if (game.Player.WantedDirection == translate[e.KeyCode])
                    game.Player.WantedDirection = Direction.None;
            }
        }
    }
}





//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Windows.Forms;
//
//namespace myGame
//{
//    internal class GameForm : Form
//    {
//        private readonly Dictionary<string, Bitmap> bitmaps = new Dictionary<string, Bitmap>();
//        private Game game;
//        private readonly Timer timer;
//        private PictureBox pictureBox = new PictureBox();
//        private Label pos;
//        private TextBox textBox1 = new TextBox();
//        private Label InputCommand;
//        private Label RemainingCommand;
//
//        public GameForm(string map)
//        {
//            game = new Game(
//                new Map(map, 50),
//                new Player(new Point { X = 75, Y = 121 }, 50, 10, 200, 50, new Dictionary<string, int> { { "aaaa", 10 } }),
//                new List<Enemy>
//                { new Enemy( new Point { X = 175, Y = 64 }, 100, 10, 50, 1, 1,
//                new List<Point> { new Point { X = 75, Y = 150 }, new Point { X = 175, Y = 150 }}, 50) },
//                new Queue<Cast>(new[] {
//                    new Cast("abc", 0, 0, 10, new Point(75, 150), 17),
//                    new Cast("abc", 0, 0, 10, new Point(75, 120), 17),
//                    new Cast("abc", 0, 0, 10, new Point(150, 150), 17) }));
//            ClientSize = new Size(600, 600);
//            timer = new Timer { Interval = 100 };
//            pos = new Label { Size = new Size(ClientSize.Width, 20) };
//            InputCommand = new Label();
//            RemainingCommand = new Label();
//            InputCommand.Top = 25;
//            InputCommand.ForeColor = Color.Black;
//            RemainingCommand.ForeColor = Color.Gray;
//            RemainingCommand.Top = 45;
//            Controls.Add(pos);
//            Controls.Add(InputCommand);
//            Controls.Add(RemainingCommand);
//            DirectoryInfo imagesDirectory = new DirectoryInfo("images");
//            foreach (var e in imagesDirectory.GetFiles("*.png"))
//                bitmaps[e.Name] = (Bitmap)Image.FromFile(e.FullName);
//            pictureBox.Paint += PictureBoxPaint;
//            Controls.Add(pictureBox);
//            pictureBox.CreateGraphics();
//            timer.Tick += (sender, args) =>
//            {
//                game.Tick();
//                pictureBox.Invalidate();
//                pos.Invalidate();
//                UpdateStatus();
//            };
//            timer.Start();
//        }
//
//        private void UpdateStatus()
//        {
//            pos.Text = game.Player.Position + " " +
//                game.Player.HealthPoints + " " +
//                (game.Enemies.Count != 0 ? game.Enemies
//                    .Select(e => e.HealthPoints.ToString())
//                    .Aggregate((x, y) => x + " " + y) : string.Empty);
//            var caster = game.Player.Caster;
//            if (caster.State == State.Typing)
//            {
//                InputCommand.Text = caster.SelectedCast.Command;
//                RemainingCommand.Text = caster.SelectedCast.Command.Substring(0, caster.CurrentChar);
//            }
//            else
//                InputCommand.Text = RemainingCommand.Text = string.Empty;
//        }
//
//        protected override void OnKeyDown(KeyEventArgs e)
//        {
//            base.OnKeyDown(e);
//            HandleKey(e, true);
//        }
//
//        protected override void OnKeyUp(KeyEventArgs e)
//        {
//            base.OnKeyUp(e);
//            HandleKey(e, false);
//        }
//
//        private readonly HashSet<Keys> Choice = new HashSet<Keys> { Keys.Q, Keys.W, Keys.E, Keys.R, Keys.T, Keys.Y };
//
//        private readonly Dictionary<Keys, Direction> translate = new Dictionary<Keys, Direction>
//        {
//            {Keys.Up, Direction.Up},
//            {Keys.Down, Direction.Down},
//            {Keys.Left, Direction.Left},
//            {Keys.Right, Direction.Right}
//        };
//        
//        private void HandleKey(KeyEventArgs e, bool down)
//        {
//            if (down && e.KeyCode == Keys.Escape)
//                game.IsPaused = !game.IsPaused;
//            if (down && Choice.Contains(e.KeyCode))
//                game.Player.Caster.Register(e.KeyCode.ToString()[0]);
//            if (translate.ContainsKey(e.KeyCode))
//            {
//                if (down)
//                {
//                    if (game.Player.WantedDirection == Direction.None)
//                        game.Player.WantedDirection = translate[e.KeyCode];
//                }
//                else if (game.Player.WantedDirection == translate[e.KeyCode])
//                    game.Player.WantedDirection = Direction.None;
//            }
//        }
//
//        private void DrawMap(Graphics g)
//        {
//            for (var i = 0; i < game.Map.WidthInCells; i++)
//                for (var j = 0; j < game.Map.HeightInCells; j++)
//                {
//                    if (game.Map.CellIsWall[i, j])
//                        g.DrawImage(bitmaps["Obstacle.png"],
//                            i * game.Map.CellSize + 100, j * game.Map.CellSize + 100);
//                    else
//                        g.DrawImage(bitmaps["Grass.png"],
//                            i * game.Map.CellSize + 100, j * game.Map.CellSize + 100);
//                }
//        }
//
//        private void DrawEnemies(Graphics g)
//        {
//            foreach (var enemy in game.Enemies)
//            {
//                g.FillRectangle(
//                    new SolidBrush(Color.Red),
//                    new Rectangle(enemy.Position.X + 100 - enemy.Radius,
//                        enemy.Position.Y + 100 - enemy.Radius,
//                        enemy.Radius * 2 + 1, enemy.Radius * 2 + 1));
//            }
//        }
//
//        private void DrawPlayer(Graphics g)
//        {
//            g.FillRectangle(
//                new SolidBrush(Color.Blue),
//                new Rectangle(game.Player.Position.X + 100 - game.Player.Radius,
//                    game.Player.Position.Y + 100 - game.Player.Radius,
//                    game.Player.Radius * 2 + 1, game.Player.Radius * 2 + 1));
//        }
//
//        private void PictureBoxPaint(object sender, PaintEventArgs e)
//        {
//            var g = e.Graphics;
//            DrawMap(g);
//            DrawEnemies(g);
//            DrawPlayer(g);
//        }
//
//        private void InitializeComponent()
//        {
//            SuspendLayout();
//            // 
//            // GameForm
//            // 
//            ClientSize = new Size(292, 212);
//            Name = "GameForm";
//            ResumeLayout(false);
//
//        }
//    }
//}