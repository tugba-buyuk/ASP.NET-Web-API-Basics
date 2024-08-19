using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface ICategoryRepository:IRepositoryBase<Category>
    {
        Task <IEnumerable<Category>> GetAllCategoriesAsync (bool trakChanges);
        Task<Category> GetOneCategoryAsync (int id, bool trackChanges);
        void CreateOneCategory(Category category);
        void UpdateCateory(Category category);
        void DeleteCateory(Category category);
    }
}
