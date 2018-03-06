using System;
using System.Linq;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StudyUp.Database;
using StudyUp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

        [Authorize]
        public IActionResult Find()
        {
            var userid = User.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Single().Value;
            var student = db.Students.Where(stud => stud.Id == 1).Single();
            db.Entry(student).Collection(s => s.Courses).Load();
            var model = new FindViewModel();  
            model.Courses = student.Courses.Select(s=>s.Course).Where(l => l.EndDate > DateTime.Now).ToList();                        
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
