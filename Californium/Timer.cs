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

        private static List<TimerInfo> timers = new List<TimerInfo>();

        public static void NextFrame(TimerEvent callback)
        {
            timers.Add(new TimerInfo { Event = () => { callback(); return true; } });
        }

        public static void EveryFrame(RepeatingTimerEvent callback)
        {
            timers.Add(new TimerInfo { Event = callback });
        }

        public static void After(float time, TimerEvent callback)
        {
            timers.Add(new TimerInfo { Event = () => { callback(); return true; }, Time = time });
        }

        public static void Every(float time, RepeatingTimerEvent callback)
        {
            timers.Add(new TimerInfo { Event = callback, StartTime = time, Time = time });
        }

        public static void Update()
        {
            var readonlyTimers = new List<TimerInfo>(timers);
            var toRemove = new List<TimerInfo>();

            foreach (var timer in readonlyTimers)
            {
                timer.Time -= GameOptions.Timestep;

                if (!(timer.Time <= 0))
                    continue;

                if (timer.Event())
                    toRemove.Add(timer);
                else
                    timer.Time = timer.StartTime;
            }

            timers.RemoveAll(toRemove.Contains);
        }
    }
}
