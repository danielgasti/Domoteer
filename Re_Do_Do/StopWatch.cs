using System;
using Microsoft.SPOT;

namespace Re_Do_Do
{
    /// <summary>
    /// Provides a set of methods and properties that you can use to accurately measure elapsed time.
    /// </summary>
    public class Stopwatch
    {
        private TimeSpan lastStartTime; // TS of last Start().
        private TimeSpan elapsed;       // The accumulated elapsed time between Starts/Stops.
        private bool isRunning;

        public Stopwatch()
        {
            Reset();
        }

        /// <summary>
        /// Returns true if running.
        /// </summary>
        public bool IsRunning
        {
            get { return isRunning; }
        }

        /// <summary>
        /// Stops the Stopwatch and resets to zero.
        /// </summary>
        public void Reset()
        {
            this.isRunning = false;
            this.elapsed = TimeSpan.Zero;
            this.lastStartTime = TimeSpan.Zero;
        }

        /// <summary>
        /// Starts, or resumes, measuring elapsed time for an interval.
        /// </summary>
        public void Start()
        {
            if (!this.isRunning)
            {
                lastStartTime = DateTime.UtcNow.TimeOfDay;
                this.isRunning = true;
            }
        }

        /// <summary>
        /// Stops measuring elapsed time for an interval.
        /// </summary>
        public void Stop()
        {
            if (this.isRunning)
            {
                TimeSpan el = DateTime.UtcNow.TimeOfDay - this.lastStartTime;
                this.elapsed += el;
                this.isRunning = false;
                if (this.elapsed < TimeSpan.Zero)
                    this.elapsed = TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Gets the total elapsed time.
        /// </summary>
        public TimeSpan Elapsed
        {
            get { return GetElapsedTimeSpan(); }
        }

        /// <summary>
        /// Gets the total elapsed seconds.
        /// </summary>
        public double ElapsedSeconds
        {
            get { return (double)GetElapsedTimeSpan().Ticks / TimeSpan.TicksPerSecond; }
        }

        private TimeSpan GetElapsedTimeSpan()
        {
            // if we are running, we need to add prior elapsed to current elapsed.
            if (isRunning)
            {
                TimeSpan el = DateTime.UtcNow.TimeOfDay - this.lastStartTime;
                return this.elapsed + el;
            }
            return this.elapsed;
        }
    }
}
