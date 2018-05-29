﻿using System;
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
        public List<Enemy> Enemies;
        public readonly Dictionary<Point, Cast> DroppedCasts = new Dictionary<Point, Cast>();
        public bool IsWon { get; private set; } = false;
        public bool IsLost { get; set; }
        public int ExitRadius = 5;

//        public Game(Map map, Player player, List<Enemy> enemies, Point exit)
//        {
//            Map = map;
//            Player = player;
//            Enemies = enemies;
//            Exit = exit;
//        }

        private void ActPlayer()
        {
            MovePlayer();
            Player.Caster.TryInvoke(this);
            PickUpCasts();
            if (Math.Abs(Player.Position.X - Exit.X) + Math.Abs(Player.Position.Y - Exit.Y) < ExitRadius)
                IsWon = true;
        }

        private void PickUpCasts()
        {
            var playerField = new Rectangle(Player.Position.X - Player.Radius, Player.Position.Y - Player.Radius,
                Player.Radius * 2 + 1, Player.Radius * 2 + 1);
            var position = default(Point);
            foreach (var castPos in DroppedCasts.Keys)
                if (playerField.Contains(castPos) && Player.Caster.TryPickUp(DroppedCasts[castPos]))
                {
                    position = castPos;
                    break;
                }

            DroppedCasts.Remove(position);
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


        private void ActEnemies()
        {
            Enemies = Enemies.Where(e => e.HealthPoints > 0).ToList();
            foreach (var enemy in Enemies)
                enemy.TryAct(Player, Map);
        }

        public void Tick()
        {
            if (Player.HealthPoints > 0)
            {
                ActPlayer();
                ActEnemies();
                CastGenerator.TryAddCast(Map, DroppedCasts);
            }
            else IsLost = true;
        }

    }

    class CastGenerator
    {
        public static TickTimer SpawnCastTimer = new TickTimer(200);
        private static readonly List<Cast> Casts =
            new List<Cast>() { Cast.AttackNearest, Cast.Heal, Cast.ParalyzeEnemies };
        public static void TryAddCast(Map map, Dictionary<Point, Cast> casts)
        {
            SpawnCastTimer.Tick();
            if (SpawnCastTimer.IsReady)
            {
                var rnd = new Random();
                var cast = Casts[rnd.Next(0, Casts.Count)];
                var position = map.FindFreePlace(map.CellSize / 3);
                if (!casts.ContainsKey(position))
                    casts.Add(position, cast);
                SpawnCastTimer.Interval *= 2;
                SpawnCastTimer.Restart();
            }
        }
    }
}