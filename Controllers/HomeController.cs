using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;
     
        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
     
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
        // ===========================REGISTRATION=======================
        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            if(dbContext.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email already in use!");
            }
            if(ModelState.IsValid)
            {
                var hasher = new PasswordHasher<User>();
                user.Password = hasher.HashPassword(user, user.Password);
                dbContext.Users.Add(user);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("UserId", user.UserId);
                return Redirect("/dashboard");
            }
            return View("Index");
        }
        // ================================LOGIN======================
        [HttpPost("login")]
        public IActionResult Login(LoginUser userData)
        {
            User userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userData.LoginEmail);
            if(userInDb == null)
            {
                ModelState.AddModelError("LoginEmail", "Email not found!");
            } 
            else
            {
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userData, userInDb.Password, userData.LoginPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("LoginPassword", "Incorrect Password!");
                }
            }
            if(!ModelState.IsValid)
            {
                return View("Index");
            }
            HttpContext.Session.SetInt32("UserId", userInDb.UserId);
            return Redirect("/dashboard");
        }
        // =======================LOGOUT=================================
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserId");
            return Redirect("/");
        }
        [HttpGet("new")]
        public IActionResult PlanWedding()
        {
            int? UserId= HttpContext.Session.GetInt32("UserId");
            if(UserId==null){
                return Redirect("/");
            }
            return View("PlanWedding");
        }
        
        [HttpPost("create")]
        public IActionResult CreateWedding(Wedding w)
        {
            int? UserId= HttpContext.Session.GetInt32("UserId");
            if (ModelState.IsValid)
            {
                w.UserId=(int) UserId;
                dbContext.Weddings.Add(w);
                dbContext.SaveChanges();
                return Redirect ($"ViewWedding/{w.WeddingId}");



            }
            return View ("PlanWedding",w);
            

        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            int? UserId= HttpContext.Session.GetInt32("UserId");
            if(UserId==null){
                return Redirect("/");
            }
            if (ModelState.IsValid)
            {
            List<Wedding> allWeddings= dbContext.Weddings
                .Include(w=>w.Attendees)
                .ThenInclude(going=>going.User)
                .ToList();
                // .FirstOrDefault()
            ViewBag.AllWeddings= allWeddings;
            ViewBag.UserId = (int)UserId;
            return View("Dashboard");
            }
            return Redirect("/");
        }

        [HttpGet("ViewWedding/{wId}")]
        public IActionResult ViewWedding(int wId)
        {
            int? UserId= HttpContext.Session.GetInt32("UserId");
            if(UserId==null){
                return Redirect("/");
            }
            ViewBag.ThisWedding = dbContext.Weddings
                .Include(w=>w.Attendees)
                .ThenInclude(going=>going.User)
                .FirstOrDefault(w => w.WeddingId==wId);    
            return View("ViewWedding");
        }

        [HttpGet("rsvp/{wId}")]

        public IActionResult rsvp(int wId)
        {
            int? UserId=HttpContext.Session.GetInt32("UserId");
            Guest g = new Guest()
            {
                WeddingId= wId,
                UserId=(int) UserId
            };
            dbContext.Guests.Add(g);
            dbContext.SaveChanges();
            return Redirect ("/dashboard");
        }

        [HttpGet("cancel/{wId}")]
        
        public IActionResult cancel(int wId)
        {   
            int? UserId=HttpContext.Session.GetInt32("UserId");
            Wedding w1= dbContext.Weddings.FirstOrDefault(w=>w.WeddingId==wId);
            dbContext.Weddings.Remove(w1);
            dbContext.SaveChanges();
            return Redirect("/dashboard");
        }

        [HttpGet("leave/{wId}")]
        
        public IActionResult leave(int wId)
        {   
            int? UserId=HttpContext.Session.GetInt32("UserId");
            Guest gg = dbContext.Guests.Where(g=>g.WeddingId==wId)
                .FirstOrDefault(g=>g.UserId==(int)UserId);
            dbContext.Guests.Remove(gg);
            dbContext.SaveChanges();
            return Redirect("/dashboard");
        }
    }
}
