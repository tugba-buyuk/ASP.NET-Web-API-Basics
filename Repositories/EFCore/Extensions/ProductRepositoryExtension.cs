using Entities.Models;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;


namespace Repositories.EFCore.Extensions
{
    public static class ProductRepositoryExtension
    {
        public static IQueryable<Product> FilterProducts(this IQueryable<Product> products,
            uint minPrice, uint maxPrice) =>
            products.Where(product =>
            product.Price >= minPrice &&
            product.Price <= maxPrice);

        public static IQueryable<Product> Search(this IQueryable<Product> products, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return products;
            }
            var lowerTerm=searchTerm.Trim().ToLower();
            return products.Where(p=>
            p.ProductName
            .ToLower()
            .Contains(lowerTerm));
        }

        public static IQueryable<Product> Sort (this IQueryable<Product> products, string orderByQueryString)
        {
            if(string.IsNullOrWhiteSpace(orderByQueryString))
            {
                return products.OrderBy(p=>p.Id);   
            }
            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Product>(orderByQueryString);
            if (orderQuery is null)
                return products.OrderBy(p=>p.Id);
            return products.OrderBy(orderQuery);

        }
    }
}
