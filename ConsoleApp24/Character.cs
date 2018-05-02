using System.Drawing;

namespace myGame
{
    internal class Character : GameItem
    {
        public Point Size { get; }
        public Point LeftLowerCorner { get; set; }
    }
}