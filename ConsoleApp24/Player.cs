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
        private List<Cast> bag;
        public int BagSize;
        public Cast SelectedCast;
        public int Fine { get; private set; }
        public int SelectedCastOrd { get; set; }
        public int CurrentChar { get; private set; }

        public Caster(int bagSize)
        {
            bag = new List<Cast>(bagSize);
            State = State.Choosing;
            bag.Add(Cast.AttackNearest);
            bag.Add(Cast.AttackNearest);
            bag.Add(Cast.Heal);
            bag.Add(Cast.Heal);
            BagSize = bagSize;
        }

        private Dictionary<char, int> translate = new Dictionary<char, int>()
        {
            {'q', 0}, {'w', 1}, {'e', 2}, {'r', 3},{'t', 4}, {'y', 5} 
        };


        public bool TryPickUp(Cast cast)
        {
            if (bag.Count >= BagSize) return false;
            bag.Add(cast);
            return true;
        }

        public void Register(char c)
        {
            switch (State)
            {
                case State.Blocked:
                    break;
                case State.Choosing:
                    if (!translate.ContainsKey(c) || translate[c] >= bag.Count)
                        break;
                    SelectedCastOrd = translate[c];
                    SelectedCast = bag[SelectedCastOrd];
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
                bag.RemoveAt(SelectedCastOrd);
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