namespace myGame
{
    public class TickTimer
    {
        public int Interval;
        private int remaining;

        public TickTimer(int interval)
        {
            Interval = remaining = interval;
        }

        public bool IsReady => remaining == 0;

        public void Tick()
        {
            if (remaining > 0)
                remaining--;
        }

        public void Restart()
        {
            remaining = Interval;
        }
    }
}