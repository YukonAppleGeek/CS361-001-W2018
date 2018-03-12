using System;
using System.Collections.Generic;

namespace StudyUp.Database
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string NameWithoutCourseId { 
            get {
                return Name.Substring(0, Name.IndexOf('('));
            }
        }
    }
}