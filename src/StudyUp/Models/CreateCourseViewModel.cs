using System;
using System.Collections.Generic;

namespace StudyUp.Models
{
    public class CreateCourseViewModel
    {
        public string Title { get;set; }
        public string Location { get;set; }
        public DateTime Date { get;set; }
        public DateTime Start { get;set; }
        public int Duration { get;set; }
        public int Capacity { get;set; }
        public string Objectives { get;set; }
    }
}