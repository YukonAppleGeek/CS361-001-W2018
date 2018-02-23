using System;
using System.Collections.Generic;

namespace StudyUp.Database
{
    public class StudyGroup
    {
        public int Id { get; set; }
        public string GroupTitle { get; set; }
        public string Location { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public int Capacity { get; set; }
        public string Objectives { get; set; }

        public Course Course { get; set; }
        public Student Owner { get; set; }
        public List<StudentStudyGroup> Members { get; set; }
    }
}