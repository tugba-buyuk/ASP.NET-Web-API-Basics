﻿using Entities.DataTransferObjects;
using Entities.LinkModels;
using Entities.Models;
using Entities.RequestFeatures;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IProductService
    {
        Task<(LinkResponse linkResponse, MetaData metaData)> AllProductsAsync(LinkParameters linkParameters, bool trackChanges);
        Task<ProductDTO> OneProductwithIDAsync(int id, bool trackChanges);
        Task<ProductDTO> CreateProductAsync(ProductDTOForInsertion productDto);
        Task UpdateProductAsync(int id, ProductDTOForUpdate productDto, bool trackChanges);
        Task DeleteProductAsync(int id,bool trackChanges);
        Task<(ProductDTOForUpdate productDto, Product product)> GetOneProductForPatchAsync(int id , bool  trackChanges);
        Task SaveChangesForPatchAsync(ProductDTOForUpdate productDto, Product product);
        Task<List<Product>> GetAllProducts(bool trackChanges);
        Task<IEnumerable<Product>> AllProductsWithCategories(bool trackChanges);

    }
}
