using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Catalog.Application.Responses;
using Catalog.Core.Entities;

namespace Catalog.Application.Mappers
{
    public class ProductMappingProfile:Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<ProductBrand,BrandResponseDto>().ReverseMap();
            CreateMap<Product,ProductResponseDto>().ReverseMap();
            CreateMap<ProductType,TypeResponseDto>().ReverseMap();
        }
    }
}
