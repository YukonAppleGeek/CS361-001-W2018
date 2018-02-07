using System;

namespace StudyUp.Models
{
    public class StudyGroup
    {
        public string GroupTitle { get; set; }

        public string Location { get; set; }

        public DateTime StartTime { get; set; }

        public TimeSpan Duration { get; set; }

        public int Capacity { get; set; }

        public string Objectives { get; set; }
    }
}