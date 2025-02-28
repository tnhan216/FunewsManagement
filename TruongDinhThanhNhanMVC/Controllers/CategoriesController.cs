using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using Repositories;

namespace TruongDinhThanhNhanMVC.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryRepository _category;
        private readonly FunewsManagementContext _context;
        public CategoriesController(ICategoryRepository category, FunewsManagementContext context)
        {
            _category = category;
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index(string? search)
        {
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            var Role = HttpContext.Session.GetInt32("Role");
            if (HttpContext.Session.GetInt32("UserId") == null || Role == 1)
            {
                return RedirectToAction("Login", "Login");
            }
            if (_category.SearchCategory(search) != null)
            {
                return View(_category.SearchCategory(search));
            }
            var cate = _category.GetCategories();
            return View(cate);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(short id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = _category.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            ViewData["ParentCategoryId"] = new SelectList(_category.GetCategories(), "CategoryId", "CategoryName");
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName,CategoryDesciption,ParentCategoryId,IsActive")] Category category)
        {
            ViewData["ParentCategoryId"] = new SelectList(_category.GetCategories(), "CategoryId", "CategoryName", category.CategoryName);
            if (ModelState.IsValid)
            {
                _category.AddCategory(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(short id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = _category.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }
            ViewData["ParentCategoryId"] = new SelectList(_category.GetCategories(), "CategoryId", "CategoryName", category.CategoryName);
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("CategoryId,CategoryName,CategoryDesciption,ParentCategoryId,IsActive")] Category category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                   _category.UpdateCategory(category);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CategoryId))
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
            ViewData["ParentCategoryId"] = new SelectList(_category.GetCategories(), "CategoryId", "CategoryDesciption", category.CategoryName);
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(short id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = _category.GetCategoryById(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var category =  _category.GetCategoryById(id);
            if (category != null)
            {
                _category.DeleteCategory(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(short id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}
