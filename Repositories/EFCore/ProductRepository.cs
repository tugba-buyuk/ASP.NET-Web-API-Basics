using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
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

        public void CreateOneProduct(Product product)=>Create(product);


        public void DeleteOneProduct(Product product) =>Delete(product);


        public async Task<IEnumerable<Product>> GetAllProductsAsync(bool trackChanges)=>
            await FindAll(trackChanges).ToListAsync();


        public async Task<Product> GetOneProductByIdAsync(int id, bool trackChanges) => 
            await FindByCondition((p => p.Id.Equals(id)),trackChanges).SingleOrDefaultAsync();

        public void UpdateOneProduct(Product product)=>Update(product);

    }
}
