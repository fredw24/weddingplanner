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
        // here we can "inject" our context service into the constructor
        public HomeController(WeddingContext context)
        {
            dbContext = context;
        }

        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost("register")]
        public IActionResult Register(Users user)
        {

            // Check initial ModelState
            if(ModelState.IsValid)
            {

                // If a User exists with provided email
                if(dbContext.Users.Any(u => u.Email == user.Email))
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("Email", "Email already in use!");
                    
                    return View("Index");
                    // You may consider returning to the View at this point
                }

                // Initializing a PasswordHasher object, providing our User class as its
                PasswordHasher<Users> Hasher = new PasswordHasher<Users>();
                user.Password = Hasher.HashPassword(user, user.Password);
                //Save your user object to the database
                dbContext.Add(user);
                dbContext.SaveChanges();

                HttpContext.Session.SetInt32("id", user.UserId);
                Console.WriteLine(HttpContext.Session.GetInt32("id"));
                return RedirectToAction("Success");

            }
            // other code
            return View("Index");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUsers userSubmission)
        {
            if(ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                // If no user exists with provided email
                if(userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Invalid Email");
                    return View("Index");
                }
                
                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUsers>();
                
                // varify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);
                
                // result can be compared to 0 for failure
                if(result == 0)
                {
                    // handle failure (this should be similar to how "existing email" is handled)
                    ModelState.AddModelError("Password", "Wrong Password");
                    return View("Index");

                }


                HttpContext.Session.SetInt32("id", userInDb.UserId);
                Console.WriteLine(HttpContext.Session.GetInt32("id"));
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
                ViewBag.weddings = allWedding;

                ViewBag.id = id;

                return View("WeddingDisplay");
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

                Console.WriteLine(wed.WeddingId);
                
                return RedirectToAction("DisplayWed", new{wedId = wed.WeddingId});

            }
            
            return View("WeddingNew");
        }

        [HttpPost("rsvp")]
        public IActionResult DisplayWed(Guest g){
            
            dbContext.Add(g);
            dbContext.SaveChanges();
            return RedirectToAction("Success");

        }

        [HttpPost("unrsvp")]
        public IActionResult RemoveWed(Guest g){

            Guest guest = dbContext.Guest.FirstOrDefault(d => d.GuestId == g.GuestId);
            
            dbContext.Remove(guest);
            dbContext.SaveChanges();
            return RedirectToAction("Success");

        }

        [HttpGet]
        [Route("Display/{wedId}")]
        public IActionResult DisplayWed(int wedId){

            Wedding oneWed = dbContext.Wedding
                .Include(p => p.Guest)
                .ThenInclude(s => s.Users)
                .FirstOrDefault(d => d.WeddingId == wedId);

            ViewBag.one = oneWed;

            return View("WeddingOne");
        }

        [HttpPost("delete/{wedId}")]
        public IActionResult RemoveWed(int wedId){
            Wedding oneRemove = dbContext.Wedding.FirstOrDefault(d => d.WeddingId == wedId);

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
