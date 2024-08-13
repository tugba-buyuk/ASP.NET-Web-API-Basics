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

        public async Task<IEnumerable<ProductDTO>> AllProductsAsync(bool trackChanges)
        {
            var products= await _manager.Product.GetAllProductsAsync(trackChanges);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);

        }

        public async Task<ProductDTO>CreateProductAsync(ProductDTOForInsertion productDto)
        {
            var entity = _mapper.Map<Product>(productDto);
            _manager.Product.CreateOneProduct(entity);
            await _manager.SaveAsync();
            return _mapper.Map<ProductDTO>(entity);
        }

        public async Task DeleteProductAsync(int id, bool trackChanges)
        {
            var product= await GetOneProductByIdAndCheckExsist(id,trackChanges);
            _manager.Product.DeleteOneProduct(product);
            await _manager.SaveAsync();
        }

        public async Task<(ProductDTOForUpdate productDto, Product product)> GetOneProductForPatchAsync(int id, bool trackChanges)
        {
            var product = await GetOneProductByIdAndCheckExsist(id, trackChanges);
            var productDtoForUpdate = _mapper.Map<ProductDTOForUpdate>(product);
            return (productDtoForUpdate, product);
        }

        public async Task<ProductDTO> OneProductwithIDAsync(int id, bool trackChanges)
        {
            var entity = await GetOneProductByIdAndCheckExsist(id, trackChanges);
            return _mapper.Map<ProductDTO>(entity);
        }

        public async Task SaveChangesForPatchAsync(ProductDTOForUpdate productDto, Product product)
        {
            _mapper.Map(productDto, product);
            await _manager.SaveAsync();
        }

        public async Task UpdateProductAsync(int id, ProductDTOForUpdate productDto, bool trackChanges)
        {
            var entity = await GetOneProductByIdAndCheckExsist(id, trackChanges);
            var product = _mapper.Map<Product>(productDto);

            _manager.Product.UpdateOneProduct(product);
            await  _manager.SaveAsync();
        }

        private async Task<Product> GetOneProductByIdAndCheckExsist(int id , bool trackChanges)
        {
            var product = await _manager.Product.GetOneProductByIdAsync(id, trackChanges);
            if (product is null)
            {
                throw new ProductNotFoundException(id);
            }
            return product;
        }
    }
}
