using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;

namespace myGame
{
    public class Player : Character
    {
        public Point Position { get; private set; }
        public Cast SelectedCast { get; private set; }
        public string SelectedCommand { get; private set; }
        public string input = "";
        public Direction WantedDirection
        {
            get => wantedDirection;
            set
            {
                wantedDirection = value;
                NextPosition = GetNextPosition(wantedDirection, Position);
            }
        }
        public int Life;
        public Point NextPosition { get; private set; }
        public bool CanMove { get; private set; }
        public bool WantAttack { get; set; }
        public bool IsBlocked { get; private set; }
        private Direction wantedDirection;
        private readonly Timer movementTimer;
        private readonly double movementCooldown;
        public readonly int Radius;
        public readonly int attackTime;
        public int AttackRadius { get; private set; }
        private int BonusDamage;
        public Queue<Cast> Backpack = new Queue<Cast>();
        private int Fine { get; set; }
        private readonly Dictionary<string, int> AttackCommands;
        public Player(Point position, double movementCooldown, int radius, int attackTime, int attackRadius, Dictionary<string, int> attackCommand)
        {
            Life = 100;
            Radius = radius;
            this.Position = position;
            this.movementCooldown = movementCooldown;
            this.attackTime = attackTime;
            this.AttackRadius = attackRadius;
            movementTimer = new Timer(this.movementCooldown);
            movementTimer.Elapsed += (sender, args) => CanMove = true;
            movementTimer.AutoReset = false;
            movementTimer.Start();
            SelectedCast = null;
            IsBlocked = true;
            Fine = 0;
            BonusDamage = 0;
            AttackCommands = attackCommand;
        }

        public void TryMove()
        {
            if (!CanMove) return;
            Position = NextPosition;
            NextPosition = GetNextPosition(WantedDirection, Position);
            CanMove = false;
            movementTimer.Start();
        }

        public void InputCommand(char c, List<Enemy> enemies)
        {
            var command = (SelectedCast == null) ? SelectedCommand : SelectedCast.Command;
            if (command.Length >= input.Length && command[input.Length] == c)
                input += c;
            else Fine++;
            if (command.Length == input.Length)
            {
                if (SelectedCast != null)
                {
                    Backpack.Dequeue();
                    Cast();
                }
                else Attack(FindEnemies(enemies));
                IsBlocked = true;
                SelectedCast = null;
                SelectedCommand = "";
                input = "";
            }
        }

        private void Cast()
        {
            BonusDamage += SelectedCast.DamageDif - Fine;
            AttackRadius += SelectedCast.AttackRadiusDif - Fine;
            Life += SelectedCast.LifeDif - Fine;
        }

        public void SelectCast()
        {
            SelectedCast = Backpack.Peek();
            IsBlocked = false;
            input = "";
            Fine = 0;
            Console.WriteLine(1);
        }

        public void SelectCommand(int n)
        {
            SelectedCommand = AttackCommands.Keys.ToList()[n];
            IsBlocked = false;
            input = "";
            Fine = 0;
        }

        public void MakeDamage(int damage)
        {
            Life -= damage;
        }

        public List<Enemy> FindEnemies(List<Enemy> enemies)
        {
            return enemies
                .Where(e => GetDistanceToTarget(e.Position, Position) <= AttackRadius)
                .ToList();
        }

        public void Attack(List<Enemy> enemies)
        {
            foreach (var enemy in enemies)
                if (Fine < BonusDamage + AttackCommands[input])
                    enemy.MakeDamage(BonusDamage + AttackCommands[input] - Fine);
        }
    }

    public enum Direction
    {
        None, Left, Right, Up, Down
    }
}