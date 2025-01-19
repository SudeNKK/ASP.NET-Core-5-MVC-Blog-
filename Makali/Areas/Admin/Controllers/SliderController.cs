using Makali.Data;
using Makali.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Makali.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Other.Role_Admin)]
    public class SliderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _he;
        public SliderController(ApplicationDbContext context, IWebHostEnvironment he)
        {
            _context = context;
            _he = he;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.Slider.ToListAsync());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Slider p)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count == 1)
                {
                    if (files[0].Length <= 3148576)
                    {
                        var ext = Path.GetExtension(files[0].FileName.ToLower());
                        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
                        {
                            using var image = Image.FromStream(files[0].OpenReadStream());
                            using var resized = new Bitmap(image, new Size(2048, 2048));
                            using var imageStream = new MemoryStream();
                            resized.Save(imageStream, ImageFormat.Jpeg);
                            string fileName = Guid.NewGuid().ToString();
                            var upload = Path.Combine(_he.WebRootPath, @"images");
                            using (var filesStreams = new FileStream(Path.Combine(upload, fileName + ext), FileMode.Create))
                            {
                                imageStream.Seek(0L, SeekOrigin.Begin);
                                imageStream.CopyTo(filesStreams);
                            }
                            p.Image = @"/images/" + fileName + ext;
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
            return View(p);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            var p = await _context.Slider.FindAsync(id);
            return View(p);
        }
        [HttpPost]
        public IActionResult Edit(Slider p)
        {
            var data = _context.Slider.FirstOrDefault(x => x.Id == p.Id);
            var files = HttpContext.Request.Form.Files;
            data.Text = p.Text;
            data.Title = p.Title;
            data.ButtonText = p.ButtonText;
            data.ButtonUrl = p.ButtonUrl;
            if (files.Count == 1)
            {
                if (files[0].Length <= 3148576)
                {
                    var ext = Path.GetExtension(files[0].FileName.ToLower());
                    if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
                    {
                        if (data.Image != null)
                        {
                            data.Image = data.Image.Replace("/", @"\");
                            var imagePath = Path.Combine(_he.WebRootPath, data.Image.TrimStart('\\'));
                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }
                        }
                        using var image = Image.FromStream(files[0].OpenReadStream());
                        using var resized = new Bitmap(image, new Size(2048, 2048));
                        using var imageStream = new MemoryStream();
                        resized.Save(imageStream, ImageFormat.Jpeg);
                        string fileName = Guid.NewGuid().ToString();
                        var upload = Path.Combine(_he.WebRootPath, @"images");
                        using (var filesStreams = new FileStream(Path.Combine(upload, fileName + ext), FileMode.Create))
                        {
                            imageStream.Seek(0L, SeekOrigin.Begin);
                            imageStream.CopyTo(filesStreams);
                        }
                        data.Image = @"/images/" + fileName + ext;
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
            var p = _context.Slider.Find(id);
            if (p.Image != null)
            {
                p.Image = p.Image.Replace("/", @"\");
                var imagePath = Path.Combine(_he.WebRootPath, p.Image.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            _context.Slider.Remove(p);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
