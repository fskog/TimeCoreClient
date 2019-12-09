using System;
using System.Collections.Generic;
using System.Text;

namespace TimeCoreClient
{
    public class TimeEntry
    {
        public Guid SystemId { get; set; }
        public DateTime StartTime { get; set; }
        public string StartTimeString =>
            ((DateTime.Now - StartTime).Days == 0)
            ? "Today " + StartTime.ToString("HH:mm")
            : StartTime.ToString("ddd, dd MMM HH:mm");
        public string EndTimeString =>
            ((DateTime.Now - StartTime).Days == 0)
            ? "Today " + StartTime.ToString("HH:mm")
            : StartTime.ToString("ddd, dd MMM HH:mm");
        public DateTime EndTime { get; set; }
        public string Title { get; set; }
        public TimeSpan Elapsed => IsRunning ? DateTime.Now - StartTime : EndTime - StartTime;
        public string ElapsedString => Elapsed.ToString((Elapsed.Hours > 0 ? "hh' h '" : "") + (Elapsed.Minutes > 0 ? "m' min '" : "") + "s' sec '");
        /*(EndTime >= StartTime) ? EndTime - StartTime : DateTime.Now.TimeOfDay;*/
        public int ElapsedMinutes => (int)Elapsed.TotalMinutes;
        public bool IsRunning => isSet(StartTime) && !isSet(EndTime);

        public TimeEntry()
        {
            SystemId = Guid.NewGuid();
            StartTime = DateTime.MinValue;
            Title = "";
        }

        public void StartOrResume()
        {

            // Start new time entry
            //StartTime = StartTime > DateTime.MinValue ? StartTime : DateTime.Now;

            Console.WriteLine($"StartOrResume: old Systemid {SystemId}");
            SystemId = Guid.NewGuid();
            Console.WriteLine($"StartOrResume: new Systemid {SystemId}");

            StartTime = DateTime.Now;
            EndTime = DateTime.MinValue;
        }

        public void Stop()
        {
            EndTime = DateTime.Now;
        }

        private bool isSet(DateTime dateTime)
        {
            return dateTime > DateTime.MinValue;
        }
    }
}
