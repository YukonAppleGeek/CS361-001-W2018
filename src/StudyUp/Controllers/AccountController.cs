using System;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;
using StudyUp.Canvas;
using StudyUp.Controllers;
using StudyUp.Models;

namespace StudyUp.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login(string returnUrl = null)
        {
            TempData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(CanvasUser user, string returnUrl = null)
        {
            if (user?.Token == null) {
                ModelState.AddModelError(string.Empty, "Must provide a token.");
                return View();
            }

            JObject userInfo;
            try {
                userInfo = await CanvasApi.GetUserInfo(user.Token);
            } catch(CanvasApiException e) {
                if (e.Response != null) {
                    foreach (var errors in e.Response["errors"]) {
                        ModelState.AddModelError(string.Empty, (string)errors.SelectToken("message"));
                    }
                } else {
                    ModelState.AddModelError(string.Empty, "Unable to authenticate with Canvas.");
                }
                return View();
            }

            if (ModelState.IsValid) {
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim("Token", user.Token));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, (string)userInfo.SelectToken("id")));
                identity.AddClaim(new Claim(ClaimTypes.Name, (string)userInfo.SelectToken("name")));

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            }

            if (returnUrl == null)
            {
                returnUrl = TempData["returnUrl"]?.ToString();
            }

            if (returnUrl != null)
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
