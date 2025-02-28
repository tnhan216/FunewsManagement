using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using Repositories;
using AspNetCoreGeneratedDocument;
using System.Globalization;

namespace TruongDinhThanhNhanMVC.Controllers
{
    public class SystemAccountsController : Controller
    {
        private readonly FunewsManagementContext _context;
        private readonly IAccountRepository _account;
        private readonly IConfiguration _configuration;
        public SystemAccountsController(IConfiguration configuration, IAccountRepository account)
        {
            _configuration = configuration;
            _account = account;
        }

        // GET: SystemAccounts
        public async Task<IActionResult> Index(string? search,string? sortBy)
        {
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            var userId = HttpContext.Session.GetString("UserId");
            var UserEmail = HttpContext.Session.GetString("UserEmail"); 
            var adminEmail = _configuration["AdminAccount:Email"];
            if (string.IsNullOrEmpty(userId) ||  !UserEmail.Equals(adminEmail))
            {
                return RedirectToAction("Login", "Login");
            }
            if (_account.SearchAccount(search) != null)
            {
                return View( _account.SearchAccount(search));
            }
            if (!string.IsNullOrEmpty(sortBy))
            {
                return View(_account.SortAccounts(sortBy)); ;
            }
            return View(_account.GetAllAccounts());

        }

        // GET: SystemAccounts/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemAccount = _account.GetAccountById(id);
            if (systemAccount == null)
            {
                return NotFound();
            }

            return View(systemAccount);
        }

        // GET: SystemAccounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SystemAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountName,AccountEmail,AccountRole,AccountPassword")] SystemAccount systemAccount)
        {
            if (ModelState.IsValid)
            {
                _account.AddAccount(systemAccount);               
                return RedirectToAction(nameof(Index));
            }
            return View(systemAccount);
        }

        // GET: SystemAccounts/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemAccount = _account.GetAccountById(id);
            if (systemAccount == null)
            {
                return NotFound();
            }
            return View(systemAccount);
        }

        // POST: SystemAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("AccountId,AccountName,AccountEmail,AccountRole,AccountPassword")] SystemAccount systemAccount)
        {
            if (id != systemAccount.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _account.UpdateAccount(systemAccount);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SystemAccountExists(systemAccount.AccountId))
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
            return View(systemAccount);
        }

        // GET: SystemAccounts/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var systemAccount = _account.GetAccountById(id);
            if (systemAccount == null)
            {
                return NotFound();
            }

            return View(systemAccount);
        }

        // POST: SystemAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var systemAccount = _account.GetAccountById(id);
            if (systemAccount != null)
            {
                _account.DeleteAccount(systemAccount);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SystemAccountExists(short id)
        {
            return _context.SystemAccounts.Any(e => e.AccountId == id);
        }
    }
}
