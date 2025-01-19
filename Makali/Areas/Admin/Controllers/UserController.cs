using Makali.Data;
using Makali.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace Makali.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Other.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet]
        [Route("/admin")]
        public IActionResult Index(string q, int page = 1)
        {
            var users = _context.ApplicationUsers.ToPagedList(page, 10);
            var role = _context.Roles.ToList();
            var userRole = _context.UserRoles.ToList();
            foreach (var item in users)
            {
                var roleId = userRole.FirstOrDefault(i => i.UserId == item.Id).RoleId;
                item.Role = role.FirstOrDefault(u => u.Id == roleId).Name;
            }
            return View(users);
        }
        [HttpGet]
        public IActionResult Search(string q, int page = 1)
        {
            if (!String.IsNullOrEmpty(q))
            {
                ViewBag.SearchText = q;
                var users = _context.ApplicationUsers
                    .Where(x => x.Name.Contains(q) ||x.Surname.Contains(q)|| x.Email.Contains(q) || x.PhoneNumber.Contains(q))
                    .ToPagedList(page, 25);
                var role = _context.Roles.ToList();
                var userRole = _context.UserRoles.ToList();
                foreach (var item in users)
                {
                    var roleId = userRole.FirstOrDefault(i => i.UserId == item.Id).RoleId;
                    item.Role = role.FirstOrDefault(u => u.Id == roleId).Name;
                }
                return View(users);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> BlockUser(string id)
        {
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == id);
            var lockDateTask = await _userManager.SetLockoutEndDateAsync(user, new DateTime(2222, 06, 06));
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> UnBlockUser(string id)
        {
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == id);
            var lockDateTask = await _userManager.SetLockoutEndDateAsync(user, null);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> AdminRole(string id)
        {
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == id);
            var roleList = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roleList);
            await _userManager.AddToRoleAsync(user, Other.Role_Admin);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> UserRole(string id)
        {
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == id);
            var roleList = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roleList);
            await _userManager.AddToRoleAsync(user, Other.Role_User);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> UserServiceRole(string id)
        {
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == id);
            var roleList = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roleList);
            await _userManager.AddToRoleAsync(user, Other.Role_Service);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
