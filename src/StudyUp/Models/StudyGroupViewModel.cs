using System;

namespace StudyUp.Models
{
    public class StudyGroupViewModel
    {
        public string GroupTitle { get; set; }

        public string Location { get; set; }

        public DateTime DateTime { get; set; }

        public TimeSpan Duration { get; set; }

        public int Capacity { get; set; }

        public string Objectives { get; set; }
    }
}