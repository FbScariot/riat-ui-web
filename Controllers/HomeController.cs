using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using RIAT.DAL.Entity.Models;

namespace RIAT.UI.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<HomeController> _localizer;

        private readonly ILogger<HomeController> _logger;

        [AllowAnonymous]
        public IActionResult SetLanguage(string culture)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                                    CookieRequestCultureProvider.MakeCookieValue(requestCulture: new RequestCulture(culture, culture)),
                                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddMonths(6) });

            return Redirect(Request.Headers["Referer"].ToString());
            //return RedirectToActionPreserveMethod("Index");
        }

        public HomeController(ILogger<HomeController> logger, IStringLocalizer<HomeController> localizer)
        {
            _logger = logger;
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            ViewData["Current"] = "Home";
            return View();
        }

        public IActionResult FAQ()
        {
            ViewData["Current"] = "FAQ";
            return View();
        }

        public IActionResult Cursos()
        {
            ViewData["Current"] = "Cursos";
            return View();
        }

        public IActionResult ProfessionalPrivacy()
        {
            ViewData["Current"] = "Professional Privacy";

            return View();
        }

        public IActionResult PatientPrivacy()
        {
            ViewData["Current"] = "Patient Privacy";

            return View();
        }
        public ActionResult About(string returnUrl = null)
        {
            ViewData["Current"] = "About";
            ViewBag.Message = "Your application description page.";

            if (returnUrl != null)
            {
                string action = HttpContext.Request.RouteValues.GetValueOrDefault("action").ToString();
                string controller = HttpContext.Request.RouteValues.GetValueOrDefault("controller").ToString();
                return Redirect(string.Format("{0}", Url.Action("About", "Home", null, null,null, returnUrl)));
            }
            else
            {
                return View();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
