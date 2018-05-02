using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace myGame
{
    class Map
    {
        private readonly int CellSize;
        public readonly GameItem[,] Field;
        public Map(GameItem[,] field)
        {
            Field = field;
        }
        public bool IsInCellBounds(int dx, int dy)
        {
            return Math.Abs(dx) <= CellSize / 2 && Math.Abs(dy) <= CellSize / 2;
        }
        public Map ChangeMap(string map)
        {
            return MapCreature.CreateMap(map);
        }
    }

    class MapCreature
    {
        public static Map CreateMap(string map)
        {
            var rows = map.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var result = new GameItem[rows[0].Length, rows.Length];
            for (var x = 0; x < result.GetLength(0); x++)
                for (var y = 0; y < result.GetLength(1); y++)
                    result[x, y] = CreateCell(rows[y][x]);
            return new Map(result);
        }

        private static GameItem CreateCell(char c)
        {
            switch (c)
            {
                case 'P':
                    return new Player();
                case 'E':
                    return new Enemy();
                case 'H':
                    return new Hole();
                case ' ':
                    return new Path();
                case 'W':
                    return new Wall();
                default:
                    throw new Exception($"wrong cell {c}");
            }
        }
    }
}
