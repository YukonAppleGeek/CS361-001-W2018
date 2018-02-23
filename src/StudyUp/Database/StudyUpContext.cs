using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace StudyUp.Database {
    public class StudyUpContext : DbContext {
        public DbSet<Student> Students { get; set; }
        public DbSet<StudyGroup> StudyGroups { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<StudentStudyGroup> StudentStudyGroups { get; set; }

        public StudyUpContext(DbContextOptions options) : base(options) {}

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentCourse>().HasKey(t => new {t.StudentId, t.CourseId});
            modelBuilder.Entity<StudentStudyGroup>().HasKey(t => new { t.StudentId, t.StudyGroupId });

        }
    }
}