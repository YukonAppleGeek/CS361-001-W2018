using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StudyUp.Models;

namespace StudyUp.Controllers
{
    public class StudyGroupController : Controller
    {
        public IActionResult Index()
        {
            var group = new StudyGroup(){
                GroupTitle = "Software Engineering I Study Group",
                Location = "KEC 1007",
                DateTime = new DateTime(2018, 3, 4, 12, 30, 0, 1),
                Duration = new TimeSpan(20, 0, 0),
                Capacity = 5,
                Objectives = "To create the StudyUp app and review for Quiz 3!"
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
