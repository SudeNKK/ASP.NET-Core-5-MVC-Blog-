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
using System.Drawing.Imaging;
using System.Drawing;
using X.PagedList;

namespace Makali.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Other.Role_Admin)]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _he;
        public ProductsController(ApplicationDbContext context, IWebHostEnvironment he)
        {
            _context = context;
            _he = he;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            var applicationDbContext = _context.Product.Include(p => p.ProductCategory);
            return View(await applicationDbContext.ToPagedListAsync(page, 10));
        }
        public IActionResult Search(string q, int page = 1)
        {
            if (!String.IsNullOrEmpty(q))
            {
                ViewBag.SearchText = q;
                return View(_context.Product.Include(x => x.ProductCategory).Where(x => x.Title.Contains(q) || x.Description.Contains(q)).ToPagedList(page, 10));
            }
            return View(_context.Product.Include(x => x.ProductCategory).ToPagedList(page, 10));
        }
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.ProductCategory, "Id", "Title");
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product p)
        {
            Product product = new Product()
            {
                Title = p.Title,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                ProductImages = new List<ProductImages>()
            };
            var files = HttpContext.Request.Form.Files;
            if (files.Count() > 0)
            {
                for (int i = 0; i < files.Count(); i++)
                {
                    if (files[i].Length <= 13048576)
                    {
                        var ext = Path.GetExtension(files[i].FileName).ToLower();
                        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".webp")
                        {
                            using var image = Image.FromStream(files[i].OpenReadStream());
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
                            ProductImages ımagesForProduct = new ProductImages()
                            {
                                Product = product,
                                Image = @"\images\" + fileName + ext,
                            };
                            product.ProductImages.Add(ımagesForProduct);
                        }
                        else
                        {
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            else
            {
                _context.Add(product);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            _context.Add(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int? id)
        {
            ViewData["CategoryId"] = new SelectList(_context.ProductCategory, "Id", "Title");
            return View(_context.Product.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == id));
        }
        [HttpPost]
        public IActionResult Edit(Product p)
        {
            var data = _context.Product.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == p.Id);
            data.Title = p.Title;
            data.Description = p.Description;
            data.Price = p.Price;
            data.CategoryId = p.CategoryId;
            var files = HttpContext.Request.Form.Files;
            if (files.Count() > 0)
            {
                for (int i = 0; i < files.Count(); i++)
                {
                    if (files[i].Length <= 13048576)
                    {
                        var ext = Path.GetExtension(files[i].FileName.ToLower());
                        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
                        {
                            using var image = Image.FromStream(files[i].OpenReadStream());
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
                            ProductImages ımagesForProduct = new ProductImages()
                            {
                                Product = data,
                                Image = @"\images\" + fileName + ext,
                            };
                            data.ProductImages.Add(ımagesForProduct);
                        }
                        else
                        {
                            return RedirectToAction(nameof(Index));
                        }
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            else
            {
                _context.Update(data);
                _context.SaveChanges();
                return RedirectToAction("Edit", new { id = p.Id });
            }
            _context.Update(data);
            _context.SaveChanges();
            return RedirectToAction("Edit", new { id = p.Id });
        }
        public IActionResult DeleteImages(int id)
        {
            var p = _context.ProductImages.Find(id);
            if (p.Image != null)
            {
                p.Image = p.Image.Replace("/", @"\");
                var imagePath = Path.Combine(_he.WebRootPath, p.Image.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            _context.ProductImages.Remove(p);
            _context.SaveChanges();
            return RedirectToAction("Edit", new { id = p.ProductId });
        }
        public IActionResult Delete(int id)
        {
            var p = _context.Product.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == id);
            for (int i = 0; i < p.ProductImages.Count(); i++)
            {
                if (p.ProductImages[i] != null)
                {
                    p.ProductImages[i].Image = p.ProductImages[i].Image.Replace("/", @"\");
                    var imagePath = Path.Combine(_he.WebRootPath, p.ProductImages[i].Image.TrimStart('\\'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
            }
            _context.Product.Remove(p);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
