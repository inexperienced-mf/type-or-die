﻿using System;
using System.Drawing;
using System.Timers;

namespace myGame
{
    public class Character
    {
        public Point GetNextPosition(Direction wantedDirection, Point position)
        {
            int dx = 0, dy = 0;
            switch (wantedDirection)
            {
                case Direction.None:
                    break;
                case Direction.Left:
                    dx = -1;
                    break;
                case Direction.Right:
                    dx = +1;
                    break;
                case Direction.Up:
                    dy = -1;
                    break;
                case Direction.Down:
                    dy = +1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new Point(position.X + dx, position.Y + dy);
        }

        public int FullHealthPoints { get; private set; }
        public int HealthPoints { get; set; }
        public readonly int Radius;
        public TickTimer MovementTimer;
        public bool IsAlive;

        public Character(int movementCooldown, int radius, int healthPoints)
        {
            FullHealthPoints = healthPoints;
            HealthPoints = healthPoints;
            MovementTimer = new TickTimer(movementCooldown);
            Radius = radius;
        }

        public void TakeDamage(int damage)
        {
            HealthPoints -= damage;
            if (HealthPoints > FullHealthPoints)
                FullHealthPoints = HealthPoints;
        }

        public static int GetDistanceToTarget(Point target, Point position) =>
            Math.Abs(target.X - position.X) + Math.Abs(target.Y - position.Y);
    }
}