using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using Repositories;
using System.Security.Claims;

namespace TruongDinhThanhNhanMVC.Controllers
{
    public class NewsArticlesController : Controller
    {
        private readonly FunewsManagementContext _context;
        private readonly INewsArticleRepository _newArt;

        public NewsArticlesController(FunewsManagementContext context,INewsArticleRepository newArt)
        {
            _context = context;
            _newArt = newArt;
        }

        // GET: NewsArticles
        public async Task<IActionResult> Index(string? search, string? filter,int? userId)
        {
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            ViewData["UserId"] = HttpContext.Session.GetInt32("UserId");
            if (_newArt.SearchNewsArticles(search,filter, userId) != null)
            {
                return View(_newArt.SearchNewsArticles(search, filter, userId));
            }
            var newArt= _newArt.GetAllNewsArticles();
            return View(newArt);
        }

        // GET: NewsArticles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsArticle = _newArt.GetNewsArticleById(id);
            if (newsArticle == null)
            {
                return NotFound();
            }

            return View(newsArticle);
        }

        // GET: NewsArticles/Create
        public IActionResult Create()
        {
            //int? userId = (int)HttpContext.Session.GetInt32("UserId");
            //if (userId == null)
            //{
            //    return RedirectToAction("Login", "Login");
            //}
            ViewData["UserId"] = HttpContext.Session.GetString("UserId");
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: NewsArticles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsArticle newsArticle)
        {
            int createby = (int)HttpContext.Session.GetInt32("UserId");
            

            var lastArticle = await _context.NewsArticles
                                            .OrderByDescending(n => n.NewsArticleId)
                                            .FirstOrDefaultAsync();

            int newId = 1; // Default ID if no articles exist

            if (lastArticle != null)
            {
                if (int.TryParse(lastArticle.NewsArticleId, out int lastId))
                {
                    newId = lastId + 1; // Increment ID
                }
            }
            newsArticle.NewsArticleId = newId.ToString(); // Convert new ID to string
            newsArticle.CreatedDate = DateTime.Now;
            newsArticle.CreatedById = (short)Convert.ToInt32(createby);

            
                _context.Add(newsArticle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            

            //ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryDesciption", newsArticle.CategoryId);
            //ViewData["CreatedById"] = new SelectList(_context.SystemAccounts, "AccountId", "AccountId", newsArticle.CreatedById);
            //ViewData["Tags"] = new SelectList(_context.Tags, "TagId", "TagName");
            //return View(newsArticle);
        }


        // GET: NewsArticles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var newsArticle =  _newArt.GetNewsArticleById(id);
            if (newsArticle == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", newsArticle.CategoryId);
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            ViewData["UserId"] = HttpContext.Session.GetInt32("UserId");
            return View(newsArticle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("NewsArticleId,NewsTitle,Headline,CreatedDate,NewsContent,NewsSource,CategoryId,NewsStatus,CreatedById,UpdatedById,ModifiedDate")] NewsArticle newsArticle)
        {

            if (id != newsArticle.NewsArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    int userId = (int)HttpContext.Session.GetInt32("UserId");
                    // Cập nhật thông tin bổ sung
                    newsArticle.ModifiedDate = DateTime.Now;
                    newsArticle.UpdatedById = (short)userId;

                     _newArt.UpdateNewsArticle(newsArticle);
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsArticleExists(newsArticle.NewsArticleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", newsArticle.CategoryId);
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            ViewData["UserId"] = HttpContext.Session.GetInt32("UserId");

            return View(newsArticle);
        }
        // GET: NewsArticles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewData["UserId"] = HttpContext.Session.GetInt32("UserId");
            if (id == null)
            {
                return NotFound();
            }

            var newsArticle = _newArt.GetNewsArticleById(id);
            if (newsArticle == null)
            {
                return NotFound();
            }
            if ( newsArticle.CreatedById != userId)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(newsArticle);
        }

        // POST: NewsArticles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var newsArticle = _newArt.GetNewsArticleById(id);
            if (newsArticle != null)
            {
                _newArt.DeleteNewsArticle(id);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool NewsArticleExists(string id)
        {
            return _context.NewsArticles.Any(e => e.NewsArticleId == id);
        }
    }
}
