using System;
using System.Linq;
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

        private static readonly Dictionary<int, TimerInfo> Timers = new Dictionary<int, TimerInfo>();

        public static int NextFrame(TimerEvent callback)
        {
            return Add(new TimerInfo
            {
                Event = () => { callback(); return true; }
            });
        }

        public static int EveryFrame(RepeatingTimerEvent callback)
        {
            return Add(new TimerInfo{ Event = callback });
        }

        public static int After(float seconds, TimerEvent callback)
        {
            return Add(new TimerInfo
            {
                Event = () => { callback(); return true; },
                Time = seconds
            });
        }

        public static int Every(float seconds, RepeatingTimerEvent callback)
        {
            return Add(new TimerInfo
            {
                Event = callback,
                StartTime = seconds,
                Time = seconds
            });
        }

        public static void Cancel(int id)
        {
            lock (Timers)
                Timers.Remove(id);
        }

        public static bool Exists(int id)
        {
            lock (Timers)
                return Timers.ContainsKey(id);
        }

        internal static void Update()
        {
            lock (Timers)
            {
                var readonlyTimers = Timers.Values.ToList();
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

                Timers.RemoveAll(kv => toRemove.Contains(kv.Value));
            }
        }

        private static int Add(TimerInfo info)
        {
            lock (Timers)
            {
                var id = GenerateId();
                Timers.Add(id, info);
                return id;
            }
        }

        private static readonly Random Random = new Random();
        private static int GenerateId()
        {
            var res = 0;
            do
            {
                res = Random.Next();
            } while (Timers.ContainsKey(res));
            return res;
        }
    }
}
