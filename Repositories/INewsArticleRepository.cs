using BusinessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public interface INewsArticleRepository
    {
        List<NewsArticle> GetAllNewsArticles();
        NewsArticle? GetNewsArticleById(string id);
        void AddNewsArticle(NewsArticle newsArticle);
        void UpdateNewsArticle(NewsArticle newsArticle);
        void DeleteNewsArticle(string id);
        public List<NewsArticle> SearchNewsArticlesbyStaff(string? query, string? filter, int? userId);
        public List<NewsArticle> SearchNewsArticlesbyAdmin(string? query, string? filter, int? userId);
    }
}
