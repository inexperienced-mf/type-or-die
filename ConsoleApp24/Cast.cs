using System.Drawing;

namespace myGame
{
    public class Cast
    {
        public readonly string Command;
        public readonly string ImageName;
        public readonly int AttackRadiusDif;
        public readonly int DamageDif;
        public readonly int LifeDif;
        public readonly Point Position;
        public readonly int Size;

        public Cast(string command, string imageName, int attackRadiusDif, int damageDif, int lifeDif, Point position, int size)
        {
            Command = command;
            ImageName = imageName;
            AttackRadiusDif = attackRadiusDif;
            DamageDif = damageDif;
            LifeDif = lifeDif;
            Position = position;
            Size = size;
        }
    }
}