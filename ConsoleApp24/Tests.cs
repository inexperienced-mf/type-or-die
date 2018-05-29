using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Drawing;

namespace myGame
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void ReturnNoneDirection()
        {
            var textMap = "@@@@@\r\n@.@.@\r\n@@@@@";
            var map = new Map(textMap, 1);
            var direction = DirectionFinder.FindDirectionToTarget(map,
                new Point { X = 1, Y = 1 }, new Point { X = 3, Y = 1 }, 1);
            Assert.AreEqual(direction, Direction.None);
        }

        [Test]
        public void BypassObstacleDown()
        {
            var textMap = "@@@@@\r\n@...@\r\n@.@.@\r\n@...@\r\n@@@@@";
            var map = new Map(textMap, 10);
            var direction = DirectionFinder.FindDirectionToTarget(map,
                new Point { X = 25, Y = 33 }, new Point { X = 31, Y = 31 }, 5);
            Assert.AreEqual(direction, Direction.Down);
        }

        [Test]
        public void BypassObstacleUp()
        {
            var textMap = "@@@@@\r\n@...@\r\n@.@.@\r\n@...@\r\n@@@@@";
            var map = new Map(textMap, 10);
            var direction = DirectionFinder.FindDirectionToTarget(map,
                new Point { X = 25, Y = 13 }, new Point { X = 27, Y = 19 }, 5);
            Assert.AreEqual(direction, Direction.Up);
        }

        [Test]
        public void BypassObstacleLeft()
        {
            var textMap = "@@@@@\r\n@...@\r\n@.@.@\r\n@...@\r\n@@@@@";
            var map = new Map(textMap, 10);
            var direction = DirectionFinder.FindDirectionToTarget(map,
                new Point { X = 17, Y = 25 }, new Point { X = 19, Y = 32 }, 5);
            Assert.AreEqual(direction, Direction.Left);
        }

        [Test]
        public void BypassObstacleRight()
        {
            var textMap = "@@@@@\r\n@...@\r\n@.@.@\r\n@...@\r\n@@@@@";
            var map = new Map(textMap, 10);
            var direction = DirectionFinder.FindDirectionToTarget(map,
                new Point { X = 32, Y = 30 }, new Point { X = 30, Y = 32 }, 5);
            Assert.AreEqual(direction, Direction.Right);
        }

        [Test]
        public void FindDirection()
        {
            var textMap = "@@@@@\r\n@...@\r\n@.@.@\r\n@...@\r\n@@@@@";
            var map = new Map(textMap, 10);
            var direction = DirectionFinder.FindDirectionToTarget(map,
                new Point { X = 25, Y = 13 }, new Point { X = 13, Y = 19 }, 5);
            Assert.AreEqual(direction, Direction.Right);
        }

        [Test]
        public void CheckBfs()
        {
            var textMap = "@@@@@\r\n@...@\r\n@.@.@\r\n@...@\r\n@@@@@";
            var map = new Map(textMap, 10);
            var direction = DirectionFinder.FindDirectionToTarget(map,
                new Point { X = 30, Y = 14 }, new Point { X = 30, Y = 34 }, 10);
            Assert.AreEqual(Direction.Left, direction);
        }

        [Test]
        public void CheckPlayerMovement()
        {
            var shift = new Dictionary<Direction, Point>() {
            { Direction.Down, new Point(0, 1) },
            { Direction.Up, new Point(0, -1) },
            { Direction.Right, new Point(1, 0) },
            { Direction.Left, new Point(-1, 0) },
            { Direction.None, new Point(0, 0) } };
            var map = new Map("@@@@@\r\n@...@\r\n@.@.@\r\n@...@\r\n@@@@@", 10);
            var player = new Player(new Point { X = 30, Y = 14 }, 200, 10, 100, 0);
            foreach (Direction d in Enum.GetValues(typeof(Direction)))
            {
                player.WantedDirection = d;
                player.TryMove();
                Assert.AreEqual(player.NextPosition, new Point { X = 30 + shift[d].X, Y = 14 + shift[d].Y });
            }
        }

        [Test]
        public void CheckDistanceToTarget()
        {
            var res = Character.GetDistanceToTarget(new Point(10, 15), new Point(12, 20));
            Assert.AreEqual(7, res);
        }

        [Test]
        public void FindFreePlace()
        {
            var textMap = "@@@@@\r\n@...@\r\n@...@\r\n@...@\r\n@@@@@";
            var map = new Map(textMap, 10);
            var point = map.FindFreePlace(1);
            var isFree = !map.IsWallAt(new Point(point.X - 1, point.Y)) &&
                !map.IsWallAt(new Point(point.X + 1, point.Y)) &&
                !map.IsWallAt(new Point(point.X, point.Y - 1)) &&
                !map.IsWallAt(new Point(point.X, point.Y + 1));
            Assert.AreEqual(true, isFree);
        }

        [Test]
        public void AddCast()
        {
            var textMap = "@@@@@\r\n@...@\r\n@...@\r\n@...@\r\n@@@@@";
            var map = new Map(textMap, 10);
            CastGenerator.AddTimer = new TickTimer(2);
            for (var i = 0; i < 2; i++)
            {
                var dict = new Dictionary<Point, Cast>();
                CastGenerator.TryAddCast(map, dict);
                Assert.AreEqual(i, dict.Count);
            }
        }

        [Test]
        public void TakeDamage()
        {
            var character = new Character(200, 10, 100);
            character.TakeDamage(80);
            Assert.AreEqual(20, character.HealthPoints);
        }

        [Test]
        public void ChangeState()
        {
            var res = new Dictionary<State, Dictionary<char, State>>() {
                { State.Choosing, new Dictionary<char, State>() { { 'b', State.Choosing }, { 'e', State.Typing } } },
                { State.Typing, new Dictionary<char, State>() { { 'x', State.Typing }, { 'h', State.Typing },
                    { 'e', State.Typing }, { 'a', State.Typing }, { 'l', State.Ready } } },
                { State.Blocked, new Dictionary<char, State>() { { 'c', State.Blocked } } },
                { State.Ready, new Dictionary<char, State>() { { 'c', State.Ready } } } };
            var caster = new Caster(6);
            foreach (State state in Enum.GetValues(typeof(State)))
            {
                caster.State = state;
                foreach (var c in res[state].Keys)
                {
                    Console.WriteLine(caster.Fine);
                    caster.Register(c);
                    Assert.AreEqual(res[state][c], caster.State);
                    caster.State = state;
                }
            }
        }
    }
}