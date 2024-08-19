using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(RepositoryContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(bool trakChanges)
        {
            return await FindAll(trakChanges).OrderBy(c=>c.CategoryName).ToListAsync();
        }

        public async Task<Category> GetOneCategoryAsync(int id, bool trackChanges)
        {
            return await FindByCondition(c => c.CategoryId.Equals(id), trackChanges).FirstOrDefaultAsync();
        }

        public void CreateOneCategory(Category category) => Create(category);
  
        public void DeleteCateory(Category category) => Delete(category);

        public void UpdateCateory(Category category) => Update(category);

    }
}
