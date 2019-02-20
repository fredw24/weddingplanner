using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private WeddingContext dbContext;
        public HomeController(WeddingContext context) { dbContext = context; }

        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(Users user)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                dbContext.createUser(HttpContext, user);
                return RedirectToAction("Success", "HomeController");
            }
            return View("Index");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUsers userSubmission)
        {
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users
                    .FirstOrDefault(u => u.Email == userSubmission.Email);
                if(userInDb is null)
                {
                    ModelState.AddModelError("Email", "Invalid Email");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LoginUsers>();
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
                if(result == 0)
                {
                    ModelState.AddModelError("Password", "Wrong Password");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("id", userInDb.UserId);
                return RedirectToAction("Success");
            }
            return View("Index");
        }

        [HttpGet]
        [Route("Dashboard")]
        public IActionResult Success()
        {
            int? id = HttpContext.Session.GetInt32("id");
            if(id != null)
            {
                List<Wedding> allWedding = dbContext.Wedding
                    .Include(d => d.Guest)
                    .ThenInclude(u =>u.Users)
                    .ToList();

                Dashboard dash = new Dashboard();
                dash.Weddings = allWedding;
                dash.User = dbContext.Users.FirstOrDefault(u => u.UserId == id);
                return View("WeddingDisplay", dash);
            }
            return View("Index");
        }

        [HttpGet]
        [Route("new")]
        public IActionResult WeddingNew(){ 
            
            return View();
        }

        [HttpPost ("new")]
        public IActionResult NewProcess(Wedding wed){
            wed.UserId = (int)HttpContext.Session.GetInt32("id");
            if (ModelState.IsValid){
                dbContext.Add(wed);
                dbContext.SaveChanges();
                return RedirectToAction("DisplayWed", new{wedId = wed.WeddingId});

            }
            
            return View("WeddingNew");
        }

        [HttpPost("rsvp")]
        public IActionResult DisplayWed(Guest g)
        {
            dbContext.Add(g);
            dbContext.SaveChanges();
            return RedirectToAction("Success");
        }

        [HttpGet("rsvp/{userId}/{weddingId}")]
        public IActionResult DisplayWed(int userId, int weddingId)
        {
           Guest g = new Guest();
           g.UserId = userId; 
           g.WeddingId = weddingId;
            dbContext.Add(g);
            dbContext.SaveChanges();
            return RedirectToAction("Success");
        }

        [HttpPost("unrsvp")]
        public IActionResult RemoveWed(Guest g)
        {
            Guest guest = dbContext.Guest.FirstOrDefault(d => d.GuestId == g.GuestId);
            dbContext.Remove(guest);
            dbContext.SaveChanges();
            return RedirectToAction("Success");

        }

        [HttpGet]
        [Route("Display/{wedId}")]
        public IActionResult DisplayWed(int wedId)
        {
            Wedding oneWed = dbContext.Wedding
                .Include(p => p.Guest)
                .ThenInclude(s => s.Users)
                .FirstOrDefault(d => d.WeddingId == wedId);

            ViewBag.one = oneWed;
            return View("WeddingOne");
        }

        [HttpPost("delete/{wedId}")]
        public IActionResult RemoveWed(int wedId)
        {
            Wedding oneRemove = dbContext.Wedding
                .FirstOrDefault(d => d.WeddingId == wedId);
            dbContext.Remove(oneRemove);
            dbContext.SaveChanges();
            return RedirectToAction("Success");
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }
    }
}
