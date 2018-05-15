using System.Collections.Generic;
using System.Drawing;
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
            var dif = (Player.Size % 2 == 0) ? 1 : 0;
            var thickness = (Player.Size % 2 == 1) ? (Player.Size - 1) / 2 : Player.Size / 2;
            var leftUpAngle = new Point(Player.NextPosition.X - thickness + dif, Player.NextPosition.Y - thickness + dif);
            var rightDownAngle = new Point(Player.NextPosition.X + thickness, Player.NextPosition.Y + thickness);
            var leftDownAngle = new Point(Player.NextPosition.X - thickness + dif, Player.NextPosition.Y + thickness);
            var rightUpAngle = new Point(Player.NextPosition.X + thickness, Player.NextPosition.Y - thickness + dif);
            if (!Map.IsWallAt(leftDownAngle) && !Map.IsWallAt(leftUpAngle) && !Map.IsWallAt(rightDownAngle) && !Map.IsWallAt(rightUpAngle))
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