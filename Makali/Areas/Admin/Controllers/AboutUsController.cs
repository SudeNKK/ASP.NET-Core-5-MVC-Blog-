using Makali.Data;
using Makali.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Makali.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Other.Role_Admin)]
    public class AboutUsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _he;
        public AboutUsController(ApplicationDbContext context, IWebHostEnvironment he)
        {
            _context = context;
            _he = he;
        }
        [HttpGet]
        public IActionResult Edit()
        {
            var data = _context.AboutUs.FirstOrDefault();
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
        public IActionResult Edit(AboutUs p)
        {

            if (ModelState.IsValid)
            {    
                    _context.Update(p);
                    _context.SaveChanges();
                
            }
            return View(p);
        }
        [HttpGet]
        public IActionResult Create()
        {

            var data = _context.AboutUs.FirstOrDefault();
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
        public IActionResult Create(AboutUs p)
        {    
            _context.Add(p);
            _context.SaveChanges();
            return RedirectToAction("Edit");
        }
    }
}
