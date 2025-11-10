using AutoMapper;
using ATI.Revenue.Application.Cases.Dtos;
using ATI.Revenue.Application.Products.Dtos;
using ATI.Revenue.Domain.Entities;

namespace ATI.Revenue.Application
{
    internal class RevenueDtoMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            // Case mappings
            configuration.CreateMap<Case, CaseDto>()
                .ForMember(dto => dto.CaseProducts, opt => opt.MapFrom(src => src.CaseProducts));
            
            configuration.CreateMap<CreateOrEditCaseDto, Case>()
                .ForMember(ent => ent.CaseProducts, opt => opt.Ignore());

            // CaseProduct mappings
            configuration.CreateMap<CaseProduct, CaseProductDto>()
                .ForMember(dto => dto.ProductName, opt => opt.MapFrom(src => src.Product.Name));

            // Product mappings
            configuration.CreateMap<Product, ProductDto>()
                .ForMember(dto => dto.ProductCategoryName, 
                    opt => opt.MapFrom(src => src.ProductCategory != null ? src.ProductCategory.Name : string.Empty));
            
            configuration.CreateMap<CreateOrEditProductDto, Product>();
        }
    }
}