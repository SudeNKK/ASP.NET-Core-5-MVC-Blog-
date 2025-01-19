using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Makali.Data;
using Makali.Models;
using Microsoft.AspNetCore.Authorization;

namespace Makali.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Other.Role_Admin)]
    public class SocialMediasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SocialMediasController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.SocialMedia.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(SocialMedia p)
        {
            if (ModelState.IsValid)
            {
                _context.Add(p);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            var p = await _context.SocialMedia.FindAsync(id);
            return View(p);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(SocialMedia p)
        {

            if (ModelState.IsValid)
            {
                _context.Update(p);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _context.SocialMedia.FindAsync(id);
            _context.SocialMedia.Remove(p);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
