using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudyUp.Models
{
    public class CreateCourseViewModel
    {
        public string Title { get;set; }
        public string Location { get;set; }
        public int DateMonth { get;set; }
        public int DateDay { get; set; }
        public int DateYear { get; set; }
        [Range(0, 12)]
        public int StartHour { get; set; }
        [Range(0, 59)]
        public int StartMin { get; set; }
        public bool StartTimePm { get; set;}
        public int Duration { get;set; }
        public int Capacity { get;set; }
        public string Objectives { get;set; }
    }
}