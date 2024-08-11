using Entities.Models;
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


        public IQueryable<Product> GetAllProducts(bool trackChanges)=>FindAll(trackChanges);


        public Product GetOneProductById(int id, bool trackChanges) => FindByCondition((p => p.Id.Equals(id)),trackChanges).SingleOrDefault();

        public void UpdateOneProduct(Product product)=>Update(product);

    }
}
