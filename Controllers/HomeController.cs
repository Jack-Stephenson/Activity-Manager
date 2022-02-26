using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using belt.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace belt.Controllers
{
    public class HomeController : Controller
    {
        private MyContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpGet("")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost("Registering")]
        public IActionResult Registering(User newUser)
        {
            if (ModelState.IsValid)
            {
                if (_context.Users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use");
                    return View("Register");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                _context.Users.Add(newUser);
                _context.SaveChanges();
                HttpContext.Session.SetString("UserEmail", newUser.Email);
                return Redirect($"Dashboard/{newUser.UserId}");
            }
            return View("Register");
        }
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost("Logging")]
        public IActionResult Logging(LoginUser loginUser)
        {
            if (ModelState.IsValid)
            {
                User _User = _context.Users.FirstOrDefault(u => u.Email == loginUser.LoginEmail);
                if (_User == null)
                {
                    ModelState.AddModelError("LoginEmail", "Incorrect Email/Password");
                    return View("Login");
                }
                PasswordHasher<LoginUser> Hasher = new PasswordHasher<LoginUser>();
                PasswordVerificationResult result = Hasher.VerifyHashedPassword(loginUser, _User.Password, loginUser.LoginPassword);
                if (result == 0)
                {
                    ModelState.AddModelError("LoginEmail", "Incorrect Email/Password");
                    return View("Login");
                }
                HttpContext.Session.SetString("UserEmail", _User.Email);
                var uid = _User.UserId;
                return Redirect($"Dashboard/{uid}");
            }
            return View("Login");
        }
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            if(_context.Users.Any(u=>u.Email == HttpContext.Session.GetString("UserEmail")))
            {
                User _User = _context.Users.FirstOrDefault(d=>d.Email == HttpContext.Session.GetString("UserEmail"));
                return Redirect($"Dashboard/{_User.UserId}");
            }
            return RedirectToAction("Login");
        }
        [HttpGet("Dashboard/{uid}")]
        public IActionResult Dashboard(int uid)
        {
            if (_context.Users.Any(u => u.Email == HttpContext.Session.GetString("UserEmail")))
            {
                ViewBag.AllHangouts = _context.Hangouts.OrderBy(hangout=>hangout.DateTime).ToList();
                ViewBag.HangoutCounts = _context.Attendances.ToList();
                ViewBag.Attendees = _context.Users.Include(s => s.Hangouts).ThenInclude(d => d.Hangout).FirstOrDefault(e => e.UserId == uid);
                ViewBag.HasAttendees = _context.Attendances.Any(c => c.UserId == uid);
                ViewBag.CurrentUserId = uid;
                ViewBag.AllUsers = _context.Users.ToList();
                return View();
            }
            return RedirectToAction("Login");
        }
        [HttpGet("Dashboard/newHangout")]
        public IActionResult newHangout()
        {
            if (_context.Users.Any(u => u.Email == HttpContext.Session.GetString("UserEmail")))
            {
                User _User = _context.Users.FirstOrDefault(d => d.Email == HttpContext.Session.GetString("UserEmail"));
                ViewBag.CurrentUser = _User.UserId;
                return View();
            }
            return RedirectToAction("Login");
        }
        [HttpPost("Dashboard/makeHangout")]
        public IActionResult makeHangout(Hangout newHangout)
        {
            User _User = _context.Users.FirstOrDefault(d => d.Email == HttpContext.Session.GetString("UserEmail"));
            ViewBag.CurrentUser = _User.UserId;
            if (ModelState.IsValid)
            {
                if (newHangout.DateTime != null)
                {
                    DateTime current = DateTime.Now;
                    DateTime date = newHangout.DateTime;
                    int result = DateTime.Compare(current, date);
                    if (result>0)
                    {
                        ModelState.AddModelError("DateTime", "Date cannot have already passed");
                        return View("newHangout");
                    }
                }
                _context.Hangouts.Add(newHangout);
                _context.SaveChanges();
                return Redirect($"{newHangout.CreatorId}");
            }
            return View("newHangout");
        }
        [HttpGet("Dashboard/HangoutDetails/{hid}")]
        public IActionResult HangoutDetails(int hid)
        {
            if (_context.Users.Any(u=>u.Email == HttpContext.Session.GetString("UserEmail")))
            {
                User _User = _context.Users.FirstOrDefault(d => d.Email == HttpContext.Session.GetString("UserEmail"));
                ViewBag.CurrentUserId = _User.UserId;
                ViewBag.HasAttendees = _context.Attendances.Any(c => c.UserId == _User.UserId);
                ViewBag.Attendees = _context.Users.Include(s => s.Hangouts).ThenInclude(d => d.Hangout).FirstOrDefault(e => e.UserId == _User.UserId);
                ViewBag.Users = _context.Users.ToList();
                ViewBag.Hangout = _context.Hangouts.Include(s=>s.Participants).ThenInclude(p=>p.User).FirstOrDefault(h=>h.HangoutId == hid);
                return View();
            }
            return RedirectToAction("Login");
        }
        [HttpPost("Dashboard/Join")]
        public IActionResult Join(Attendance newAttendance)
        {
            _context.Attendances.Add(newAttendance);
            _context.SaveChanges();
            return Redirect($"{newAttendance.UserId}");
        }
        [HttpPost("Dashboard/HangoutDetails/Join")]
        public IActionResult DetailsJoin(Attendance newAttendance)
        {
            _context.Attendances.Add(newAttendance);
            _context.SaveChanges();
            return Redirect($"{newAttendance.HangoutId}");
        }
        [HttpPost("Dashboard/HangoutDetails/Leave")]
        public IActionResult HangoutLeave(Attendance deleteAttendance)
        {
            var userAttendance = _context.Attendances.FirstOrDefault(d=>d.UserId == deleteAttendance.UserId && d.HangoutId == deleteAttendance.HangoutId);
            _context.Attendances.Remove(userAttendance);
            _context.SaveChanges();
            return Redirect($"{deleteAttendance.HangoutId}");
        }
        [HttpPost("Dashboard/Leave")]
        public IActionResult Leave(Attendance deleteAttendance)
        {
            var userAttendance = _context.Attendances.FirstOrDefault(d=>d.UserId == deleteAttendance.UserId && d.HangoutId == deleteAttendance.HangoutId);
            _context.Attendances.Remove(userAttendance);
            _context.SaveChanges();
            return Redirect($"{deleteAttendance.UserId}");
        }
        [HttpPost("Dashboard/Delete/{hid}")]
        public IActionResult Delete(int hid)
        {
            var Hangout = _context.Hangouts.FirstOrDefault(t=>t.HangoutId == hid);
            var attendances = _context.Attendances.FirstOrDefault(f=>f.HangoutId == hid);
            var userId = Hangout.CreatorId;
            _context.Hangouts.Remove(Hangout);
            if(attendances != null)
            {
                _context.Attendances.Remove(attendances);
            }
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
