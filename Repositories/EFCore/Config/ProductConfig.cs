using Entities.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore.Config
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasData(
                new Product { Id = 1,CategoryId=3, ProductName = "Computer", Price = 5000 },
                new Product { Id = 2,CategoryId=3, ProductName = "Keyboard", Price = 300 },
                new Product { Id = 3,CategoryId=3, ProductName = "Mouse", Price = 200 },
                new Product { Id = 4,CategoryId=3, ProductName = "Desk", Price = 4000 }
                );
        }
    }
}
