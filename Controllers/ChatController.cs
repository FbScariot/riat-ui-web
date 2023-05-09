using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RIAT.DAL.Entity.Data;

namespace RIAT.UI.Web.Controllers
{
    public class ChatController : Controller
    {
        private readonly RIATContext _context;

        public ChatController(RIATContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult IndexNovo()
        {
            return View();
        }

        public IActionResult Index2()
        {
            return View();
        }
    }
}
