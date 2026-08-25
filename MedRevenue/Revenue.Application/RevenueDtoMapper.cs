using AutoMapper;
using ATI.Revenue.Application.Products.Dtos;
using ATI.Revenue.Application.ProcedureTypes.Dtos;
using ATI.Revenue.Application.ProcedureTransactions.Dtos;
using ATI.Revenue.Application.HospitalProductPrices.Dtos;
using ATI.Revenue.Application.ProductQuotas.Dtos;
using ATI.Revenue.Domain.Entities;

namespace ATI.Revenue.Application
{
    internal class RevenueDtoMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            // Product mappings
            configuration.CreateMap<Product, ProductDto>()
                .ForMember(dto => dto.ProductCategoryName,
                    opt => opt.MapFrom(src => src.ProductCategory != null ? src.ProductCategory.Name : string.Empty));

            configuration.CreateMap<CreateOrEditProductDto, Product>();

            // ProcedureType mappings
            configuration.CreateMap<ProcedureType, ProcedureTypeDto>();
            configuration.CreateMap<CreateOrEditProcedureTypeDto, ProcedureType>();

            // ProcedureTransaction (case) mappings. The device lines are projected
            // explicitly in the app service, so they are not mapped here.
            configuration.CreateMap<ProcedureTransaction, ProcedureTransactionDto>()
                .ForMember(dto => dto.HospitalName,
                    opt => opt.MapFrom(src => src.Hospital != null ? src.Hospital.FacilityName : string.Empty))
                .ForMember(dto => dto.PhysicianName,
                    opt => opt.MapFrom(src => src.Physician != null ?
                        (src.Physician.FIRST_NAME + " " + src.Physician.LAST_NAME).Trim() : string.Empty))
                .ForMember(dto => dto.DeviceCount, opt => opt.MapFrom(src => src.Products.Count))
                .ForMember(dto => dto.TotalUnits, opt => opt.Ignore())
                .ForMember(dto => dto.ProductSummary, opt => opt.Ignore())
                .ForMember(dto => dto.Products, opt => opt.Ignore());

            configuration.CreateMap<CreateOrEditProcedureTransactionDto, ProcedureTransaction>()
                .ForMember(ent => ent.Hospital, opt => opt.Ignore())
                .ForMember(ent => ent.Physician, opt => opt.Ignore())
                .ForMember(ent => ent.Products, opt => opt.Ignore());

            // ProductQuota mappings
            configuration.CreateMap<ProductQuota, ProductQuotaDto>()
                .ForMember(dto => dto.HospitalName,
                    opt => opt.MapFrom(src => src.Hospital != null ? src.Hospital.FacilityName : string.Empty))
                .ForMember(dto => dto.ProductCategoryName,
                    opt => opt.MapFrom(src => src.ProductCategory != null ? src.ProductCategory.Name : string.Empty))
                .ForMember(dto => dto.ProductName,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty));

            configuration.CreateMap<CreateOrEditProductQuotaDto, ProductQuota>()
                .ForMember(ent => ent.Hospital, opt => opt.Ignore())
                .ForMember(ent => ent.ProductCategory, opt => opt.Ignore())
                .ForMember(ent => ent.Product, opt => opt.Ignore());

            // HospitalProductPrice mappings
            configuration.CreateMap<HospitalProductPrice, HospitalProductPriceDto>()
                .ForMember(dto => dto.HospitalName,
                    opt => opt.MapFrom(src => src.Hospital != null ? src.Hospital.FacilityName : string.Empty))
                .ForMember(dto => dto.ProductName,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty));

            configuration.CreateMap<CreateOrEditHospitalProductPriceDto, HospitalProductPrice>()
                .ForMember(ent => ent.Hospital, opt => opt.Ignore())
                .ForMember(ent => ent.Product, opt => opt.Ignore());
        }
    }
}