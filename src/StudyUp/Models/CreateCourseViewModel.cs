using System;
using System.Collections.Generic;

namespace StudyUp.Models
{
    public class CreateCourseViewModel
    {
        public string Title { get;set; }
        public string Location { get;set; }
        public int DateMonth { get;set; }
        public int DateDay { get; set; }
        public int DateYear { get; set; }
        public int StartHour { get; set; }
        public int StartMin { get; set; }
        public int Duration { get;set; }
        public int Capacity { get;set; }
        public string Objectives { get;set; }
    }
}