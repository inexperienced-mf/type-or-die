using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime;

namespace myGame
{
    public class DirectionFinder
    {
        private static readonly Dictionary<Direction, Point> shift = new Dictionary<Direction, Point>
        {
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

            var wantedDirection = Direction.None;
            if (target.X - position.X > 0) wantedDirection = Direction.Right;
            else if (target.X - position.X != 0) wantedDirection = Direction.Left;
            else if (target.Y - position.Y > 0) wantedDirection = Direction.Down;
            else if (target.Y - position.Y != 0) wantedDirection = Direction.Up;
            return BypassObstacle(wantedDirection, map, radius, position);
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
            if (direction == Direction.Left && (map.IsWallAt(leftDownAngle) || map.IsWallAt(leftUpAngle)))
                return (map.IsWallAt(leftDownAngle)) ? Direction.Up : Direction.Down;
            if (direction == Direction.Down && (map.IsWallAt(leftDownAngle) || map.IsWallAt(rightDownAngle)))
                return (map.IsWallAt(rightDownAngle)) ? Direction.Left : Direction.Right;
            if (direction == Direction.Up && (map.IsWallAt(leftUpAngle) || map.IsWallAt(rightUpAngle)))
                return (map.IsWallAt(rightUpAngle)) ? Direction.Left : Direction.Right;
            return direction;
        }
    }

    public class Enemy : Character
    {
        private readonly int Damage;
        private readonly int visibilityRadius;
        private readonly int attackRadius;
        private readonly int pursuitRadius;
        public TickTimer AttackTimer;
        public TickTimer StopTimer = new TickTimer(0);

        public Point Position { get; private set; }
        private readonly Queue<Point> Trajectory = new Queue<Point>();

        public Enemy(
            Point position, 
            int movementCooldown, 
            int radius, 
            int visibilityRadius,
            int attackRadius, 
            int pursuitRadius, 
            List<Point> trajectory, 
            int healthPoints, 
            int damage, 
            int attackCooldown) 
            : base(movementCooldown, radius, healthPoints)
        {
            Position = position;
            Damage = damage;
            AttackTimer = new TickTimer(attackCooldown);
            this.visibilityRadius = visibilityRadius;
            this.attackRadius = attackRadius;
            this.pursuitRadius = pursuitRadius;
            Trajectory = new Queue<Point>(trajectory);
        }

        private void TryAttack(Player player)
        {
            AttackTimer.Tick();
            if (AttackTimer.IsReady)
            {
                player.TakeDamage(Damage);
                AttackTimer.Restart();
            }
        }

        public void Stop(int interval)
        {
            StopTimer = new TickTimer(interval);
        }

        public void TryAct(Player player, Map map)
        {
            if (StopTimer.IsReady)
            {
                MovementTimer.Tick();
                if (MovementTimer.IsReady)
                    Act(player, map);
            }
            else
                StopTimer.Tick();
        }

        private void Act(Player player, Map map)
        {
            Point target = Position;
            if (GetDistanceToTarget(player.Position, Position) <= visibilityRadius)
            {
                if (GetDistanceToTarget(player.Position, Position) > attackRadius)
                    target = player.Position;
                else
                    TryAttack(player);
            }
            else
            {
                if (Position == Trajectory.Peek())
                    Trajectory.Enqueue(Trajectory.Dequeue());
                target = Trajectory.Peek();
            }

            var direction = DirectionFinder.FindDirectionToTarget(map, target, Position, Radius);
            if (direction != Direction.None)
            {
                Position = GetNextPosition(direction, Position);
                MovementTimer.Restart();
            }
        }
    }
}
