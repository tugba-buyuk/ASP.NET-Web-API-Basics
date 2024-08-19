using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _context;
        private readonly Lazy<IProductRepository> _productRepository;
        private readonly Lazy<ICategoryRepository> _categoryRepository;

        public RepositoryManager(RepositoryContext context)
        {
            _context = context;
            _productRepository=new Lazy<IProductRepository>(()=> new ProductRepository(context));
            _categoryRepository=new Lazy<ICategoryRepository>(()=> new CategoryRepository(context));
        }

        public IProductRepository Product =>_productRepository.Value;
        public ICategoryRepository Category =>_categoryRepository.Value;

        public async Task SaveAsync()
        {
           await _context.SaveChangesAsync();
        }
    }
}
