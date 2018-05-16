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
                new Point { X = 1, Y = 1 }, new Point { X = 1, Y = 3 }, 1);
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
            var player = new Player(new Point { X = 30, Y = 14 }, 200, 10, 200, 10, new Dictionary<string, int>());
            foreach (Direction d in Enum.GetValues(typeof(Direction)))
            {
                player.WantedDirection = d;
                player.TryMove();
                Assert.AreEqual(player.NextPosition, new Point { X = 30 + shift[d].X, Y = 14 + shift[d].Y });
            }
        }
    }
}