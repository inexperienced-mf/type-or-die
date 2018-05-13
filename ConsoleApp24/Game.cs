using System.Collections.Generic;
using System.Timers;

namespace myGame
{
    public class Game
    {
        public Map Map;
        public Player Player;
        public List<Enemy> Enemies = new List<Enemy>();

        public void Pause()
        {
            // nothing to do
        }
        
        public void Tick()
        {
            if (!Map.IsWallAt(Player.NextPosition))
                Player.TryMove();
            foreach (var enemy in Enemies)
            {
                if (enemy.Life <= 0)
                    continue;
                enemy.Move(Player, Map);
            }
        }
    }
}