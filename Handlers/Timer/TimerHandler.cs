using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

/*
 * @author SibauiRP.de
 * Published by 
 * Ich hab dir immer gesagt, reg mich nicht auf.
 */
namespace GangRP_Server.Handlers.Timer
{
    public class TimerHandler : ITimerHandler
    {
        private readonly List<System.Timers.Timer> _globalIntervals = new List<System.Timers.Timer>();

        public void AddTimeout(double timeout, ElapsedEventHandler handler)
        {
            using var timer = new System.Timers.Timer
            {
                Interval = timeout,
                AutoReset = false
            };
            timer.Elapsed += handler;
            timer.Start();
        }

        public void AddInterval(double interval, ElapsedEventHandler handler)
        {
            var timer = new System.Timers.Timer
            {
                Interval = interval
            };
            timer.Elapsed += handler;
            timer.Start();
            _globalIntervals.Add(timer);
        }

        public void StopAllIntervals()
        {
            foreach (var timer in _globalIntervals)
            {
                timer.Stop();
            }
        }
    }
}
