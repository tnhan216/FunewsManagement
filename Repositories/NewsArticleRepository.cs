using BusinessObjects;
using DataAccessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        private readonly NewsArticleDAO _dao;

        public NewsArticleRepository(FunewsManagementContext context)
        {
            _dao = new NewsArticleDAO(context);
        }

        public List<NewsArticle> GetAllNewsArticles() => _dao.GetAllNewsArticles();

        public NewsArticle? GetNewsArticleById(string id) => _dao.GetNewsArticleById(id);

        public void AddNewsArticle(NewsArticle newsArticle) => _dao.AddNewsArticle(newsArticle);

        public void UpdateNewsArticle(NewsArticle newsArticle) => _dao.UpdateNewsArticle(newsArticle);

        public void DeleteNewsArticle(string id) => _dao.DeleteNewsArticle(id);
        public List<NewsArticle> SearchNewsArticlesbyStaff(string? query, string? filter, int? userId) => _dao.SearchNewsArticlesbyStaff(query, filter, userId);
        public List<NewsArticle> SearchNewsArticlesbyAdmin(string? query, string? filter, int? userId) => _dao.SearchNewsArticlesbyAdmin(query, filter, userId);
    }
}
