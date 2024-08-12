using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public record ProductDTO
    {
        public int Id { get; init; }
        public string ProductName { get; init; }    
        public decimal Price { get; init; }
    }
}
