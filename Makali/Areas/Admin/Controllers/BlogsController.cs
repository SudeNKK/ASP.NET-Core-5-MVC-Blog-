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
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using X.PagedList;

namespace Makali.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Other.Role_Admin)]
    public class BlogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _he;
        public BlogsController(ApplicationDbContext context, IWebHostEnvironment he)
        {
            _context = context;
            _he = he;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            var applicationDbContext = _context.Blog.Include(b => b.BlogCategory);
            return View(await applicationDbContext.OrderByDescending(x=>x.CreateDate).ToPagedListAsync(page, 10));
        }
        public IActionResult Search(string q, int page = 1)
        {
            if (!String.IsNullOrEmpty(q))
            {
                ViewBag.SearchText = q;
                return View(_context.Blog.Include(x => x.BlogCategory).Where(x => x.Title.Contains(q) || x.Text.Contains(q)).ToPagedList(page, 10));
            }
            return View(_context.Blog.Include(x => x.BlogCategory).ToPagedList(page, 10));
        }
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.BlogCategory, "Id", "Title");
            return View();
        }
        [HttpPost]
        public IActionResult Create(Blog p)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count == 1)
                {
                    if (files[0].Length <= 13148576)
                    {
                        var ext = Path.GetExtension(files[0].FileName.ToLower());
                        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
                        {
                            using var image = Image.FromStream(files[0].OpenReadStream());
                            using var resized = new Bitmap(image, new Size(1024, 1024));
                            using var imageStream = new MemoryStream();
                            resized.Save(imageStream, ImageFormat.Jpeg);
                            string fileName = Guid.NewGuid().ToString();
                            var upload = Path.Combine(_he.WebRootPath, @"images");
                            using (var filesStreams = new FileStream(Path.Combine(upload, fileName + ext), FileMode.Create))
                            {
                                imageStream.Seek(0L, SeekOrigin.Begin);
                                imageStream.CopyTo(filesStreams);
                            }
                            p.Image = @"\images\" + fileName + ext;
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                _context.Add(p);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.BlogCategory, "Id", "Title", p.CategoryId);
            return View(p);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blog.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.BlogCategory, "Id", "Title", blog.CategoryId);
            return View(blog);
        }
        [HttpPost]
        public IActionResult Edit(Blog p)
        {
            var data = _context.Blog.FirstOrDefault(x => x.Id == p.Id);
            var files = HttpContext.Request.Form.Files;
            data.CreateDate = p.CreateDate;
            data.Title = p.Title;
            data.Text = p.Text;
            data.CategoryId = p.CategoryId;
            if (files.Count == 1)
            {
                if (files[0].Length <= 13148576)
                {
                    var ext = Path.GetExtension(files[0].FileName.ToLower());
                    if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
                    {
                        if (data.Image != null)
                        {
                            var imagePath = Path.Combine(_he.WebRootPath, data.Image.TrimStart('\\'));
                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }
                        }
                        using var image = Image.FromStream(files[0].OpenReadStream());
                        using var resized = new Bitmap(image, new Size(1024, 1024));
                        using var imageStream = new MemoryStream();
                        resized.Save(imageStream, ImageFormat.Jpeg);
                        string fileName = Guid.NewGuid().ToString();
                        var upload = Path.Combine(_he.WebRootPath, @"images");
                        using (var filesStreams = new FileStream(Path.Combine(upload, fileName + ext), FileMode.Create))
                        {
                            imageStream.Seek(0L, SeekOrigin.Begin);
                            imageStream.CopyTo(filesStreams);
                        }
                        data.Image = @"\images\" + fileName + ext;
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {

            }
            _context.Update(data);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var p = _context.Blog.Find(id);
            if (p.Image != null)
            {
                p.Image = p.Image.Replace("/", @"\");
                var imagePath = Path.Combine(_he.WebRootPath, p.Image.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            _context.Blog.Remove(p);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
