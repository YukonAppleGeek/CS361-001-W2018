using System;
using System.Linq;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StudyUp.Database;
using StudyUp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Collections.Generic;

namespace StudyUp.Controllers
{
    public class StudyGroupController : Controller
    {
        private readonly StudyUpContext db;

        public StudyGroupController(StudyUpContext dbContext)
        {
            db = dbContext;
        }

        public IActionResult Index()
        {
            var group = new StudyGroupViewModel(){
                GroupTitle = "Software Engineering I Study Group",
                Location = "KEC 1007",
                DateTime = new DateTime(2018, 3, 4, 12, 30, 0, 1),
                Duration = new TimeSpan(20, 0, 0),
                Capacity = 5,
                Objectives = "To create the StudyUp app and review for Quiz 3!"
            };
            return View(group);
        }

// This is to test adding and pulling stuff from the database to text the View UI 
            public IActionResult testData()
            {
                var MyStudent = db.Students.Find(6089447);
                var Course = db.Courses.Find(1662157);
                var StudyGroup = new StudyGroup(){GroupTitle = "Web Dev Study Group", Owner = MyStudent,Course=Course};
                db.StudyGroups.Add(StudyGroup);
                db.SaveChanges(); 
                return NotFound();
            }
            

        [Authorize]
        public IActionResult Find()
        {
            var userid = int.Parse(User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Single().Value);
            var student = db.Students.Find(userid);
            db.Entry(student).Collection(s => s.Courses).Load();
            var model = new FindViewModel();  
            model.Courses = new List<FindViewModel.CourseStudyGroups>();
            foreach (var c in student.Courses)
            {
                db.Entry(c).Reference(p => p.Course).Load();
            }
            var Courses = student.Courses.Select(s=>s.Course).Where(l => l.EndDate > DateTime.Now).ToList();                        
            foreach (var sc in Courses)
            {
                var ststudygroups = db.StudyGroups.Where(s=>s.Course.Id == sc.Id).ToList();
                var var1 = new FindViewModel.CourseStudyGroups();
                var1.Course = sc;
                var1.StudyGroups = ststudygroups;
                model.Courses.Add(var1);
            }
            return View(model);
        }
        public IActionResult Join()
        {
            throw new NotImplementedException();
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            
        }

       
    }
}
