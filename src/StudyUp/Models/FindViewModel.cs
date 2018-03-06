using System;
using System.Collections.Generic;

namespace StudyUp.Models
{
    public class FindViewModel
    {
        public List<CourseStudyGroups> Courses { get;set; }

        public class CourseStudyGroups
        {
            public Database.Course Course { get;set; }
            public List<Database.StudyGroup> StudyGroups { get;set; }
        }
    }


}