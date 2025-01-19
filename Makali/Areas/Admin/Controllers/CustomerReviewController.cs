using Makali.Data;
using Makali.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Makali.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Other.Role_Admin)]
    public class CustomerReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerReviewController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.CustomerReview.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CustomerReview p)
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
            var p = await _context.CustomerReview.FindAsync(id);
            return View(p);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CustomerReview p)
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
            var p = await _context.CustomerReview.FindAsync(id);
            _context.CustomerReview.Remove(p);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
