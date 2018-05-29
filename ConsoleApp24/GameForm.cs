using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace myGame
{
    internal class GameForm : Form
    {
        private readonly Dictionary<string, Bitmap> bitmaps = new Dictionary<string, Bitmap>();
        private Game game;
        private readonly Timer timer;
        private int currentLevel;
        private bool isPaused { get; set; }

        public GameForm(List<Game> levels)
        {
            currentLevel = 0;
            if (levels.Count != 0)
                game = levels[0];
            ClientSize = new Size(850, 600);
            timer = new Timer { Interval = 5 };
            DirectoryInfo imagesDirectory = new DirectoryInfo("images");
            foreach (var e in imagesDirectory.GetFiles("*.png"))
                bitmaps[e.Name] = (Bitmap)Image.FromFile(e.FullName);
            timer.Tick += (sender, args) =>
            {
                if (!isPaused)
                    game.Tick();
                if (game.IsWon)
                    game = levels[++currentLevel];
                Invalidate();
            };
            FormBorderStyle = FormBorderStyle.FixedSingle;
            DoubleBuffered = true;
            timer.Start();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!isPaused) HandleMoving(e, true);
            if (e.KeyCode == Keys.Escape) 
                isPaused = !isPaused;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if (!isPaused)
                game.Player.Caster.Register(e.KeyChar);
        }


        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (!isPaused) HandleMoving(e, false);
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
            var xShift = 10;
            var yShift = 70;
            for (var i = 0; i < game.Map.WidthInCells; i++)
                for (var j = 0; j < game.Map.HeightInCells; j++)
                {
                    if (game.Map.CellIsWall[i, j])
                        g.DrawImage(bitmaps["Obstacle.png"], new Rectangle(
                            i * game.Map.CellSize + xShift, j * game.Map.CellSize + yShift,
                            game.Map.CellSize, game.Map.CellSize));
                    else
                        g.DrawImage(bitmaps["Grass.png"],
                            new Rectangle(
                            i * game.Map.CellSize + xShift, j * game.Map.CellSize + yShift,
                            game.Map.CellSize, game.Map.CellSize));
                }
            foreach (var color in game.Map.CheckedPoints)
                foreach (var point in color.Value)
                    g.DrawRectangle(
                       new Pen(new SolidBrush(color.Key), 5),
                       new Rectangle(
                           point.X * game.Map.CellSize + xShift, point.Y * game.Map.CellSize + yShift,
                           game.Map.CellSize, game.Map.CellSize));
            g.DrawEllipse(Pens.DarkBlue, 
                new Rectangle(game.Exit.X - game.ExitRadius, 
                    game.Exit.Y - game.ExitRadius, 
                    game.ExitRadius * 2, 
                    game.ExitRadius * 2));
        }

        private void DrawEnemies(Graphics g)
        {
            var xShift = 10;
            var yShift = 70;
            foreach (var enemy in game.Enemies)
            {
                g.FillRectangle(
                    new SolidBrush(Color.Red),
                    new Rectangle(enemy.Position.X + xShift - enemy.Radius,
                        enemy.Position.Y + yShift - enemy.Radius,
                        enemy.Radius * 2 + 1, enemy.Radius * 2 + 1));
                DrawHpStatus(g, enemy.HealthPoints, enemy.FullHealthPoints, enemy.Position, enemy.Radius);
            }
        }

        private void DrawBag(Graphics g)
        {
            var yShift = 80;
            var xShift = 10;
            for (var i = 0; i < game.Player.Caster.BagSize; i++)
            {
                var rect = new Rectangle(xShift * (i + 1) + game.Map.CellSize * i,
                        game.Map.HeightInCells * game.Map.CellSize + yShift,
                        game.Map.CellSize, game.Map.CellSize);
                g.FillRectangle(
                    new SolidBrush(Color.Black), rect);
                if (game.Player.Caster.Bag.Count > i)
                {
                    var itemCommand = game.Player.Caster.Bag[i].Command;
                    g.DrawImage(bitmaps[itemCommand + ".png"], rect);
                }
            }
        }

        private void DrawHpStatus(Graphics g, int hp, int fullHp, Point position, int radius)
        {
            var xShift = 10;
            var yShift = 65;
            var hpColor = new Color();
            if (hp > 0.7 * fullHp)
                hpColor = Color.LimeGreen;
            else hpColor = (hp > 0.35 * fullHp) ? Color.Orange : Color.Red;
            g.FillRectangle(
                new SolidBrush(Color.Black),
                new Rectangle(position.X + xShift - radius,
                    position.Y + yShift - radius,
                    radius * 2 + 1, 3));
            g.FillRectangle(
                new SolidBrush(hpColor),
                new Rectangle(position.X + xShift - radius,
                    position.Y + yShift - radius,
                    (radius * 2 + 1) * hp / fullHp, 3));
        }

        private void DrawPlayer(Graphics g)
        {
            var xSift = 10;
            var yShift = 70;
            g.FillRectangle(
                new SolidBrush(Color.Blue),
                new Rectangle(game.Player.Position.X + xSift - game.Player.Radius,
                    game.Player.Position.Y + yShift - game.Player.Radius,
                    game.Player.Radius * 2 + 1, game.Player.Radius * 2 + 1));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            DrawMap(g);
            DrawEnemies(g);
            DrawPlayer(g);
            DrawBag(g);
            DrawStatusBar(g);
            DrawCasts(g);
        }

        private void DrawCasts(Graphics g)
        {
            var castSize = game.Map.CellSize / 3;
            var xShift = 10;
            var yShift = 70;
            foreach (var cast in game.DroppedCasts)
            {
                var rect = new Rectangle(cast.Key.X - castSize / 2 + xShift,
                                        cast.Key.Y - castSize / 2 + yShift,
                                        castSize, castSize);
                g.DrawImage(bitmaps[cast.Value.Command + ".png"], rect);
            }
        }

        private void DrawStatusBar(Graphics g)
        {
            var xShift = 10;
            var hpColor = new Color();
            if (game.Player.HealthPoints > 0.7 * game.Player.FullHealthPoints)
                hpColor = Color.LimeGreen;
            else hpColor = (game.Player.HealthPoints > 0.35 * game.Player.FullHealthPoints) ? Color.Orange : Color.Red;
            g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(10, 20, 100, 30));
            g.FillRectangle(new SolidBrush(hpColor), new Rectangle(10, 20,
                    100 * game.Player.HealthPoints / game.Player.FullHealthPoints, 30));
            var font = new Font("Arial", 18);
            g.DrawString(game.Player.HealthPoints.ToString(), font, new SolidBrush(Color.Black),
               new Point(100 + xShift, 21));

            if (game.Player.Caster.State == State.Typing)
            {
                var yShift = 80;
                var castPosition = new Point(xShift, game.Map.CellSize * (game.Map.HeightInCells + 1) + yShift);
                g.DrawString(game.Player.Caster.SelectedCast.Command, font, new SolidBrush(Color.Gray), castPosition);
                g.DrawString(game.Player.Caster.SelectedCast.Command.Substring(0, game.Player.Caster.CurrentChar),
                    font, new SolidBrush(Color.Black), castPosition);
            }
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