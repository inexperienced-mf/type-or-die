using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        private void MovePlayer()
        {
            var leftUpAngle = new Point(Player.NextPosition.X - Player.Radius, Player.NextPosition.Y - Player.Radius);
            var rightDownAngle = new Point(Player.NextPosition.X + Player.Radius, Player.NextPosition.Y + Player.Radius);
            var leftDownAngle = new Point(Player.NextPosition.X - Player.Radius, Player.NextPosition.Y + Player.Radius);
            var rightUpAngle = new Point(Player.NextPosition.X + Player.Radius, Player.NextPosition.Y - Player.Radius);
            if (!Map.IsWallAt(leftDownAngle) && !Map.IsWallAt(leftUpAngle) &&
                !Map.IsWallAt(rightDownAngle) && !Map.IsWallAt(rightUpAngle))
                Player.TryMove();
        }

        private void MoveEnemies()
        {
            Enemies = Enemies.Where(e => e.Life > 0).ToList();
            foreach (var enemy in Enemies)
            {
                enemy.TryMove(Player, Map);
            }
        }
        
        public void Tick()
        {
            if (Player.Life > 0)
            {
                MovePlayer();
                MoveEnemies();
            }
        }
    }
}