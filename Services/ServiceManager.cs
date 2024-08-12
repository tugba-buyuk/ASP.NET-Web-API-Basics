using Repositories.Contracts;
using Repositories.EFCore;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<ProductService> _productService;
        public ServiceManager(IRepositoryManager manager,ILoggerService logger)
        {
            _productService=new Lazy<ProductService>(() => new ProductService(manager,logger));
        }
        public IProductService ProductService => _productService.Value;
    }
}
