using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Google.Authenticator;
using i_Turtle.Models;
using i_Turtle.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace i_Turtle.Controllers
{
   
    public class TwoFactorController : Controller
    {
        private readonly TurtleDbContext _userManager;
        private readonly TwoFactorService _twoFactorService;

        public TwoFactorController(TurtleDbContext userManager)
        {
            _userManager = userManager;
            _twoFactorService = new TwoFactorService(userManager);
        }

        [HttpGet]
        public async Task<IActionResult> EnableTwoFactor()
        {  
            var user = await _userManager.Users.FirstOrDefaultAsync(x=> x.Id == Convert.ToInt32((HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)));
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var key = _twoFactorService.GetSecretKey(user);
            if (string.IsNullOrEmpty(key))
            {
                _twoFactorService.GenerateAndStoreSecretKey(user);
                key = _twoFactorService.GetSecretKey(user);
            }

            var tokenUrl = $"otpauth://totp/YourApp:{user.Email}?secret={key}&issuer=iTurtle";
            var qrCodeBitmap = _twoFactorService.GenerateQrCodeImage(key, user.Email);
            var model = new TwoFactorViewModel
            {

                SharedKey = key
            };
            using (var stream = new MemoryStream())
            {
                qrCodeBitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                model.QrCodeUrl = stream.ToArray();
            }

       
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EnableTwoFactor(string token)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == Convert.ToInt32((HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)));
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError("token", "Please enter a token.");
                return View();
            }

            var isValid = _twoFactorService.VerifyToken(user, token);
            if (isValid)
            {
                user.TwoFactorEnabled = true;
                _userManager.Update(user);
                await _userManager.SaveChangesAsync();


                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("token", "Invalid token.");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> DisableTwoFactor()
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == Convert.ToInt32((HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)));
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DisableTwoFactor(string token)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == Convert.ToInt32((HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)));
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrEmpty(token))
            {
                ModelState.AddModelError("token", "Please enter a token.");
                return View();
            }

            var isValid = _twoFactorService.VerifyToken(user, token);
            if (isValid)
            {
                user.TwoFactorEnabled = false;
                _userManager.Update(user);
                await _userManager.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("token", "Invalid token.");
            return View();
        }
    }
}