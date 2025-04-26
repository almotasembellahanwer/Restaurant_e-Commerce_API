using AllFoods.Core.Domain.Entities;
using AllFoods.Core.Domain.RepositoryContracts;
using AllFoods.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
namespace AllFoods.Infrastructure.Repositories
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoriesRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Category> AddCategory(Category category)
        {
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            return category;
        }



        public async Task<List<Category>> GetAllCategories()
        {
            return await _db.Categories.ToListAsync();
        }

        public async Task<Category?> GetCategoryByCategoryID(Guid? categoryID)
        {
            return await _db.Categories.FirstOrDefaultAsync(temp => temp.CategoryID == categoryID);
        }

        public async Task<Category?> GetCategoryByCategoryName(string? categoryName)
        {
            return await _db.Categories.FirstOrDefaultAsync(temp => temp.CategoryName == categoryName);
        }

        public async Task<Category> UpdateCategory(Category category)
        {
            _db.Categories.Update(category);
            await _db.SaveChangesAsync();
            return category;
        }
        public async Task<bool> DeleteCategory(Guid? categoryID)
        {
            Category? existingCategory = await GetCategoryByCategoryID(categoryID);
            if (existingCategory is null)
                return false;
            _db.Categories.Remove(existingCategory);
            int affectedRows = await _db.SaveChangesAsync();
            return affectedRows > 0;
        }
    }
}
