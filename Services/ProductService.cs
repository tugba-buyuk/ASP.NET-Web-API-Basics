using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IRepositoryManager _manager;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;
        public ProductService(IRepositoryManager manager, ILoggerService logger, IMapper mapper)
        {
            _manager = manager;
            _logger = logger;
            _mapper = mapper;
        }

        public IEnumerable<ProductDTO> AllProducts(bool trackChanges)
        {
            var products= _manager.Product.GetAllProducts(trackChanges);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);

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

        public void UpdateProduct(int id, ProductDTOForUpdate productDto, bool trackChanges)
        {
            var entity = _manager.Product.GetOneProductById(id, trackChanges);
            if(entity is null)
            {
                throw new ProductNotFoundException(id);
            }
            var product= _mapper.Map<Product>(productDto);

            _manager.Product.UpdateOneProduct(product);
            _manager.Save();
        }
    }
}
