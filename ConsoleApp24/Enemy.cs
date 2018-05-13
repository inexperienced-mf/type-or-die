using System;
using System.Collections.Generic;
using System.Drawing;
using System.Timers;

namespace myGame
{
    public class Enemy
    {
        public readonly int Damage;
        public int Life { get; private set; }
        private readonly int visibilityRadius;
        public readonly int Size;
        public bool CanMove { get; private set; }
        private readonly int attackRadius;
        private readonly int pursuitRadius;
        public Point Position { get; private set; }
        public readonly Queue<Point> Trajectory = new Queue<Point>();
        private readonly Timer movementTimer;
        private readonly Dictionary<Direction, Point> difs = new Dictionary<Direction, Point>() {
            { Direction.Down, new Point(0, 1) },
            { Direction.Up, new Point(0, -1) },
            { Direction.Right, new Point(1, 0) },
            { Direction.Left, new Point(-1, 0) },
            { Direction.None, new Point(0, 0) } };

        public Enemy(Point position, int movementCooldown, int size, int visibilityRadius,
            int attackRadius, int pursuitRadius, List<Point> trajectory, int damage)
        {
            Position = position;
            Size = size;
            Damage = damage;
            movementTimer = new Timer(movementCooldown);
            this.visibilityRadius = visibilityRadius;
            this.attackRadius = attackRadius;
            this.pursuitRadius = pursuitRadius;
            Trajectory = new Queue<Point>(trajectory);
            Life = 100;
            movementTimer.Elapsed += (sender, args) => CanMove = true;
            movementTimer.Start();
        }

        public void MakeDamage(int damage)
        {
            Life -= damage;
        }

        private Direction FindDirectionToTarget(Map map, Point target)
        {
            if (map.InWhichCell(target) != map.InWhichCell(Position))
            {
                var ways = new Dictionary<Point, Direction>();
                var points = new Queue<Point>();
                for (var i = 0; i < 4; i++)
                {
                    var point = new Point { X = Position.X + difs[(Direction)(i + 1)].X * map.CellSize,
                        Y = Position.Y + difs[(Direction)(i + 1)].Y * map.CellSize };
                    if (!map.IsWallAt(point))
                    {
                        ways.Add(point, (Direction)(i + 1));
                        points.Enqueue(point);
                    }
                }
                while (points.Count != 0)
                {
                    var point = points.Dequeue();
                    if (map.InWhichCell(target) == map.InWhichCell(point))
                    {
                        return MoveInsideCell(ways[point], map);
                    }
                    for (var i = 0; i < 4; i++)
                    {
                        var lastPoint = new Point { X = point.X + difs[(Direction)(i + 1)].X * map.CellSize,
                            Y = point.Y + difs[(Direction)(1 + i)].Y * map.CellSize };
                        if (map.IsWallAt(lastPoint) || ways.ContainsKey(lastPoint))
                            continue;
                        points.Enqueue(lastPoint);
                        ways.Add(lastPoint, ways[point]);
                    }
                }
            }
            else
            {
                var wantedDirection = Direction.None;
                if (target.X - Position.X > 0) wantedDirection = Direction.Right;
                else if (target.X - Position.X != 0) wantedDirection = Direction.Left;
                else if (target.Y - Position.Y > 0) wantedDirection = Direction.Down;
                else if (target.Y - Position.Y != 0) wantedDirection = Direction.Up;
                return MoveInsideCell(wantedDirection, map);
            }
            return Direction.None;
        }

        private Direction MoveInsideCell(Direction direction, Map map)
        {
            var thickness = 0;
            if (Size % 2 == 1)
                thickness = (Size - 1) / 2;
            else
                thickness = Size / 2;
            var nextPosition = new Point(Position.X + difs[direction].X, Position.Y + difs[direction].Y);
            var leftUpAngle = new Point(nextPosition.X - thickness, nextPosition.Y - thickness);
            var rightDownAngle = new Point(nextPosition.X + thickness, nextPosition.Y + thickness);
            var leftDownAngle = new Point(nextPosition.X - thickness, nextPosition.Y + thickness);
            var rightUpAngle = new Point(nextPosition.X + thickness, nextPosition.Y - thickness);
            if (Size % 2 == 0)
            {
                leftUpAngle = new Point(leftUpAngle.X + 1, leftUpAngle.Y + 1);
                leftDownAngle = new Point(leftDownAngle.X + 1, leftDownAngle.Y);
                rightUpAngle = new Point(rightUpAngle.X, rightUpAngle.Y + 1);
            }
            if (direction == Direction.Right && (map.IsWallAt(rightDownAngle) || map.IsWallAt(rightUpAngle)))
                return (map.IsWallAt(rightDownAngle)) ? Direction.Up : Direction.Down;
            else if (direction == Direction.Left && (map.IsWallAt(leftDownAngle) || map.IsWallAt(leftUpAngle)))
                return (map.IsWallAt(leftDownAngle)) ? Direction.Up : Direction.Down;
            else if (direction == Direction.Down && (map.IsWallAt(leftDownAngle) || map.IsWallAt(rightDownAngle)))
                return (map.IsWallAt(rightDownAngle)) ? Direction.Left : Direction.Right;
            else if (direction == Direction.Up && (map.IsWallAt(leftUpAngle) || map.IsWallAt(rightUpAngle)))
                return (map.IsWallAt(rightUpAngle)) ? Direction.Left : Direction.Right;
            return direction;
        }

        private int GetDistanceToTarget(Point target)
        {
            return Math.Abs(target.X - Position.X) + Math.Abs(target.Y - Position.Y);
        }

        public void Move(Player player, Map map)
        {
            if (CanMove)
            {
                if (GetDistanceToTarget(player.Position) <= visibilityRadius)
                {
                    if (GetDistanceToTarget(player.Position) > attackRadius)
                        Position = GetNextPosition(FindDirectionToTarget(map, player.Position));
                    else
                        Attack(player);
                }
                else
                {
                    if (Position == Trajectory.Peek())
                        Trajectory.Enqueue(Trajectory.Dequeue());
                    Position = GetNextPosition(FindDirectionToTarget(map, Trajectory.Peek()));
                }
                CanMove = false;
            }
        }

        private void Attack(Player player)
        {
        }

        private Point GetNextPosition(Direction direction)
        {
            int dx = 0, dy = 0;
            switch (direction)
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
    }
}