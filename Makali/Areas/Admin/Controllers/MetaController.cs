using Makali.Data;
using Makali.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Makali.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Other.Role_Admin)]
    public class MetaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _he;
        public MetaController(ApplicationDbContext context, IWebHostEnvironment he)
        {
            _context = context;
            _he = he;
        }
        [HttpGet]
        public IActionResult Edit()
        {
            var data = _context.Meta.FirstOrDefault();
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
        public IActionResult Edit(Meta p)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var upload = Path.Combine(_he.WebRootPath, @"images");
                    var ext = Path.GetExtension(files[0].FileName);
                    p.SiteIconImage = _context.Meta.Where(x => x.Id == p.Id).Select(i => i.SiteIconImage).FirstOrDefault();
                    if (p.SiteIconImage != null)
                    {
                        var imagePath = Path.Combine(_he.WebRootPath, p.SiteIconImage.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using (var filesStreams = new FileStream(Path.Combine(upload, fileName + ext), FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    p.SiteIconImage = @"\images\" + fileName + ext;
                    _context.Update(p);
                    _context.SaveChanges();
                    return RedirectToAction("Edit");
                }
                else
                {
                    p.SiteIconImage = _context.Meta.Where(x => x.Id == p.Id).Select(i => i.SiteIconImage).FirstOrDefault();
                    _context.Update(p);
                    _context.SaveChanges();
                    return RedirectToAction("Edit");
                }
            }
            return View(p);
        }
        [HttpGet]
        public IActionResult Create()
        {

            var data = _context.Meta.FirstOrDefault();
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
        public IActionResult Create(Meta p)
        {
            var files = HttpContext.Request.Form.Files;
            if (files.Count > 0)
            {
                string fileName = Guid.NewGuid().ToString();
                var upload = Path.Combine(_he.WebRootPath, @"images");
                var ext = Path.GetExtension(files[0].FileName);
                if (p.SiteIconImage != null)
                {
                    var imagePath = Path.Combine(_he.WebRootPath, p.SiteIconImage.TrimStart('\\'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                using (var filesStreams = new FileStream(Path.Combine(upload, fileName + ext), FileMode.Create))
                {
                    files[0].CopyTo(filesStreams);
                }
                p.SiteIconImage = @"\images\" + fileName + ext;
            }
            _context.Add(p);
            _context.SaveChanges();
            return RedirectToAction("Edit");
        }
    }
}
