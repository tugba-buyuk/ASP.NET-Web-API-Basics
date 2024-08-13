using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IProductRepository :IRepositoryBase<Product>
    {
        Task<IEnumerable<Product>> GetAllProductsAsync(bool trackChanges);
        Task<Product> GetOneProductByIdAsync(int id, bool trackChanges);
        void CreateOneProduct(Product product);
        void UpdateOneProduct(Product product);
        void DeleteOneProduct(Product product);

    }
}
