using System;
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
        public bool IsEnd { get; private set; }
        public bool IsPaused = true;


        private void ActPlayer()
        {
            var leftUpAngle = new Point(Player.NextPosition.X - Player.Radius, Player.NextPosition.Y - Player.Radius);
            var rightDownAngle = new Point(Player.NextPosition.X + Player.Radius, Player.NextPosition.Y + Player.Radius);
            var leftDownAngle = new Point(Player.NextPosition.X - Player.Radius, Player.NextPosition.Y + Player.Radius);
            var rightUpAngle = new Point(Player.NextPosition.X + Player.Radius, Player.NextPosition.Y - Player.Radius);
            if (!Map.IsWallAt(leftDownAngle) && !Map.IsWallAt(leftUpAngle) &&
                !Map.IsWallAt(rightDownAngle) && !Map.IsWallAt(rightUpAngle))
                Player.TryMove();
            Player.Caster.TryInvoke(this);
        }

        public Game (Map map, Player player, List<Enemy> enemies)
        {
            Map = map;
            Player = player;
            Enemies = enemies;
        }

        private void ActEnemies()
        {
            Enemies = Enemies.Where(e => e.HealthPoints > 0).ToList();
            foreach (var enemy in Enemies)
                enemy.TryAct(Player, Map);
        }
        
        public void Tick()
        {
            if (IsPaused)
                return;
            if (Player.HealthPoints > 0)
            {
                ActPlayer();
                ActEnemies();
            }
        }
    }
}