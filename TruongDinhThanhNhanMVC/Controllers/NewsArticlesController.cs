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
using NuGet.Packaging;

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
            int? Id= HttpContext.Session.GetInt32("UserId");
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            ViewData["UserId"] = HttpContext.Session.GetInt32("UserId");
            if(Id!= null && Id == 9999)
            {
                if (_newArt.SearchNewsArticlesbyAdmin(search, filter, userId) != null)
                {
                    return View(_newArt.SearchNewsArticlesbyAdmin(search, filter, userId));
                }
            }
            if (_newArt.SearchNewsArticlesbyStaff(search, filter, userId) != null)
            {
                return View(_newArt.SearchNewsArticlesbyStaff(search, filter, userId));
            }
            var newArt= _newArt.GetAllNewsArticles().Where(n=>n.NewsStatus==true);
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
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }
            ViewData["UserId"] = HttpContext.Session.GetString("UserId");
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewBag.Tags = _context.Tags.ToList();
            return View();
        }

        // POST: NewsArticles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewsArticle newsArticle, int[] selectedTags)
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

            newsArticle.Tags = _context.Tags.Where(t => selectedTags.Contains(t.TagId)).ToList();

            _context.Add(newsArticle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


        // GET: NewsArticles/Edit/5
        [HttpGet]
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
            ViewBag.Tags = _context.Tags.ToList();
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", newsArticle.CategoryId);
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            ViewData["UserId"] = HttpContext.Session.GetInt32("UserId");
            return View(newsArticle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("NewsArticleId,NewsTitle,Headline,CreatedDate,NewsContent,NewsSource,CategoryId,NewsStatus,CreatedById,UpdatedById,ModifiedDate")] NewsArticle newsArticle, int[] selectedTags)
        {

            if (id != newsArticle.NewsArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy bài viết gốc từ DB để cập nhật
                    var existingArticle = await _context.NewsArticles
                        .Include(n => n.Tags) // Load danh sách Tags cũ
                        .FirstOrDefaultAsync(n => n.NewsArticleId == newsArticle.NewsArticleId);

                    if (existingArticle == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin từ form vào bản ghi gốc
                    existingArticle.NewsTitle = newsArticle.NewsTitle;
                    existingArticle.Headline = newsArticle.Headline;
                    existingArticle.NewsContent = newsArticle.NewsContent;
                    existingArticle.NewsSource = newsArticle.NewsSource;
                    existingArticle.CategoryId = newsArticle.CategoryId;
                    existingArticle.NewsStatus = newsArticle.NewsStatus;
                    existingArticle.ModifiedDate = DateTime.Now;
                    existingArticle.UpdatedById = (short)HttpContext.Session.GetInt32("UserId");

                    // Xóa toàn bộ tag cũ
                    existingArticle.Tags.Clear();
                    await _context.SaveChangesAsync(); // Lưu ngay để tránh lỗi khóa chính

                    // Thêm lại tag mới
                    var tagsToAdd = await _context.Tags.Where(t => selectedTags.Contains(t.TagId)).ToListAsync();
                    existingArticle.Tags.AddRange(tagsToAdd);

                    // Lưu thay đổi vào DB
                    await _context.SaveChangesAsync();
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
                var existingArticle = await _context.NewsArticles
                        .Include(n => n.Tags) // Load danh sách Tags cũ
                        .FirstOrDefaultAsync(n => n.NewsArticleId == newsArticle.NewsArticleId);
                existingArticle.Tags.Clear();
                await _context.SaveChangesAsync(); // Lưu ngay để tránh lỗi khóa chính
                _newArt.DeleteNewsArticle(id);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool NewsArticleExists(string id)
        {
            return _context.NewsArticles.Any(e => e.NewsArticleId == id);
        }
        [HttpPost]
        public IActionResult Approve(string id)
        {
            var article = _newArt.GetNewsArticleById(id);
            if (article == null)
            {
                return NotFound();
            }

            article.NewsStatus = true; 
            _newArt.UpdateNewsArticle(article);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Reject(string id)
        {
            var article = _newArt.GetNewsArticleById(id);
            if (article == null)
            {
                return NotFound();
            }

            article.NewsStatus = false; 
            _newArt.UpdateNewsArticle(article);
            return RedirectToAction(nameof(Index));
        }

    }
}
