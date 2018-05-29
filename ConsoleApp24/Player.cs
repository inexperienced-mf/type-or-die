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
        public readonly Caster Caster;
        private Direction wantedDirection;
        public Direction WantedDirection
        {
            get => wantedDirection;
            set
            {
                wantedDirection = value;
                NextPosition = GetNextPosition(wantedDirection, Position);
            }
        }
        public Point NextPosition { get; private set; }
        public readonly int Radius;
        public Player(Point position, int movementCooldown, int radius, int healthPoints, int bagSize) 
            : base(movementCooldown, radius, healthPoints)
        {
            Radius = radius;
            Position = position;
            Caster = new Caster(bagSize);
        }

        public void TryMove()
        {
            MovementTimer.Tick();
            if (MovementTimer.IsReady && Position != NextPosition) 
            {
                Move();
                MovementTimer.Restart();
            }
        }

        private void Move()
        {
            Position = NextPosition;
            NextPosition = GetNextPosition(WantedDirection, Position);
        }
    }

    public class Caster
    {
        public State State;
        public List<Cast> Bag { get; private set; }
        public int BagSize;
        public Cast SelectedCast;
        public int Fine { get; private set; }
        public int SelectedCastOrd { get; set; }
        public int CurrentChar { get; private set; }

        public Caster(int bagSize)
        {
            Bag = new List<Cast>(bagSize);
            State = State.Choosing;
            Bag.Add(Cast.AttackNearest);
            Bag.Add(Cast.AttackNearest);
            Bag.Add(Cast.Heal);
            Bag.Add(Cast.Heal);
            Bag.Add(Cast.CheckNeighbourhood);
            Bag.Add(Cast.ParalizeEnemies);
            BagSize = bagSize;
        }

        private Dictionary<char, int> translate = new Dictionary<char, int>()
        {
            {'q', 0}, {'w', 1}, {'e', 2}, {'r', 3},{'t', 4}, {'y', 5} 
        };


        public bool TryPickUp(Cast cast)
        {
            if (Bag.Count >= BagSize) return false;
            Bag.Add(cast);
            return true;
        }

        public void Register(char c)
        {
            switch (State)
            {
                case State.Blocked:
                    break;
                case State.Choosing:
                    if (!translate.ContainsKey(c) || translate[c] >= Bag.Count)
                        break;
                    SelectedCastOrd = translate[c];
                    SelectedCast = Bag[SelectedCastOrd];
                    CurrentChar = 0;
                    State = State.Typing;
                    break;
                case State.Typing:
                    if (SelectedCast.Command[CurrentChar] == c)
                        CurrentChar++;
                    else Fine++;
                    if (CurrentChar == SelectedCast.Command.Length)
                        State = State.Ready;
                    break;
                case State.Ready:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void TryInvoke(Game game)
        {
            if (State == State.Ready)
            {
                SelectedCast.Effect(game, Fine);
                Fine = 0;
                State = State.Choosing;
                Bag.RemoveAt(SelectedCastOrd);
            }
        }
    }

    public enum State
    {
        Blocked, Choosing, Typing, Ready
    }

    public enum Direction
    {
        None, Left, Right, Up, Down
    }
}