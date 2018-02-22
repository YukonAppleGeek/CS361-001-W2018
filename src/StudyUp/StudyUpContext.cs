using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StudyUp {
    public class StudyUpContext : DbContext {
        public DbSet<Student> Students { get; set; }
        public DbSet<StudyGroup> StudyGroups { get; set; }
        public DbSet<Course> Courses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=studyup.db");
        }
    }

    public class Student {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<Course> Courses { get; set; }
    }

    public class StudyGroup {
        public int Id { get; set; }
        public string GroupTitle { get; set; }
        public string Location { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public int Capacity { get; set; }
        public string Objectives { get; set; }

        public Student Owner { get; set; }
        public List<Student> Members { get; set; }
    }

    public class Course {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}