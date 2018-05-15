using System;
using System.Drawing;
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

        private void Attack(Character character)
        {
        }

        public int Life;
        public Point NextPosition { get; private set; }
        public bool CanMove { get; private set; }
        private Direction wantedDirection;
        private readonly Timer movementTimer;
        private readonly double movementCooldown;
        public readonly int Size;
        public Player(Point position, double movementCooldown, int size)
        {
            Life = 100;
            Size = size;
            this.Position = position;
            this.movementCooldown = movementCooldown;
            movementTimer = new Timer(this.movementCooldown);
            movementTimer.Elapsed += (sender, args) => CanMove = true;
            movementTimer.Elapsed += (sender, args) => Console.WriteLine(wantedDirection);
            movementTimer.AutoReset = false;
            movementTimer.Start();
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