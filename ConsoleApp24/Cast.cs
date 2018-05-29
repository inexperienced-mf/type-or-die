using System;
using System.Collections.Generic;
using System.Drawing;

namespace myGame
{
    public class Cast
    {
        public readonly string Command;
        public Action<Game, int> Effect;

        public Cast(string command, Action<Game, int> effect)
        {
            Command = command;
            Effect = effect;
        }

//        public static Cast CheckNeighbourhood = new Cast("neighbourhood", (game, mistakesCount) =>
//        {
//            var color = default(Color);
//            var distance = Math.Max(Math.Abs(game.Map.InWhichCell(game.Player.Position).X - game.Exit.X), 
//                Math.Abs(game.Map.InWhichCell(game.Player.Position).Y - game.Exit.Y));
//            if (distance >= Math.Min(game.Map.HeightInCells - 1, game.Map.WidthInCells - 1) * 2 / 3)
//                color = Color.Red;
//            else color = (distance < Math.Min(game.Map.HeightInCells, game.Map.WidthInCells) / 3) ? Color.ForestGreen : Color.Orange;
//            if (!game.Map.CheckedPoints.ContainsKey(color))
//                game.Map.CheckedPoints[color] = new HashSet<Point>();
//            game.Map.CheckedPoints[color].Add(game.Map.InWhichCell(game.Player.Position));
//        });

        public static Cast ParalyzeEnemies = new Cast("paralyze", (game, mistakesCount) =>
        {
            foreach(var e in game.Enemies)
            {
                e.Stop(500 / (mistakesCount + 1));
            }
        });

        public static Cast AttackNearest = new Cast("punch", (game, mistakesCount) =>
        {
            Enemy victim = null;
            var minDistance = 0;
            foreach (var enemy in game.Enemies)
            {
                var distance = Character.GetDistanceToTarget(game.Player.Position, enemy.Position);
                if (victim is null || distance < minDistance)
                {
                    victim = enemy;
                    minDistance = distance;
                }
            }
            //victim?.TakeDamage(50 / (mistakesCount + 1));
            victim?.TakeDamage(50 / ((mistakesCount + 1) * (minDistance / (game.Map.CellSize * 2) + 1)));
        });

        public static Cast Heal = new Cast("heal", (game, mistakesCount) =>
        {
            game.Player.TakeDamage(-30);
        });
    }
}