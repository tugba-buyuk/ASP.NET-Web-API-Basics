﻿using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using Repositories.EFCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(RepositoryContext context) : base(context)
        {
        }

        public async Task<List<Product>> AllProducts(bool trackChanges)
        {
            return await FindAll(trackChanges)
                .OrderBy(p=>p.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> AllProductsWithCategoriesAsync(bool trackChanges)
        {
            return await FindAll(trackChanges)
                .Include(p=>p.Category)
                .OrderBy(p=>p.Id)
                .ToArrayAsync();
        }

        public void CreateOneProduct(Product product)=>Create(product);


        public void DeleteOneProduct(Product product) =>Delete(product);


        public async Task<PagedList<Product>> GetAllProductsAsync(ProductParameters productParameters ,bool trackChanges)
        {
            var products = await FindAll(trackChanges)
                .FilterProducts(productParameters.MinPrice,productParameters.MaxPrice)
                .Search(productParameters.SearchTerm)
                .Sort(productParameters.OrderBy)
                .ToListAsync();

            return PagedList<Product>
                .ToPagedList(products, 
                productParameters.pageNumber, 
                productParameters.PageSize);

        }
            
        public async Task<Product> GetOneProductByIdAsync(int id, bool trackChanges) => 
            await FindByCondition
            ((p => p.Id.Equals(id))
                ,trackChanges)
            .SingleOrDefaultAsync();

        public void UpdateOneProduct(Product product)=>Update(product);

    }
}
