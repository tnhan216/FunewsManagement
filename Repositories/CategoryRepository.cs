using BusinessObjects;
using DataAccessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CategoryDAO _dao;

        public CategoryRepository(FunewsManagementContext context)
        {
            _dao = new CategoryDAO(context);
        }

        public List<Category> GetCategories() => _dao.GetCategories();

        public Category? GetCategoryById(short categoryId) => _dao.GetCategoryById(categoryId);

        public void AddCategory(Category category) => _dao.AddCategory(category);

        public void UpdateCategory(Category category) => _dao.UpdateCategory(category);

        public void DeleteCategory(short categoryId) => _dao.DeleteCategory(categoryId);
        public List<Category> SearchCategory(string? query) => _dao.SearchCategory(query);
    }
}
