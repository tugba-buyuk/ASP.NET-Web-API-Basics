using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CategoryManager : ICategoryService
    {
        private readonly IRepositoryManager _manager;

        public CategoryManager(IRepositoryManager manager)
        {
            _manager = manager;
        }

        public Task<IEnumerable<Category>> AllCategoriesAsync(bool trackChanges) =>
            _manager.Category.GetAllCategoriesAsync(trackChanges);

        public Task<Category> OneCategory(int id, bool trackChanges) =>
            _manager.Category.GetOneCategoryAsync(id, trackChanges);
        
    }
}
