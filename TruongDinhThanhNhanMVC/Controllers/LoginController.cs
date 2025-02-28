using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories;

namespace TruongDinhThanhNhanMVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAccountRepository _accountService; 
        private readonly IConfiguration _configuration;
        public LoginController(IAccountRepository accountService, IConfiguration configuration)
        {
            _accountService = accountService; 
            _configuration = configuration;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }
        [HttpPost]
        public IActionResult Login(SystemAccount model)
        {
            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];

            if (model.AccountEmail == adminEmail && model.AccountPassword == adminPassword)
            {
                HttpContext.Session.SetInt32("UserId", 9999); 
                HttpContext.Session.SetString("Username", "Admin");
                HttpContext.Session.SetString("UserEmail", adminEmail);

                return RedirectToAction("Index", "SystemAccounts");
            }
            var user = _accountService.GetAccountByEmail(model.AccountEmail);

            if (user != null && user.AccountPassword == model.AccountPassword)
            {
                HttpContext.Session.SetInt32("UserId", user.AccountId);
                HttpContext.Session.SetString("Username", user.AccountName);
                HttpContext.Session.SetString("UserEmail", user.AccountEmail);
                HttpContext.Session.SetInt32("Role", user.AccountRole);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid username or password.";
                HttpContext.Session.Clear();
            }

            return View(model);
        }

        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = _accountService.GetAccountById((short)userId.Value);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }
        [HttpPost]
        public IActionResult UpdateProfile(SystemAccount model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Tìm tài khoản cần cập nhật
            var account = _accountService.GetAccountById(model.AccountId);
            if (account == null) return NotFound();

            // Cập nhật thông tin
            account.AccountName = model.AccountName;
            account.AccountPassword = model.AccountPassword;
            account.AccountRole = model.AccountRole;

            _accountService.UpdateAccount(account);
            return RedirectToAction("Index", "NewsArticles");

        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clear session data
            return RedirectToAction("Login");
        }
    }
}
