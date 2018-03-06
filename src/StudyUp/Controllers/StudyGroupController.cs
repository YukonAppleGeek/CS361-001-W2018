using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StudyUp.Models;
using StudyUp.Database;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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

        // This is to test adding and pulling stuff from the database to text the View UI 
        public IActionResult testData(){
            var MyStudent = db.Students.Find(6089312);
            var StudyGroup = new StudyGroup(){GroupTitle = "Web Dev Study Group", Owner = MyStudent};
            db.StudyGroups.Add(StudyGroup);
            db.SaveChanges(); 
            return NotFound();

        }

        public IActionResult Join(int Id){
            //see if studyGroup id exists
            var dbEntry = db.StudyGroups.Find(Id);
            if(dbEntry == null) return RedirectToAction("Error"); 
            //check to see if student id is not already in that study group
            int UserId = int.Parse(User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value); //get user id 
            if(db.StudentStudyGroups.Find(UserId, Id) != null) return RedirectToAction("View", new{id = Id});
            //if both true, join that student
            //if notfound == true && UserId not found, join(UserId)
            db.StudentStudyGroups.Add(new StudentStudyGroup(){StudyGroupId = Id, StudentId = UserId});
            db.SaveChanges();

            return RedirectToAction("View", new{id = Id}); 
        }

        public IActionResult Leave(int Id){
            //check to see if student id is not already in that study group
            int UserId = int.Parse(User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value); //get user id 
            var StudentMembership = db.StudentStudyGroups.Find(UserId, Id); 
            if(StudentMembership == null) return RedirectToAction("View", new{id = Id});
            //if both true, join that student
            //if notfound == true && UserId not found, join(UserId)
            db.StudentStudyGroups.Remove(StudentMembership);
            db.SaveChanges();

            return RedirectToAction("View", new{id = Id}); 
        }

        public IActionResult Cancel(int Id){
            //set a "cancel" flag
            //find record
            var StudyGroup = db.StudyGroups.Find(Id);
            //modify object
            StudyGroup.Cancel = true; 
            //update or save changes
            db.SaveChanges();
            return RedirectToAction("View", new{id = Id}); 
        }

        [Authorize]
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
                Objectives = dbEntry.Objectives,
                Id = dbEntry.Id,
                IsCanceled = dbEntry.Cancel
            };

            int UserId = int.Parse(User.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var Stu = db.Students.Find(UserId); 
            db.Entry(dbEntry).Reference(sg => sg.Owner);
            group.IsOwner = dbEntry.Owner == Stu; 

            group.HasJoined = db.StudentStudyGroups.Find(UserId, Id) != null;
        
            return View(group);
        }

        [Authorize]
        public IActionResult Create(int? Id = null)
        {
            if (Id == null)
            {
                var id = User.Claims.Single(s => s.Type == ClaimTypes.NameIdentifier);
                var stu = db.Students.Find(int.Parse(id.Value));
                db.Entry(stu).Collection(s => s.Courses);
                var stu_courses = db.StudentCourses.Where(sc => sc.StudentId == stu.Id);
                foreach (var sc in stu_courses)
                {
                    db.Entry(sc).Reference(p => p.Course);
                }

                var model = new ChooseCourseViewModel()
                {
                    Courses = stu_courses.Select(s => s.Course).Where(l => l.EndDate > DateTime.Now).ToList()
                };
                
                return View("ChooseCourse", model);
            }

            var createModel = new CreateCourseViewModel();
            createModel.DateDay = DateTime.Now.Day;
            createModel.DateMonth = DateTime.Now.Month;
            createModel.DateYear = DateTime.Now.Year;

            return View(createModel);
        }

        [HttpPost]
        public IActionResult Create(CreateCourseViewModel model) {
            var id = int.Parse(User.Claims.Single(s => s.Type == ClaimTypes.NameIdentifier).Value);
            var student = db.Students.Find(id);
            var studyGroup = new StudyGroup() {
                Owner = student,
                GroupTitle = model.Title,
                Location = model.Location,
                StartTime = new DateTime(model.DateYear, model.DateMonth, model.DateDay, model.StartHour, model.StartMin, 0),
                Duration = new TimeSpan(model.Duration, 0, 0),
                Capacity = model.Capacity,
                Objectives = model.Objectives
            };

            db.StudyGroups.Add(studyGroup);
            db.SaveChanges();

            return RedirectToAction("View", new {Id = studyGroup.Id});
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            
        }
    }
}
