using Makali.Data;
using Makali.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Makali.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Other.Role_Admin)]
    public class WhatsappController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WhatsappController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Edit()
        {
            var data = _context.WhatsApp.FirstOrDefault();
            if (data == null)
            {
                return RedirectToAction("Create");
            }
            else
            {
                return View(data);
            }
        }
        [HttpPost]
        public IActionResult Edit(WhatsApp p)
        {
            _context.Update(p);
            _context.SaveChanges();
            return RedirectToAction("Edit");
        }
        [HttpGet]
        public IActionResult Create()
        {

            var data = _context.WhatsApp.FirstOrDefault();
            if (data == null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Edit");
            }
        }
        [HttpPost]
        public IActionResult Create(WhatsApp p)
        {
            _context.Add(p);
            _context.SaveChanges();
            return RedirectToAction("Edit");
        }
    }
}
