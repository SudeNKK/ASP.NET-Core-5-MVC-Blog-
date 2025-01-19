using Makali.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace Makali.Areas.User.Controllers
{
    [Area("User")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toast;
        public ProductController(ApplicationDbContext context, IToastNotification toast)
        {
            _context = context;
            _toast = toast;
        } 
        public IActionResult ProductListForCategory(int? id,int page=1)
        {
            if (id!=null)
            {
                TempData["CategoryTitle"] = _context.ProductCategory.FirstOrDefault(x => x.Id == id).Title;
                TempData["CategoryId"] = _context.ProductCategory.FirstOrDefault(x => x.Id == id).Id;
                return View(_context.Product.Include(x => x.ProductImages).Include(x => x.ProductCategory).Where(x => x.CategoryId == id).ToPagedList(page, 12));
            }
            else
            {
                TempData["CategoryTitle"] = "Tüm Ürünler";
                TempData["CategoryId"] = null;
                return View(_context.Product.Include(x => x.ProductImages).ToPagedList(page, 12));
            }
        }
        public IActionResult PriceAscForProduct(int? id, int page = 1)
        {
            if (id != null)
            {
                TempData["CategoryTitle"] = _context.ProductCategory.FirstOrDefault(x => x.Id == id).Title;
                TempData["CategoryId"] = _context.ProductCategory.FirstOrDefault(x => x.Id == id).Id;
                _toast.AddSuccessToastMessage("Artan Fiyata Göre Sıralandı");
                return View(_context.Product.Include(x => x.ProductImages).Include(x => x.ProductCategory).Where(x => x.CategoryId == id).OrderBy(x=>x.Price).ToPagedList(page, 12));
            }
            else
            {
                TempData["CategoryTitle"] = "Tüm Ürünler";
                TempData["CategoryId"] = null;
                _toast.AddSuccessToastMessage("Artan Fiyata Göre Sıralandı");
                return View(_context.Product.Include(x => x.ProductImages).OrderBy(x => x.Price).ToPagedList(page, 12));
            }
        }  
        public IActionResult PriceDescForProduct(int? id, int page = 1)
        {
            if (id != null)
            {
                TempData["CategoryTitle"] = _context.ProductCategory.FirstOrDefault(x => x.Id == id).Title;
                TempData["CategoryId"] = _context.ProductCategory.FirstOrDefault(x => x.Id == id).Id;
                _toast.AddSuccessToastMessage("Azalan Fiyata Göre Sıralandı");
                return View(_context.Product.Include(x => x.ProductImages).Include(x => x.ProductCategory).Where(x => x.CategoryId == id).OrderByDescending(x=>x.Price).ToPagedList(page, 12));
            }
            else
            {
                TempData["CategoryTitle"] = "Tüm Ürünler";
                TempData["CategoryId"] = null;
                _toast.AddSuccessToastMessage("Azalan Fiyata Göre Sıralandı");
                return View(_context.Product.Include(x => x.ProductImages).OrderByDescending(x => x.Price).ToPagedList(page, 12));
            }
        }
        public IActionResult ProductDetails(int id)
        {
            return View(_context.Product.Include(x => x.ProductImages).Include(x=>x.ProductCategory).FirstOrDefault(x=>x.Id==id));
        }
    }
}
