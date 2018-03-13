using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudyUp.Models
{
    public class CreateViewModel
    {
        [Required]
        public string Title { get;set; }

        [Required]
        public string Location { get;set; }

        [Required]
        public int? DateMonth { get;set; }

        [Required]
        public int? DateDay { get; set; }

        [Required]
        public int? DateYear { get; set; }
        
        [Required]
        [Range(0, 12)]
        public int? StartHour { get; set; }

        [Required]
        [Range(0, 59)]
        public int? StartMin { get; set; }
        public bool StartTimePm { get; set;}

        [Required]
        public int? Duration { get;set; }

        [Required]
        public int? Capacity { get;set; }

        [Required]        
        public string Objectives { get;set; }
    }
}
