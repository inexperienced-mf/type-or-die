using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace myGame
{
    public class Game
    {
        public Point Exit;
        public Map Map;
        public Player Player;
        public List<Enemy> Enemies = new List<Enemy>();
        public Dictionary<Point, Cast> Casts = new Dictionary<Point, Cast>();
        public bool? IsWon { get; private set; }
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
            var playerField = new Rectangle(Player.Position.X - Player.Radius, Player.Position.Y - Player.Radius,
                Player.Radius * 2 + 1, Player.Radius * 2 + 1);
            var position = default(Point);
            foreach (var castPos in Casts.Keys)
                if (playerField.Contains(castPos) && Player.Caster.TryPickUp(Casts[castPos]))
                {
                    position = castPos;
                    break;
                }
            Casts.Remove(position);
            if (Map.CheckedPoints
                .SelectMany(points => points.Value)
                .Contains(Map.InWhichCell(Player.Position)))
                IsWon = true;
        }

        public Game(Map map, Player player, List<Enemy> enemies, Point exit)
        {
            Map = map;
            Player = player;
            Enemies = enemies;
            Exit = exit;
            IsWon = null;
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
                CastGenerator.TryAddCast(Map, Casts);
            }
            else IsWon = false;
        }

    }

    class CastGenerator
    {
        public static TickTimer AddTimer = new TickTimer(200);
        private static readonly List<Cast> Casts = new List<Cast>() { Cast.AttackNearest, Cast.Heal, Cast.CheckNeighbourhood, Cast.ParalizeEnemies };
        public static void TryAddCast(Map map, Dictionary<Point, Cast> casts)
        {
            AddTimer.Tick();
            if (AddTimer.IsReady)
            {
                var rnd = new Random();
                var cast = Casts[rnd.Next(0, Casts.Count)];
                var position = map.FindFreePlace(map.CellSize / 3);
                if (!casts.ContainsKey(position))
                    casts.Add(position, cast);
                AddTimer.Interval *= 2;
                AddTimer.Restart();
            }
        }
    }
}