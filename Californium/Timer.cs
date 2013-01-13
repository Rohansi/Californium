using System.Collections.Generic;

namespace Californium
{
    public static class Timer
    {
        public delegate bool RepeatingTimerEvent();
        public delegate void TimerEvent();

        class TimerInfo
        {
            public RepeatingTimerEvent Event;
            public float StartTime;
            public float Time;
        }

        private static readonly List<TimerInfo> Timers = new List<TimerInfo>();

        public static void NextFrame(TimerEvent callback)
        {
            Timers.Add(new TimerInfo { Event = () => { callback(); return true; } });
        }

        public static void EveryFrame(RepeatingTimerEvent callback)
        {
            Timers.Add(new TimerInfo { Event = callback });
        }

        public static void After(float seconds, TimerEvent callback)
        {
            Timers.Add(new TimerInfo { Event = () => { callback(); return true; }, Time = seconds });
        }

        public static void Every(float seconds, RepeatingTimerEvent callback)
        {
            Timers.Add(new TimerInfo { Event = callback, StartTime = seconds, Time = seconds });
        }

        public static void Update()
        {
            var readonlyTimers = new List<TimerInfo>(Timers);
            var toRemove = new List<TimerInfo>();

            foreach (var timer in readonlyTimers)
            {
                timer.Time -= GameOptions.Timestep;

                if (timer.Time > 0)
                    continue;

                if (timer.Event())
                    toRemove.Add(timer);
                else
                    timer.Time = timer.StartTime;
            }

            Timers.RemoveAll(toRemove.Contains);
        }
    }
}
