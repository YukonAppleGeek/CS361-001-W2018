using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudyUp.Database
{
    public class StudentStudyGroup
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public int StudyGroupId { get; set; }
        public StudyGroup StudyGroup { get; set; }
    }
}