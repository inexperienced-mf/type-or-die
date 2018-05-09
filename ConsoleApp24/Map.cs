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
        
        public Map(bool[,] cells, int cellSize)
        {
            CellIsWall = cells;
            WidthInCells = CellIsWall.GetLength(0);
            HeightInCells = CellIsWall.GetLength(1);
            CellSize = cellSize;
            Width = WidthInCells * CellSize;
            Height = HeightInCells * CellSize;
        }

        public Map(string map, int cellSize) : this(ParseMap(map), cellSize) {}

        private static bool[,] ParseMap(string map)
        {
            var rows = map.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var height = rows.Length;
            var width = rows[0].Length;
            var result = new bool[width, height];
            for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                    result[x, y] = rows[x][y] == '@';
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

    }

//    class MapCreature
//    {
//        public static Map CreateMap(string map)
//        {
//            var rows = map.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
//            var result = new GameItem[rows[0].Length, rows.Length];
//            for (var x = 0; x < result.GetLength(0); x++)
//                for (var y = 0; y < result.GetLength(1); y++)
//                    result[x, y] = CreateCell(rows[y][x]);
//            return new Map(result);
//        }
//
//        private static GameItem CreateCell(char c)
//        {
//            switch (c)
//            {
//                case 'P':
//                    return new Player();
//                case 'E':
//                    return new Enemy();
//                case 'H':
//                    return new Hole();
//                case ' ':
//                    return new Path();
//                case 'W':
//                    return new Wall();
//                default:
//                    throw new Exception($"wrong cell {c}");
//            }
//        }
//    }
}
