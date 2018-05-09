using System.Timers;

namespace myGame
{
    public class Game
    {
        public Map Map;
        public Player Player;

        public void Pause()
        {
            // nothing to do
        }
        
        public void Tick()
        {
            if (!Map.IsWallAt(Player.NextPosition))
                Player.TryMove();
        }
    }
}