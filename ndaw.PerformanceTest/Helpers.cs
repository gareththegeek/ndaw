﻿using System;
using System.Diagnostics;

namespace ndaw.PerformanceTest
{
    public static class Helpers
    {
        public static TimeSpan Time(Action toTime)
        {
            var timer = Stopwatch.StartNew();

            toTime();

            timer.Stop();

            return timer.Elapsed;
        }
    }
}
