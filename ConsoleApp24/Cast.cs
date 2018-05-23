using System;
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

            victim?.TakeDamage(50 / (mistakesCount + 1));
        });
        
        public static Cast Heal = new Cast("heal", (game, mistakesCount) =>
        {
            game.Player.TakeDamage(-30);
        });
    }
}