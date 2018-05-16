using System;
using System.Drawing;

namespace myGame
{
    public class Character
    {
        public Point GetNextPosition(Direction wantedDirection, Point position)
        {
            int dx = 0, dy = 0;
            switch (wantedDirection)
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
            return new Point(position.X + dx, position.Y + dy);
        }

        public int GetDistanceToTarget(Point target, Point position)
        {
            return Math.Abs(target.X - position.X) + Math.Abs(target.Y - position.Y);
        }
    }
}