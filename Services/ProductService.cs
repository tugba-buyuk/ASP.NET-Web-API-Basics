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
    public class ProductService : IProductService
    {
        private readonly IRepositoryManager _manager;

        public ProductService(IRepositoryManager manager)
        {
            _manager = manager;
        }

        public IEnumerable<Product> AllProducts(bool trackChanges)
        {
            return _manager.Product.GetAllProducts(trackChanges);
        }

        public void CreateProduct(Product product)
        {
            if(product is null)
            {
                throw new ArgumentNullException(nameof(product));
            }
            _manager.Product.CreateOneProduct(product);
            _manager.Save();

        }

        public void DeleteProduct(int id, bool trackChanges)
        {
            var product=_manager.Product.GetOneProductById(id, trackChanges);
            if(product is null)
            {
                throw new Exception($"Product with {id} id is not found");
            }
            _manager.Product.DeleteOneProduct(product);
            _manager.Save();
        }

        public Product OneProductwithID(int id, bool trackChanges)
        {
            var entity = _manager.Product.GetOneProductById(id,trackChanges);
            if(entity is null)
            {
                throw new Exception($"Product with {id} is not found");
            }
            return entity;
        }

        public void UpdateProduct(int id, Product product, bool trackChanges)
        {
            var entity = _manager.Product.GetOneProductById(id, trackChanges);
            if(entity is null)
            {
                throw new Exception($"Product with {id} id is not found");
            }
            entity.ProductName = product.ProductName;
            entity.Price = product.Price;
            _manager.Product.UpdateOneProduct(entity);
            _manager.Save();
        }
    }
}
