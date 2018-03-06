using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StudyUp.Models;
using StudyUp.Database;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization; 

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
