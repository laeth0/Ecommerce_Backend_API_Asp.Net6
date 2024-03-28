using AutoMapper;
using Ecommerce.Core.Models;
using Ecommerce.PL.DTO;

namespace Ecommerce.PL.MappingProfiles
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {

            // i want to show the category name in the product
            CreateMap<Product, ProductResponseDTO>()
                .ForMember(dest => dest.Category, option => option.MapFrom(src => src.Category.Name));

            // i dont want to show the products in the category
            CreateMap<Category, CategoryDTO>();


        }
    }
}
