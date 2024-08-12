using Entities.Exceptions;
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
        private readonly ILoggerService _logger;
        public ProductService(IRepositoryManager manager, ILoggerService logger)
        {
            _manager = manager;
            _logger = logger;
        }

        public IEnumerable<Product> AllProducts(bool trackChanges)
        {
            return _manager.Product.GetAllProducts(trackChanges);
        }

        public void CreateProduct(Product product)
        {
            _manager.Product.CreateOneProduct(product);
            _manager.Save();

        }

        public void DeleteProduct(int id, bool trackChanges)
        {
            var product=_manager.Product.GetOneProductById(id, trackChanges);
            if(product is null)
            {
                throw new ProductNotFoundException(id);
            }
            _manager.Product.DeleteOneProduct(product);
            _manager.Save();
        }

        public Product OneProductwithID(int id, bool trackChanges)
        {
            var entity = _manager.Product.GetOneProductById(id,trackChanges);
            if(entity is null)
            {
                throw new ProductNotFoundException(id);
            }
            return entity;
        }

        public void UpdateProduct(int id, Product product, bool trackChanges)
        {
            var entity = _manager.Product.GetOneProductById(id, trackChanges);
            if(entity is null)
            {
                throw new ProductNotFoundException(id);
            }
            entity.ProductName = product.ProductName;
            entity.Price = product.Price;
            _manager.Product.UpdateOneProduct(entity);
            _manager.Save();
        }
    }
}
