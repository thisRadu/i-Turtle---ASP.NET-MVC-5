using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using i_Turtle.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace i_Turtle.Controllers
{
    public class AccountController : Controller
    {
        private readonly TurtleDbContext _context;

        public AccountController(TurtleDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Logout()
        {
          return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string args)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // GET: Account
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Account/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Account/Create
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;  
            return View();
        }

        // POST: Account/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("UserName,Password")] LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {

                // Authenticate user here
                var user = await _context.Users.FirstOrDefaultAsync(x=> x.Name == model.UserName);

                if (user.TwoFactorEnabled)
                {
                    return RedirectToAction("TwoFactor", new { returnUrl });
                }
                if (user != null && user.Password == model.Password)
                {

                    // Create claims for authenticated user
                    var claims = new List<Claim>
            {
                new Claim("UserName", user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

                    // Create authentication properties
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                    };

                    // Create and sign in the user
                    object value =  HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                        authProperties);
                

                // Redirect to home page
                return Redirect("https://localhost:44395/Patients");// +returnUrl);
                }

                ModelState.AddModelError(string.Empty, "Invalid username or password");
            

        }
            return BadRequest();
        }
     
        public async Task<IActionResult> Register(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Register([Bind("UserName,Password,Email")] RegistreModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "A user with this email already exists.");
                return View(model);
            }

            // Check if user with the same username already exists
            existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Name == model.UserName);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "A user with this username already exists.");
                return View(model);
            }
            var user = new User
            {
                Role = "user",
                Active = true,
                Phone = "Not set",
                Name = model.UserName,
                Password = model.Password,
                Email = model.Email,
                TwoFactorEnabled = false,
                TwoFactorCode = string.Empty

            };
/*
            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, model.Password);*/

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login", "Account");
        }
        // GET: Account/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Account/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Password,Email,Phone,Role,Active,TwoFactorEnabled")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Account/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MyAccount()
        {
            int id = Convert.ToInt32((HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value));
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Account/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyAccount(int id, [Bind("Name,Password,Email,Phone")] User user)
        {
            var updateUser = await _context.Users.FindAsync(id);
            if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value != updateUser.Id.ToString()) return BadRequest();
          

            if (ModelState.IsValid)
            {
                try
                {
                    updateUser.Name = user.Name;
                    updateUser.Password = user.Password;
                    updateUser.Email = user.Email;
                    updateUser.Phone = user.Phone;
                    _context.Update(updateUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return View(user);
            }
            return View(user);
        }


        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }



    }
}
