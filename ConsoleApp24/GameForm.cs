using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace myGame
{
    internal class GameForm : Form
    {
        private Game game;
        private readonly Timer timer;
        private PictureBox pictureBox = new PictureBox();

        public GameForm(string map)
        {
            game = new Game()
            {
                Map = new Map(map, 50),
                Player = new Player(new Point(74, 75), 20, 10),
                Enemies = new List<Enemy>() { new Enemy(new Point(74, 100), 50, 10, 20, 1, 1,
                new List<Point>() { new Point(74, 80), new Point(174, 120) }, 10) }
            };
            ClientSize = new Size(600, 600);
            timer = new Timer { Interval = 10 };
            var pos = new Label { Size = new Size(ClientSize.Width, 30) };
            Controls.Add(pos);
            DoubleBuffered = true;
            pictureBox.Paint += new PaintEventHandler(PictureBoxPaint);
            Controls.Add(pictureBox);
            timer.Tick += (sender, args) => { game.Tick(); };
            timer.Tick += (sender, args) => { pictureBox.Invalidate(); };
            timer.Tick += (sender, args) => { pos.Text = game.Player.Position.ToString() + " " + 
                game.Enemies[0].Position.ToString() + "  " + game.Player.Life.ToString(); };
            timer.Start();
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
        }

        private readonly Dictionary<Keys, Direction> translate = new Dictionary<Keys, Direction>
        {
            {Keys.Up, Direction.Up},
            {Keys.Down, Direction.Down},
            {Keys.Left, Direction.Left},
            {Keys.Right, Direction.Right},
        };

        private void PictureBoxPaint(object sender, PaintEventArgs e)
        {
            var g = CreateGraphics();
            for (var i = 0; i < game.Map.WidthInCells; i++)
                for (var j = 0; j < game.Map.HeightInCells; j++)
                {
                    if (game.Map.CellIsWall[i, j])
                        g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(i * game.Map.CellSize + 100,
                            j * game.Map.CellSize + 100, game.Map.CellSize, game.Map.CellSize));
                    else
                        g.FillRectangle(new SolidBrush(Color.White), new Rectangle(i * game.Map.CellSize + 100,
                            j * game.Map.CellSize + 100, game.Map.CellSize, game.Map.CellSize));
                }
            foreach (var enemy in game.Enemies)
                g.FillRectangle(new SolidBrush(Color.Red), new Rectangle(enemy.Position.X + 100 - enemy.Size / 2 + 1,
                    enemy.Position.Y + 100 - enemy.Size / 2 + 1, enemy.Size, enemy.Size));
            g.FillRectangle(new SolidBrush(Color.Blue), new Rectangle(game.Player.Position.X + 100 - game.Player.Size / 2 + 1,
                    game.Player.Position.Y + 100 - game.Player.Size / 2 + 1, game.Player.Size, game.Player.Size));
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
    }
}