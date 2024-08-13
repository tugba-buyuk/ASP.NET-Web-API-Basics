using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;

namespace first_Application.Utilities.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<ProductDTOForUpdate, Product>().ReverseMap();
            CreateMap<Product, ProductDTO>();
            CreateMap<ProductDTOForInsertion, Product>();

        }

    }
}
