using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjects
{
    public class CategoryDAO
    {
        private readonly FunewsManagementContext _context;

        public CategoryDAO(FunewsManagementContext context)
        {
            _context = context;
        }

        public List<Category> GetCategories()
        {
            return _context.Categories.Include(c => c.ParentCategory).ToList();
        }

        public Category? GetCategoryById(short categoryId)
        {
            return _context.Categories.Include(c => c.ParentCategory)
                .FirstOrDefault(c => c.CategoryId == categoryId);
        }

        public void AddCategory(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void UpdateCategory(Category category)
        {
            var existingCategory = _context.Categories.Find(category.CategoryId);
            if (existingCategory != null)
            {
                existingCategory.CategoryName = category.CategoryName;
                existingCategory.CategoryDesciption = category.CategoryDesciption;
                existingCategory.ParentCategoryId = category.ParentCategoryId;
                existingCategory.IsActive = category.IsActive;
                _context.SaveChanges();
            }
        }

        public void DeleteCategory(short categoryId)
        {
            var category = _context.Categories.Find(categoryId);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }
        }
        public List<Category> SearchCategory(string? query)
        {
            var category = _context.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                category = category.Where(a => a.CategoryName.Contains(query));
            }
            return category.ToList();
        }
    }
}
