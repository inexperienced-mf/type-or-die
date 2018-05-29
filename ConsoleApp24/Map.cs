using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace myGame
{
    public class Map
    {
        public readonly int CellSize;
        public readonly int HeightInCells;
        public readonly int WidthInCells;
        public readonly int Width;
        public readonly int Height;
        public readonly bool[,] CellIsWall;
        public readonly Point Exit;
        public Dictionary<Color, HashSet<Point>> CheckedPoints = new Dictionary<Color, HashSet<Point>>();

        public Map(bool[,] cells, int cellSize)
        {
            CellIsWall = cells;
            WidthInCells = CellIsWall.GetLength(0);
            HeightInCells = CellIsWall.GetLength(1);
            CellSize = cellSize;
            Width = WidthInCells * CellSize;
            Height = HeightInCells * CellSize;
        }

        public Map(string map, int cellSize) : this(ParseMap(map), cellSize) { }

        private static bool[,] ParseMap(string map)
        {
            var rows = map.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var height = rows.Length;
            var width = rows[0].Length;
            var result = new bool[width, height];
            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                    result[x, y] = rows[y][x] == '@';
            return result;
        }

        public Point InWhichCell(Point position)
        {
            return new Point(position.X / CellSize, position.Y / CellSize);
        }

        public bool IsWallAt(Point position)
        {
            var cell = InWhichCell(position);
            return CellIsWall[cell.X, cell.Y];
        }

        public Point FindFreePlace(int radius)
        {
            var rnd = new Random();
            var cell = new Point(rnd.Next(0, WidthInCells), rnd.Next(0, HeightInCells));
            while (CellIsWall[cell.X, cell.Y])
                cell = new Point(rnd.Next(0, WidthInCells), rnd.Next(0, HeightInCells));
            var dx = rnd.Next(0, CellSize);
            var dy = rnd.Next(0, CellSize);
            while (true)
            {
                if (IsWallAt(new Point(cell.X * CellSize + dx - radius, cell.Y * CellSize + dy - radius)) ||
                    IsWallAt(new Point(cell.X * CellSize + dx - radius, cell.Y * CellSize + dy + radius)) ||
                    IsWallAt(new Point(cell.X * CellSize + dx + radius, cell.Y * CellSize + dy - radius)) ||
                    IsWallAt(new Point(cell.X * CellSize + dx + radius, cell.Y * CellSize + dy + radius)))
                {
                    dx = rnd.Next(0, CellSize);
                    dy = rnd.Next(0, CellSize);
                }
                else break;
            }

            return new Point(cell.X * CellSize + dx, cell.Y * CellSize + dy);
        }
    }
}
