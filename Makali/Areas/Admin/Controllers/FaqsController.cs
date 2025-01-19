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
    public class FaqsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FaqsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Faqs.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Faqs faqs)
        {
            if (ModelState.IsValid)
            {
                _context.Add(faqs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(faqs);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            var faqs = await _context.Faqs.FindAsync(id);
            return View(faqs);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Faqs faqs)
        {
            
            if (ModelState.IsValid)
            {
                _context.Update(faqs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(faqs);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var faqs = await _context.Faqs.FindAsync(id);
            _context.Faqs.Remove(faqs);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
