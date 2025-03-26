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
using Microsoft.Data.SqlClient;
using X.PagedList;
using X.PagedList.Extensions;

namespace TruongDinhThanhNhanMVC.Controllers
{
    public class SystemAccountsController : Controller
    {
        private readonly FunewsManagementContext _context;
        private readonly IAccountRepository _account;
        private readonly IConfiguration _configuration;
        public SystemAccountsController(IConfiguration configuration, IAccountRepository account,FunewsManagementContext context)
        {
            _configuration = configuration;
            _account = account;
            _context = context;
        }

        // GET: SystemAccounts
        public async Task<IActionResult> Index(string search, string sortBy, string sortOrder,int? page)
        {
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            var userId = HttpContext.Session.GetString("UserId");
            var UserEmail = HttpContext.Session.GetString("UserEmail"); 
            var adminEmail = _configuration["AdminAccount:Email"];
            if (string.IsNullOrEmpty(userId) ||  !UserEmail.Equals(adminEmail))
            {
                return RedirectToAction("Login", "Login");
            }
            var accounts = _context.SystemAccounts.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                accounts = accounts.Where(a => a.AccountName.Contains(search) || a.AccountEmail.Contains(search));
            }
            sortOrder = sortOrder == "desc" ? "desc" : "asc"; // Đảm bảo giá trị hợp lệ
            accounts = sortBy switch
            {
                "name" => sortOrder == "asc" ? accounts.OrderBy(a => a.AccountName) : accounts.OrderByDescending(a => a.AccountName),
                "email" => sortOrder == "asc" ? accounts.OrderBy(a => a.AccountEmail) : accounts.OrderByDescending(a => a.AccountEmail),
                "role" => sortOrder == "asc" ? accounts.OrderBy(a => a.AccountRole) : accounts.OrderByDescending(a => a.AccountRole),
                _ => accounts
            };
            int pageSize = 5;
            int pageNumber = page ?? 1;
            var pagedAccounts = accounts.ToPagedList(pageNumber, pageSize);

            return View(pagedAccounts);

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
        public async Task<IActionResult> Statictical(DateTime? startDate, DateTime? endDate)
        {
            using (_context) // Thay YourDbContext bằng DbContext của bạn
            {
                var query = _context.NewsArticles.AsQueryable();

                if (startDate.HasValue)
                {
                    query = query.Where(a => a.CreatedDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(a => a.CreatedDate <= endDate.Value);
                }

                var articles = await query.ToListAsync();

                ViewData["StartDate"] = startDate;
                ViewData["EndDate"] = endDate;

                return View(articles);
            }
        }

    }
}
