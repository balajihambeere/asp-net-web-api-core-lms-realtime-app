namespace LearningApiCore.Repositories
{
    using LearningApiCore.DataAccess;
    using LearningApiCore.DataAccess.Models;
    using LearningApiCore.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;


    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDataContext _context;
        public CategoryRepository(AppDataContext context)
        {
            _context = context;
        }

        public async Task<Category> Add(Category category)
        {
            category.CreatedOn = DateTime.Now;
            category.RowId = Guid.NewGuid();
            _context.Category.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> Exists(int id)
        {
            return await _context.Category.AnyAsync(e => e.CategoryId == id);
        }

        public async Task<bool> Exists(string name)
        {
            return await _context.Category.AnyAsync(e => e.Name.Equals(name.ToString(), StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Category> Find(int id)
        {
            var result = await _context.Category.FindAsync(id);
            return result;
        }

        public IEnumerable<Category> GetAll()
        {
            var sourceCollection = _context.Category.Select(x => new Category
            {
                CategoryId = x.CategoryId,
                Name = x.Name,
                IsActive = x.IsActive
            });
            return sourceCollection;
        }

        public IEnumerable<KeyValue> GetkeyValue(bool isAuthenticated = false)
        {
            if (isAuthenticated)
            {
                var sourceCollection = _context.Category.Select(x => new KeyValue
                {
                    Key = x.CategoryId,
                    Value = x.Name,
                    Slug = x.Slug
                });
                return sourceCollection;
            }
            else
            {
                var sourceCollection = _context.Category.Where(x => x.IsActive).Select(x => new KeyValue
                {
                    Key = x.CategoryId,
                    Value = x.Name,
                    Slug = x.Slug
                });
                return sourceCollection;
            }
        }

        public async Task<Category> Update(Category category)
        {
            category.ModifiedOn = DateTime.Now;
            _context.Category.Update(category);
            await _context.SaveChangesAsync();
            return category;

        }
    }
}
