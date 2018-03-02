using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StudyUp.Models;
using StudyUp.Database;
using System.Linq;

namespace StudyUp.Controllers
{
    public class StudyGroupController : Controller
    {
         private readonly StudyUpContext db;

        public StudyGroupController(StudyUpContext dbContext) {
            db = dbContext;
        }

        // This is to test adding and pulling stuff from the database to text the View UI 
        public IActionResult testData(){
            var StudyGroup = new StudyGroup(){GroupTitle = "Web Dev Study Group"};
            db.StudyGroups.Add(StudyGroup);
            db.SaveChanges(); 
            return NotFound();
        }

        public IActionResult View(int? Id = null)
        {
            var dbEntry = db.StudyGroups.Find(Id);
            if(dbEntry == null) return NotFound();
            var group = new StudyGroupViewModel(){
                GroupTitle = dbEntry.GroupTitle,
                Location = dbEntry.Location,
                DateTime = dbEntry.StartTime,
                Duration = dbEntry.Duration,
                Capacity = dbEntry.Capacity,
                Objectives = dbEntry.Objectives
            };
            return View(group);
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


// var group = new StudyGroupViewModel(){
//                 GroupTitle = "Software Engineering I Study",
//                 Location = "KEC 1007",
//                 DateTime = new DateTime(2018, 3, 4, 12, 30, 0, 1),
//                 Duration = new TimeSpan(20, 0, 0),
//                 Capacity = 5,
//                 Objectives = "To create the StudyUp app and review for Quiz 3!"
//             };