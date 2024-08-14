using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Entities.RequestFeatures;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
        private readonly IDataShaper<ProductDTO> _shaper;
        private IRepositoryManager manager;
        private ILoggerService logger;
        private IMapper mapper;

        public ProductService(IRepositoryManager manager, ILoggerService logger, IMapper mapper)
        {
            this.manager = manager;
            this.logger = logger;
            this.mapper = mapper;
        }

        public ProductService(IRepositoryManager manager, ILoggerService logger, IMapper mapper, IDataShaper<ProductDTO> shaper)
        {
            _manager = manager;
            _logger = logger;
            _mapper = mapper;
            _shaper = shaper;
        }

        public async Task<(IEnumerable<ExpandoObject> productDto, MetaData metaData)> AllProductsAsync(ProductParameters productParameters, bool trackChanges)
        {
            if (!productParameters.ValidPriceRange)
            {
                throw new PriceOutofRangeBadRequestException();
            }
            var productsWithMetaData= await _manager.Product.GetAllProductsAsync(productParameters, trackChanges);
            var productDto=  _mapper.Map<IEnumerable<ProductDTO>>(productsWithMetaData);
            var shapeedData = _shaper.ShapeData(productDto, productParameters.Fields);
            return (shapeedData, productsWithMetaData.MetaData);
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
