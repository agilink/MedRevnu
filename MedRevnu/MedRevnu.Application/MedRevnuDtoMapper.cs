using ATI.Authorization.Users;
using ATI.Authorization.Users.Dto;
using ATI.MedRevnu.Application.LafayetteQuota.Dto;
using ATI.MedRevnu.Domain.Entities;
using AutoMapper;


namespace ATI.MedRevnu.Application
{
    internal static class MedRevnuDtoMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            // Product mappings
            configuration.CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CreatorUserName, opt => opt.Ignore()) // Resolved in application service
                .ForMember(dest => dest.LastModifierUserName, opt => opt.Ignore()); // Resolved in application service

            configuration.CreateMap<CreateOrEditProductDto, Product>().ReverseMap();

            // Personnel mappings
            configuration.CreateMap<Personnel, PersonnelDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
                .ForMember(dest => dest.CreatorUserName, opt => opt.Ignore()) // Resolved in application service
                .ForMember(dest => dest.LastModifierUserName, opt => opt.Ignore()); // Resolved in application service

            configuration.CreateMap<CreateOrEditPersonnelDto, Personnel>().ReverseMap();

            // Case mappings
            configuration.CreateMap<Case, CaseDto>()
                .ForMember(dest => dest.TotalRevenue, opt => opt.MapFrom(src => src.GetTotalRevenue()))
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.CaseProducts.Count))
                .ForMember(dest => dest.CreatorUserName, opt => opt.Ignore()) // Resolved in application service
                .ForMember(dest => dest.LastModifierUserName, opt => opt.Ignore()); // Resolved in application service

            configuration.CreateMap<CreateOrEditCaseDto, Case>()
                .ForMember(dest => dest.CaseProducts, opt => opt.Ignore())
                .ReverseMap();

            // CaseProduct mappings
            configuration.CreateMap<CaseProduct, CaseProductDto>()
                .ForMember(dest => dest.Profit, opt => opt.MapFrom(src => src.GetProfit()))
                .ForMember(dest => dest.RevenuePerUnit, opt => opt.MapFrom(src => src.GetRevenuePerUnit()))
                .ForMember(dest => dest.ProductName, opt => opt.Ignore()) // Resolved in application service via joins
                .ForMember(dest => dest.ProductModelNo, opt => opt.Ignore()) // Resolved in application service via joins
                .ForMember(dest => dest.ProductCategory, opt => opt.Ignore()) // Resolved in application service via joins
                .ForMember(dest => dest.CreatorUserName, opt => opt.Ignore()) // Resolved in application service
                .ForMember(dest => dest.LastModifierUserName, opt => opt.Ignore()); // Resolved in application service

            configuration.CreateMap<CreateOrEditCaseProductDto, CaseProduct>().ReverseMap();
        }
    }
}