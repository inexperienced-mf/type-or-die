using System;
using System.Collections.Generic;
using System.Drawing;
using System.Timers;

namespace myGame
{
    public class DirectionFinder
    {
        private static readonly Dictionary<Direction, Point> shift = new Dictionary<Direction, Point>() {
            { Direction.Down, new Point(0, 1) },
            { Direction.Up, new Point(0, -1) },
            { Direction.Right, new Point(1, 0) },
            { Direction.Left, new Point(-1, 0) },
            { Direction.None, new Point(0, 0) } };


        private static void GetFirstSteps(Point position, Map map, Dictionary<Point, Direction> ways, Queue<Point> points)
        {
            for (var i = 0; i < 4; i++)
            {
                var point = new Point
                {
                    X = position.X + shift[(Direction)(i + 1)].X * map.CellSize,
                    Y = position.Y + shift[(Direction)(i + 1)].Y * map.CellSize
                };
                if (!map.IsWallAt(point))
                {
                    ways.Add(point, (Direction)(i + 1));
                    points.Enqueue(point);
                }
            }
        }

        private static Direction FindDirectionToCell(Map map, Point position, Point target, int size)
        {
            var ways = new Dictionary<Point, Direction>();
            var points = new Queue<Point>();
            GetFirstSteps(position, map, ways, points);
            while (points.Count != 0)
            {
                var point = points.Dequeue();
                if (map.InWhichCell(target) == map.InWhichCell(point))
                {
                    return BypassObstacle(ways[point], map, size, position);
                }
                for (var i = 0; i < 4; i++)
                {
                    var lastPoint = new Point
                    {
                        X = point.X + shift[(Direction)(i + 1)].X * map.CellSize,
                        Y = point.Y + shift[(Direction)(1 + i)].Y * map.CellSize
                    };
                    if (map.IsWallAt(lastPoint) || ways.ContainsKey(lastPoint))
                        continue;
                    points.Enqueue(lastPoint);
                    ways.Add(lastPoint, ways[point]);
                }
            }
            return Direction.None;
        }

        public static Direction FindDirectionToTarget(Map map, Point target, Point position, int size)
        {
            if (map.InWhichCell(target) != map.InWhichCell(position))
                return FindDirectionToCell(map, position, target, size);
            else
            {
                var wantedDirection = Direction.None;
                if (target.X - position.X > 0) wantedDirection = Direction.Right;
                else if (target.X - position.X != 0) wantedDirection = Direction.Left;
                else if (target.Y - position.Y > 0) wantedDirection = Direction.Down;
                else if (target.Y - position.Y != 0) wantedDirection = Direction.Up;
                return BypassObstacle(wantedDirection, map, size, position);
            }
        }

        private static Direction BypassObstacle(Direction direction, Map map, int size, Point position)
        {
            var dif = (size % 2 == 1) ? 0 : 1;
            var thickness = (size % 2 == 1) ? (size - 1) / 2 : size / 2;
            var nextPosition = new Point(position.X + shift[direction].X, position.Y + shift[direction].Y);
            var leftUpAngle = new Point(nextPosition.X - thickness + dif, nextPosition.Y - thickness + dif);
            var rightDownAngle = new Point(nextPosition.X + thickness, nextPosition.Y + thickness);
            var leftDownAngle = new Point(nextPosition.X - thickness + dif, nextPosition.Y + thickness);
            var rightUpAngle = new Point(nextPosition.X + thickness, nextPosition.Y - thickness + dif);
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
    }

    public class Enemy : Character
    {
        private Direction wantedDirection;
        public Direction WantedDirection
        {
            get => wantedDirection;
            set
            {
                wantedDirection = value;
                Position = GetNextPosition(wantedDirection, Position);
            }
        }
        private readonly int Damage;
        public int Life { get; private set; }
        private readonly int visibilityRadius;
        public readonly int Size;
        public bool CanMove { get; set; }
        private readonly int attackRadius;
        private readonly int pursuitRadius;
        public Point Position { get; private set; }
        private readonly Queue<Point> Trajectory = new Queue<Point>();
        private readonly Timer movementTimer;

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

        private int GetDistanceToTarget(Point target)
        {
            return Math.Abs(target.X - Position.X) + Math.Abs(target.Y - Position.Y);
        }

        private void MakeDamage(int damage)
        {
            Life -= damage;
        }

        private void Attack(Player player)
        {
            player.MakeDamage(Damage);
        }

        public void Move(Player player, Map map)
        {
            if (CanMove)
            {
                if (GetDistanceToTarget(player.Position) <= visibilityRadius)
                {
                    if (GetDistanceToTarget(player.Position) > attackRadius)
                        WantedDirection = DirectionFinder.FindDirectionToTarget(map, player.Position, Position, Size);
                    else
                        Attack(player);
                }
                else
                {
                    if (Position == Trajectory.Peek())
                        Trajectory.Enqueue(Trajectory.Dequeue());
                    WantedDirection = DirectionFinder.FindDirectionToTarget(map, Trajectory.Peek(), Position, Size);
                }
                CanMove = false;
            }
        }
    }
}
