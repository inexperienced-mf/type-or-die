using System.Timers;

namespace myGame
{
    public class Game
    {
        public Map Map;
        public Player Player;
        public readonly Timer timer;
        public bool playerCanMove;

        public void Pause()
        {
            // what to do??
        }
        
        public void TimerTick()
        {
            if (Player.CanMove && !Map.IsWallAt(Player.NextPosition))
                Player.Move();
        }
    }
}