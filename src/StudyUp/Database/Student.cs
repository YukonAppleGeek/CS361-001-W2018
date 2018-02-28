using System.Collections.Generic;

namespace StudyUp.Database
{
    public class Student
    {
        public Student() {
            this.Courses = new List<StudentCourse>();
            this.StudyGroups = new List<StudentStudyGroup>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<StudentCourse> Courses { get; private set; }
        public virtual ICollection<StudentStudyGroup> StudyGroups { get; private set; }
    }
}