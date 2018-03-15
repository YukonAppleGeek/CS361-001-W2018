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
        private readonly IClock clock;

        public StudyGroupController(StudyUpContext dbContext, IClock clock)
        {
            db = dbContext;
            this.clock = clock;
        }


        [Authorize]
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

        [Authorize]
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

        [Authorize]
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
        public IActionResult View(int Id)
        {
            var dbEntry = db.StudyGroups.Find(Id);
            db.Entry(dbEntry).Collection(i => i.Members).Load();
             db.Entry(dbEntry).Reference(i => i.Course).Load();
            if(dbEntry == null) return NotFound();
            var group = new StudyGroupViewModel(){
                GroupTitle = dbEntry.GroupTitle,
                Location = dbEntry.Location,
                DateTime = dbEntry.StartTime,
                Duration = dbEntry.Duration,
                Capacity = dbEntry.Capacity,
                Objectives = dbEntry.Objectives,
                Id = dbEntry.Id,
                IsCanceled = dbEntry.Cancel,
                MemberCount = dbEntry.Members.Count,
                Course = dbEntry.Course.NameWithoutCourseId.ToTitleCase()
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
            var Courses = student.Courses.Select(s=>s.Course).Where(l => l.EndDate > clock.Now).ToList();                        
            foreach (var sc in Courses)
            {
                var ststudygroups = db.StudyGroups.Where(s=> s.Course.Id == sc.Id && s.StartTime > clock.Now).ToList();
                var var1 = new FindViewModel.CourseStudyGroups();
                var1.Course = sc;
                var1.StudyGroups = ststudygroups;
                model.Courses.Add(var1);
            }
            return View(model);
        }

        public IActionResult Edit(int Id) {
            var EditGroup = db.StudyGroups.Find(Id);
            var createModel = new CreateViewModel(){
                DateDay = EditGroup.StartTime.Day,
                DateMonth = EditGroup.StartTime.Month,
                DateYear = EditGroup.StartTime.Year,
                Title = EditGroup.GroupTitle,
                Location = EditGroup.Location,
                Duration = EditGroup.Duration.Hours,
                Capacity = EditGroup.Capacity,
                Objectives = EditGroup.Objectives,
                StartHour = EditGroup.StartTime.Hour > 12 ? EditGroup.StartTime.Hour - 12 : EditGroup.StartTime.Hour,
                StartMin = EditGroup.StartTime.Minute,
                StartTimePm = EditGroup.StartTime.Hour > 12
            };


            ViewData["Title"] = "Edit Study Group";
            ViewData["Action"] = "Edit";
            return View("Create", createModel);
        }

        [HttpPost]     
        public IActionResult Edit(int Id, CreateViewModel model){    //Adding capability to edit form
            var studyGroup = db.StudyGroups.Find(Id);
            studyGroup.GroupTitle = model.Title;
            studyGroup.Location = model.Location;
            studyGroup.StartTime = new DateTime(model.DateYear.Value, model.DateMonth.Value, model.DateDay.Value, model.StartHour.Value + (model.StartTimePm ? 12 : 0), model.StartMin.Value, 0);
            studyGroup.Duration = new TimeSpan(model.Duration.Value, 0, 0);
            studyGroup.Capacity = model.Capacity.Value;
            studyGroup.Objectives = model.Objectives;

            db.StudyGroups.Update(studyGroup); //find data bas e entry, update to all values in model, then save changes (update all vals entity)
            db.SaveChanges();

            return RedirectToAction("View", new {Id = studyGroup.Id});
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
                    Courses = stu_courses.Select(s => s.Course).Where(l => l.EndDate > clock.Now).ToList()
                };
                
                return View("ChooseCourse", model);
            }

            var createModel = new CreateViewModel();
            createModel.DateDay = clock.Now.Day;
            createModel.DateMonth = clock.Now.Month;
            createModel.DateYear = clock.Now.Year;

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
