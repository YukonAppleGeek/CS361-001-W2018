using System;
using Microsoft.AspNetCore.Mvc;
using StudyUp.Models;

namespace StudyUp.Controllers
{
    public class StudyGroupController : Controller
    {
        public IActionResult Index()
        {
            return View();
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
