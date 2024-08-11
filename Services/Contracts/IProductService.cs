using Entities.Models;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IProductService
    {
        IEnumerable<Product> AllProducts(bool trackChanges);
        Product OneProductwithID(int id, bool trackChanges);
        void CreateProduct(Product product);
        void UpdateProduct(int id, Product product, bool trackChanges);
        void DeleteProduct(int id,bool trackChanges);

    }
}
