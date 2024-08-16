using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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
        private readonly Lazy<AuthenticationManager> _authenticationManager;
        public ServiceManager(IRepositoryManager manager,ILoggerService logger,IMapper mapper, IProductLinks productLinks, UserManager<User> userManager, IConfiguration configuration)
        {
            _productService=new Lazy<ProductService>(() => new ProductService(manager,logger,mapper, productLinks));
            _authenticationManager=new Lazy<AuthenticationManager>(() =>  new AuthenticationManager(mapper,configuration,userManager,logger));
        }
        public IProductService ProductService => _productService.Value;

        public IAuthenticationService AuthenticationService => _authenticationManager.Value;
    }
}
