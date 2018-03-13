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
        public IActionResult testData()
        {
            var MyStudent = db.Students.Find(6089447);
            var Course = db.Courses.Find(1662157);
            var StudyGroup = new StudyGroup(){GroupTitle = "Web Dev Study Group", Owner = MyStudent,Course=Course};
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
                var ststudygroups = db.StudyGroups.Where(s=> s.Course.Id == sc.Id && s.StartTime > DateTime.Now).ToList();
                var var1 = new FindViewModel.CourseStudyGroups();
                var1.Course = sc;
                var1.StudyGroups = ststudygroups;
                model.Courses.Add(var1);
            }
            return View(model);
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

            var createModel = new CreateViewModel();
            createModel.DateDay = DateTime.Now.Day;
            createModel.DateMonth = DateTime.Now.Month;
            createModel.DateYear = DateTime.Now.Year;

            return View(createModel);
        }

        [HttpPost]
        public IActionResult Create(int Id, CreateViewModel model) {
            if (ModelState.ErrorCount > 0) {
                return View(model);
            }

            var userId = int.Parse(User.Claims.Single(s => s.Type == ClaimTypes.NameIdentifier).Value);
            var student = db.Students.Find(userId);
            var studyGroup = new StudyGroup() {
                Owner = student,
                CourseId = Id,
                GroupTitle = model.Title,
                Location = model.Location,
                StartTime = new DateTime(model.DateYear.Value, model.DateMonth.Value, model.DateDay.Value, model.StartHour.Value + (model.StartTimePm ? 12 : 0), model.StartMin.Value, 0),
                Duration = new TimeSpan(model.Duration.Value, 0, 0),
                Capacity = model.Capacity.Value,
                Objectives = model.Objectives
            };

            db.StudyGroups.Add(studyGroup);
            db.SaveChanges();

            var studentStudyGroup = new StudentStudyGroup() {
                Student = student,
                StudyGroup = studyGroup
            };

            db.StudentStudyGroups.Add(studentStudyGroup);
            db.SaveChanges();

            return RedirectToAction("View", new {Id = studyGroup.Id});
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            
        }
    }
}
