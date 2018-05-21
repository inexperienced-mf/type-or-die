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
        private PictureBox pictureBox = new PictureBox();
        private Label pos;
        private TextBox textBox1 = new TextBox();
        private Label InputCommand;
        private Label RemainingCommand;

        public GameForm(string map)
        {
            game = new Game(
                new Map(map, 50),
                new Player(new Point { X = 75, Y = 121 }, 50, 10, 200, 50, new Dictionary<string, int>() { { "aaaa", 10 } }),
                new List<Enemy>() { new Enemy( new Point { X = 175, Y = 64 }, 100, 10, 50, 1, 1,
                new List<Point>() { new Point { X = 75, Y = 150 }, new Point { X = 175, Y = 150 }}, 50) },
                new Queue<Cast>(new Cast[] {
                    new Cast("abc", "Heart.png", 0, 0, 10, new Point(75, 150), 17),
                    new Cast("abc", "Heart.png", 0, 0, 10, new Point(75, 120), 17),
                    new Cast("abc", "Heart.png", 0, 0, 10, new Point(150, 150), 17) }));
            ClientSize = new Size(600, 600);
            timer = new Timer { Interval = 100 };
            pos = new Label() { Size = new Size(ClientSize.Width, 20) };
            InputCommand = new Label();
            RemainingCommand = new Label();
            InputCommand.Top = 25;
            InputCommand.ForeColor = Color.Black;
            RemainingCommand.ForeColor = Color.Gray;
            RemainingCommand.Top = 45;
            Controls.Add(pos);
            Controls.Add(InputCommand);
            Controls.Add(RemainingCommand);
            DirectoryInfo imagesDirectory = new DirectoryInfo("images");
            foreach (var e in imagesDirectory.GetFiles("*.png"))
                bitmaps[e.Name] = (Bitmap)Image.FromFile(e.FullName);
            pictureBox.Paint += new PaintEventHandler(PictureBoxPaint);
            Controls.Add(pictureBox);
            timer.Tick += (sender, args) => { game.Tick(); };
            timer.Tick += (sender, args) => { pictureBox.Invalidate(); };
            timer.Tick += (sender, args) => { pos.Invalidate(); };
            timer.Tick += (sender, args) => { UpdateStatus(); };
            timer.Start();
        }

        private void UpdateStatus()
        {
            pos.Text = game.Player.Position.ToString() + " " +
                game.Player.Life.ToString() + " " +
                (game.Enemies.Count != 0 ? game.Enemies
                    .Select(e => e.Life.ToString())
                    .Aggregate((x, y) => x + " " + y) : "");
            InputCommand.Text = game.Player.input;
            if (game.Player.SelectedCast == null)
                RemainingCommand.Text = game.Player.SelectedCommand;
            else
                RemainingCommand.Text = game.Player.SelectedCast.Command;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            HandleKey(e, true);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            HandleKey(e, false);

            if (!game.Player.IsBlocked)
            {
                if (e.KeyCode.ToString().Length == 1
                    && e.KeyCode.ToString()[0] >= 'A'
                    && e.KeyCode.ToString()[0] <= 'Z')
                {
                    game.Player.InputCommand(e.KeyCode.ToString().ToLower()[0], game.Enemies);
                }
            }
            else if (e.KeyCode == Keys.Space)
            {
                if (game.Player.Backpack.Count > 0)
                    game.Player.SelectCast();
            }
            else if (Choice.Contains(e.KeyCode))
            {
                game.Player.SelectCommand(Choice.IndexOf(e.KeyCode));
            }
        }

        private readonly List<Keys> Choice = new List<Keys>()
        { Keys.Q, Keys.W, Keys.E, Keys.R, Keys.T, Keys.Y };

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
                            i * game.Map.CellSize + 100, j * game.Map.CellSize + 100);
                    else
                        g.DrawImage(bitmaps["Grass.png"],
                            i * game.Map.CellSize + 100, j * game.Map.CellSize + 100);
                }
        }

        private void DrawEnemies(Graphics g)
        {
            foreach (var enemy in game.Enemies)
            {
                g.FillRectangle(
                    new SolidBrush(Color.Red),
                    new Rectangle(enemy.Position.X + 100 - enemy.Radius,
                        enemy.Position.Y + 100 - enemy.Radius,
                        enemy.Radius * 2 + 1, enemy.Radius * 2 + 1));
            }
        }

        private void DrawPlayer(Graphics g)
        {
            var attackRadius = game.Player.AttackRadius;
            var attackRadiusPoint1 = new Point(game.Player.Position.X - attackRadius + 100, game.Player.Position.Y + 100);
            var attackRadiusPoint2 = new Point(game.Player.Position.X + 100, game.Player.Position.Y + attackRadius + 100);
            var attackRadiusPoint3 = new Point(game.Player.Position.X + attackRadius + 100, game.Player.Position.Y + 100);
            var attackRadiusPoint4 = new Point(game.Player.Position.X + 100, game.Player.Position.Y - attackRadius + 100);
            g.DrawPolygon(new Pen(Color.LightSkyBlue), new PointF[] {
                attackRadiusPoint1, attackRadiusPoint2,
                attackRadiusPoint3, attackRadiusPoint4 });
            g.FillRectangle(
                new SolidBrush(Color.Blue),
                new Rectangle(game.Player.Position.X + 100 - game.Player.Radius,
                    game.Player.Position.Y + 100 - game.Player.Radius,
                    game.Player.Radius * 2 + 1, game.Player.Radius * 2 + 1));
        }

        private void PictureBoxPaint(object sender, PaintEventArgs e)
        {
            DoubleBuffered = true;
            var g = CreateGraphics();
            DrawMap(g);
            DrawEnemies(g);
            if (game.ThrowCast && game.Casts.Count != 0)
                g.DrawImage(bitmaps[game.Casts.Peek().ImageName], game.Casts.Peek().Position.X + 100, game.Casts.Peek().Position.Y + 100);
            DrawPlayer(g);
        }

        private void HandleKey(KeyEventArgs e, bool down)
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // GameForm
            // 
            this.ClientSize = new System.Drawing.Size(292, 212);
            this.Name = "GameForm";
            this.ResumeLayout(false);

        }
    }
}