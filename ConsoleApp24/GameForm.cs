using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace myGame
{
    internal class GameForm: Form
    {
        private Game game;
        private readonly Timer timer;
        
        public GameForm(string map)
        {
            game = new Game() {Map = new Map(map, 10), Player = new Player(new Point(15, 15), 200)};
            timer = new Timer {Interval = 10};
            var pos = new Label {Size = new Size(ClientSize.Width, 30)};
            Controls.Add(pos);
            timer.Tick += (sender, args) => { game.Tick(); };
            timer.Tick += (sender, args) => { pos.Text = game.Player.Position.ToString(); };
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


        private void HandleKey(KeyEventArgs e, bool down)
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