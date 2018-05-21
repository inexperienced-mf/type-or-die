using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Timers;

namespace myGame
{
    public class Game
    {
        private Timer CastTimer = new Timer { Interval = 50000 };
        public bool ThrowCast;
        public Map Map;
        public Player Player;
        public List<Enemy> Enemies = new List<Enemy>();
        public Queue<Cast> Casts = new Queue<Cast>();
        public bool End { get; private set; }

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
            if (Casts.Count != 0 && Player.GetDistanceToTarget(Casts.Peek().Position, Player.Position) < Casts.Peek().Size)
            {
                Player.Backpack.Enqueue(Casts.FirstOrDefault());
                Casts.Dequeue();
                ThrowCast = false;
                CastTimer.Start();
            }
        }

        public Game (Map map, Player player, List<Enemy> enemies, Queue<Cast> casts)
        {
            Map = map;
            Player = player;
            Enemies = enemies;
            Casts = casts;
            CastTimer.Elapsed += (sender, args) => ThrowCast = true;
            CastTimer.AutoReset = false;
            CastTimer.Start();
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
            if (Enemies.Count == 0)
                End = true;
        }
    }
}