using System.Collections.Generic;
using System.Windows.Forms;

namespace myGame
{
    internal class GameForm: Form
    {
        private Game game;
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            HandleKey(e, true);
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