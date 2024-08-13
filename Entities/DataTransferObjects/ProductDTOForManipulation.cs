using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public abstract record ProductDTOForManipulation
    {
        [Required(ErrorMessage="ProductName is a required field.")]
        [MinLength(2,ErrorMessage="ProductName must consist of at least 2 characters.")]
        [MaxLength(50,ErrorMessage ="ProductName must consist of maximum 50 characters.")]
        public String ProductName {  get; init; }

        [Required(ErrorMessage ="Price is a required field.")]
        [Range(10,100000)]
        public decimal Price { get; init; }
    }
}
