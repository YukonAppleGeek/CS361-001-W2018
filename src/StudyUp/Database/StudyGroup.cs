using System;
using System.Collections.Generic;

namespace StudyUp.Database
{
    public class StudyGroup
    {
        public StudyGroup() {
            this.Members = new List<StudentStudyGroup>();
        }

        public int Id { get; set; }
        public int CourseId { get; set; }
        public int OwnerId { get; set; }
        public string GroupTitle { get; set; }
        public string Location { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public int Capacity { get; set; }
        public string Objectives { get; set; }
        public bool Cancel { get; set; }

        public virtual Course Course { get; set; }
        public virtual Student Owner { get; set; }
        public virtual ICollection<StudentStudyGroup> Members { get; set; }
    }
}