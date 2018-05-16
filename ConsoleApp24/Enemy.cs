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


        private static void AddNeighbourhood(Point position, Map map, Dictionary<Point, Direction> ways, Queue<Point> points)
        {
            foreach (Direction d in Enum.GetValues(typeof(Direction)))
            {
                var point = new Point
                {
                    X = position.X + shift[d].X * map.CellSize,
                    Y = position.Y + shift[d].Y * map.CellSize
                };
                if (!map.IsWallAt(point) && d != Direction.None && !ways.ContainsKey(point))
                {
                    if (!ways.ContainsKey(position))
                        ways.Add(point, d);
                    else
                        ways.Add(point, ways[position]);
                    points.Enqueue(point);
                }
            }
        }

        private static Direction FindDirectionToCell(Map map, Point position, Point target, int radius)
        {
            var ways = new Dictionary<Point, Direction>();
            var points = new Queue<Point>();
            AddNeighbourhood(position, map, ways, points);
            while (points.Count != 0)
            {
                var point = points.Dequeue();
                if (map.InWhichCell(target) == map.InWhichCell(point))
                {
                    return BypassObstacle(ways[point], map, radius, position);
                }
                AddNeighbourhood(point, map, ways, points);
            }
            return Direction.None;
        }

        public static Direction FindDirectionToTarget(Map map, Point target, Point position, int radius)
        {
            if (map.InWhichCell(target) != map.InWhichCell(position))
            {
                return FindDirectionToCell(map, position, target, radius);
            }
            else
            {
                var wantedDirection = Direction.None;
                if (target.X - position.X > 0) wantedDirection = Direction.Right;
                else if (target.X - position.X != 0) wantedDirection = Direction.Left;
                else if (target.Y - position.Y > 0) wantedDirection = Direction.Down;
                else if (target.Y - position.Y != 0) wantedDirection = Direction.Up;
                return BypassObstacle(wantedDirection, map, radius, position);
            }
        }

        private static Direction BypassObstacle(Direction direction, Map map, int radius, Point position)
        {
            var nextPosition = new Point(position.X + shift[direction].X, position.Y + shift[direction].Y);
            var leftUpAngle = new Point(nextPosition.X - radius, nextPosition.Y - radius);
            var rightDownAngle = new Point(nextPosition.X + radius, nextPosition.Y + radius);
            var leftDownAngle = new Point(nextPosition.X - radius, nextPosition.Y + radius);
            var rightUpAngle = new Point(nextPosition.X + radius, nextPosition.Y - radius);
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
        public readonly int Radius;
        public bool CanMove { get; set; }
        private readonly int attackRadius;
        private readonly int pursuitRadius;
        public Point Position { get; private set; }
        private readonly Queue<Point> Trajectory = new Queue<Point>();
        private readonly Timer movementTimer;

        public Enemy(Point position, int movementCooldown, int radius, int visibilityRadius,
            int attackRadius, int pursuitRadius, List<Point> trajectory, int damage)
        {
            Position = position;
            Radius = radius;
            Damage = damage;
            this.visibilityRadius = visibilityRadius;
            this.attackRadius = attackRadius;
            this.pursuitRadius = pursuitRadius;
            Trajectory = new Queue<Point>(trajectory);
            Life = 100;
            movementTimer = new Timer(movementCooldown)
            {
                AutoReset = false
            };
            movementTimer.Elapsed += (sender, args) => CanMove = true;
            movementTimer.Start();
        }

        public void MakeDamage(int damage)
        {
            Life -= damage;
        }

        private void Attack(Player player)
        {
            player.MakeDamage(Damage);
        }

        public void TryMove(Player player, Map map)
        {
            if (CanMove)
            {
                if (GetDistanceToTarget(player.Position, Position) <= visibilityRadius)
                {
                    if (GetDistanceToTarget(player.Position, Position) > attackRadius)
                        WantedDirection = DirectionFinder.FindDirectionToTarget(map, player.Position, Position, Radius);
                    else
                        Attack(player);
                }
                else
                {
                    if (Position == Trajectory.Peek())
                        Trajectory.Enqueue(Trajectory.Dequeue());
                    WantedDirection = DirectionFinder.FindDirectionToTarget(map, Trajectory.Peek(), Position, Radius);
                }
                CanMove = false;
                movementTimer.Start();
            }
        }
    }
}
