using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;

namespace myGame
{
    public class Player : Character
    {
        public Point Position { get; private set; }

        public Direction WantedDirection
        {
            get => wantedDirection;
            set
            {
                wantedDirection = value;
                NextPosition = GetNextPosition(wantedDirection, Position);
            }
        }
        public void MakeDamage(int damage)
        {
            Life -= damage;
        }

        public List<Enemy> FindEnemies(List<Enemy> enemies)
        {
            return enemies
                .Where(e => GetDistanceToTarget(e.Position, Position) <= attackRadius)
                .ToList();
        }

        public void Attack(List<Enemy> enemies, string command)
        {
            foreach (var enemy in enemies)
                enemy.MakeDamage(Commands[command]);
        }
        public int Life;
        public Point NextPosition { get; private set; }
        public bool CanMove { get; private set; }
        public bool WantAttack { get; set; }

        private Direction wantedDirection;
        private readonly Timer movementTimer;
        private readonly double movementCooldown;
        public readonly int Radius;
        public readonly int attackTime;
        private readonly int attackRadius;
        public readonly Dictionary<string, int> Commands;
        public Player(Point position, double movementCooldown, int radius, int attackTime, int attackRadius, Dictionary<string, int> commands)
        {
            Life = 100;
            Radius = radius;
            this.Position = position;
            this.movementCooldown = movementCooldown;
            this.attackTime = attackTime;
            this.attackRadius = attackRadius;
            movementTimer = new Timer(this.movementCooldown);
            movementTimer.Elapsed += (sender, args) => CanMove = true;
            movementTimer.AutoReset = false;
            movementTimer.Start();
            Commands = commands;
        }

        public void TryMove()
        {
            if (!CanMove) return;
            Position = NextPosition;
            NextPosition = GetNextPosition(WantedDirection, Position);
            CanMove = false;
            movementTimer.Start();
        }
    }

    public enum Direction
    {
        None, Left, Right, Up, Down
    }
}