using System;
using System.Drawing;
using System.Timers;

namespace myGame
{
    public class Player
    {
        public Point Position { get; private set; }

        public Direction WantedDirection
        {
            get => wantedDirection;
            set
            {
                wantedDirection = value;
                NextPosition = getNextPosition();
            }
        }

        public Point NextPosition { get; private set; }
        public bool CanMove { get; private set; }
        private Direction wantedDirection;
        private readonly Timer movementTimer;
        private readonly double movementCooldown;

        public Player(Point position, double movementCooldown)
        {
            this.Position = position;
            this.movementCooldown = movementCooldown;
            movementTimer = new Timer(this.movementCooldown);
            movementTimer.Elapsed += (sender, args) => CanMove = true;
            movementTimer.Elapsed += (sender, args) => Console.WriteLine(wantedDirection);
            movementTimer.AutoReset = false;
            movementTimer.Start();
        }

        private Point getNextPosition()
        {
            int dx = 0, dy = 0;
            switch (WantedDirection)
            {
                case Direction.None:
                    break;
                case Direction.Left:
                    dx = -1;
                    break;
                case Direction.Right:
                    dx = +1;
                    break;
                case Direction.Up:
                    dy = -1;
                    break;
                case Direction.Down:
                    dy = +1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new Point(Position.X + dx, Position.Y + dy);
        }

        public void TryMove()
        {
            if (!CanMove) return;
            Position = NextPosition;
            NextPosition = getNextPosition();
            CanMove = false;
            movementTimer.Start();
        }
    }

    public enum Direction
    {
        None, Left, Right, Up, Down
    }
}