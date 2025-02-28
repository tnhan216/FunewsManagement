using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ICategoryRepository
    {
        List<Category> GetCategories();
        Category? GetCategoryById(short categoryId);
        void AddCategory(Category category);
        void UpdateCategory(Category category);
        void DeleteCategory(short categoryId);
        public List<Category> SearchCategory(string? query);
    }
}
