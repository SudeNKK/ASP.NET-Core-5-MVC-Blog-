using Makali.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using X.PagedList;

namespace Makali.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IToastNotification _toast;
        public HomeController(ApplicationDbContext context, IToastNotification toast)
        {
            _context = context;
            _toast = toast;
        }

        [Route("~/")]
        [Route("~/Anasayfa")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("/Hakkimizda")]
        public IActionResult AboutUs()
        {
            return View(_context.AboutUs.ToList());
        }
        [Route("/Sikca-Sorulan-Sorular")]
        public IActionResult Faqs()
        {
            return View(_context.Faqs.ToList());
        }
        [Route("/Iletisim")]
        public IActionResult Contact()
        {
            TempData["GoogleMapSrc"] = _context.Contact.Select(x=>x.GoogleMap).FirstOrDefault();
            return View();
        }
        [HttpPost]
        [Route("/Iletisim")]
        public IActionResult Contact(string NameSurname, string Email, string Subject, string Message)
        {
            var mailSettings = _context.MailSettings.FirstOrDefault();
            if (mailSettings != null)
            {
                if (NameSurname != null && Email != null && Subject != null && Message != null)
                {
                    MailMessage msg = new MailMessage();
                    msg.Subject = "Yeni bir iletişim mesajı";
                    msg.From = new MailAddress(mailSettings.FromEmailAddress, mailSettings.FromEmailAddressDisplayName);
                    msg.To.Add(new MailAddress(mailSettings.SendEmailAddress, mailSettings.SendEmailAddressDisplayName));
                    msg.IsBodyHtml = true;
                    msg.Body = "Ad: " + NameSurname + "<br>" +  "Email: " + Email + "<br><br>" + "Konu: " + Subject + "<br>" + "Mesaj: " + Message;
                    msg.Priority = MailPriority.High;
                    SmtpClient smtp = new SmtpClient(mailSettings.SmtpHost, Int32.Parse(mailSettings.SmtpPort));
                    NetworkCredential AccountInfo = new NetworkCredential(mailSettings.EmailAddress, mailSettings.EmailAddressPassword);
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = AccountInfo;
                    smtp.EnableSsl = false;
                    try
                    {
                        smtp.Send(msg);
                        _toast.AddSuccessToastMessage("Mesaj Gönderildi");
                        return RedirectToAction("Contact");
                    }
                    catch (Exception)
                    {
                        _toast.AddErrorToastMessage("Mesaj Gönderilemedi");
                    }
                }
                else
                {
                    _toast.AddErrorToastMessage("Geçersiz İçerik");
                }
            }
            _toast.AddErrorToastMessage("İletişim Formu Ayarlanmadı");
            return RedirectToAction("Contact");
        }
        public IActionResult Blogs(int page=1)
        {
            return View(_context.Blog.OrderByDescending(x=>x.CreateDate).ToPagedList(page,9));
        }
        public IActionResult BlogDetails(int? id)
        {
            return View(_context.Blog.FirstOrDefault(x=>x.Id==id));
        }
        public IActionResult BlogListForCategory(int id, int page = 1)
        {
            TempData["BlogTitle"] = _context.BlogCategory.FirstOrDefault(x => x.Id == id).Title;
            TempData["BlogId"] = _context.BlogCategory.FirstOrDefault(x => x.Id == id).Id;
            return View(_context.Blog.Where(x=>x.BlogCategory.Id==id).OrderByDescending(x => x.CreateDate).ToPagedList(page, 9));
        }
        public IActionResult BlogsSearch(string q)
        {
            if (!String.IsNullOrEmpty(q))
            {
                ViewBag.SearchText = q;
                return View(_context.Blog.Where(x => x.Title.Contains(q) || x.Text.Contains(q)).OrderByDescending(x => x.CreateDate).ToList());
            }
            return View(_context.Blog.OrderByDescending(x => x.CreateDate).ToList());
        }
        [Route("/Sözlesmeler/{id}")]
        public IActionResult ContractDetails(int id)
        {
            return View(_context.Contracts.FirstOrDefault(x => x.Id == id));
        }
        public IActionResult Error1(int code)
        {
            return View();
        }
    }
}
