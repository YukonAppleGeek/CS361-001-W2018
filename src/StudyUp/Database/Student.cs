using System.Collections.Generic;

namespace StudyUp.Database
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<StudentCourse> Courses { get; set; }
        public List<StudentStudyGroup> StudyGroups { get; set; }
    }
}