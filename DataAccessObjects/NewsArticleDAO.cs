using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjects
{
    public class NewsArticleDAO
    {
        private readonly FunewsManagementContext _context;

        public NewsArticleDAO(FunewsManagementContext context)
        {
            _context = context;
        }

        public List<NewsArticle> GetAllNewsArticles()
        {
            return _context.NewsArticles.Include(n => n.Category).Include(n => n.CreatedBy).Include(n => n.Tags).ToList();
        }

        public NewsArticle? GetNewsArticleById(string id)
        {
            return _context.NewsArticles.Include(n => n.Category).Include(n => n.CreatedBy).Include(n => n.Tags)
                .FirstOrDefault(n => n.NewsArticleId == id);
        }

        public void AddNewsArticle(NewsArticle newsArticle)
        {
            _context.NewsArticles.Add(newsArticle);
            _context.SaveChanges();
        }

        public void UpdateNewsArticle(NewsArticle newsArticle)
        {
            _context.NewsArticles.Update(newsArticle);
            _context.SaveChanges();
        }

        public void DeleteNewsArticle(string id)
        {
            var newsArticle = _context.NewsArticles.Find(id);
            if (newsArticle != null)
            {
                _context.NewsArticles.Remove(newsArticle);
                _context.SaveChanges();
            }
        }
        public List<NewsArticle> SearchNewsArticlesbyStaff(string? query, string? filter, int? userId)
        {
            var newsArticles = _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                .AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                newsArticles = newsArticles.Where(n => n.NewsTitle.Contains(query) ||
                                                       n.Headline.Contains(query) ||
                                                       n.NewsContent.Contains(query) ||
                                                       n.Tags.Any(t => t.TagName.Contains(query)));
            }

            if (!string.IsNullOrEmpty(filter))
            {
                switch (filter.ToLower())
                {
                    case "createdbyme":
                        newsArticles = newsArticles.Where(n => n.CreatedById == userId);
                        break;
                    case "publish":
                        newsArticles = newsArticles.Where(n => n.NewsStatus == true && n.CreatedById == userId);
                        break;
                    case "private":
                        newsArticles = newsArticles.Where(n => n.NewsStatus == false && n.CreatedById == userId);
                        break;
                    default:
                        // Nếu không có filter, chỉ hiển thị bài viết được publish
                        newsArticles = newsArticles.Where(n => n.NewsStatus == true);
                        break;
                }
            }
            else
            {
                // Nếu không có filter, chỉ hiển thị bài viết được publish
                newsArticles = newsArticles.Where(n => n.NewsStatus == true);
            }

            return newsArticles.ToList();
        }

        public List<NewsArticle> SearchNewsArticlesbyAdmin(string? query, string? filter, int? userId)
        {
            var newsArticles = _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                .AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                newsArticles = newsArticles.Where(n => n.NewsTitle.Contains(query) ||
                                                       n.Headline.Contains(query) ||
                                                       n.NewsContent.Contains(query) ||
                                                       n.Tags.ToString().Contains(query));
            }

            if (!string.IsNullOrEmpty(filter))
            {
                switch (filter.ToLower())
                {
                    case "createdbyme":
                        newsArticles = newsArticles.Where(n => n.CreatedById == userId);
                        break;
                    case "publish":
                        newsArticles = newsArticles.Where(n => n.NewsStatus == true);
                        break;
                    case "private":
                        newsArticles = newsArticles.Where(n => n.NewsStatus == false);
                        break;
                }
            }


            return newsArticles.ToList();
        }
    }
}
