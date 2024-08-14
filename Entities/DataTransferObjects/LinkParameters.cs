using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.DataTransferObjects
{
    public record LinkParameters
    {
        public ProductParameters ProductParameters { get; init; }
        public HttpContext HttpContext { get; init; }
    }
}
