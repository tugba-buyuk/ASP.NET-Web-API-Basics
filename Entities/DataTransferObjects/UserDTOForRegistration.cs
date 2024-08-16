using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public record UserDTOForRegistration
    {
        public String? FirstName { get; init; }
        public String? LastName { get; init; }

        [Required(ErrorMessage ="UserName is required")]
        public String? UserName {  get; init; }

        [Required(ErrorMessage ="Password is required")]
        public String? Password {  get; init; }

        [Required(ErrorMessage = "Email is required")]
        public String? Email { get; init; }

        [Required(ErrorMessage = "PhoneNumber is required")]
        public String? PhoneNumber { get; init; }
        public ICollection<string>? Roles { get; init; }
    }
}
