using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyUp.Models;

namespace StudyUp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier)) {
                return RedirectToAction("Find", "StudyGroup");
            }

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
