using AutoMapper;
using ATI.Revenue.Application.Cases.Dtos;
using ATI.Revenue.Application.Products.Dtos;
using ATI.Revenue.Application.ProcedureTypes.Dtos;
using ATI.Revenue.Application.ProcedureQuotas.Dtos;
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
            // Case mappings
            configuration.CreateMap<Case, CaseDto>()
                .ForMember(dto => dto.CaseProducts, opt => opt.MapFrom(src => src.CaseProducts))
                .ForMember(dto => dto.ProcedureTypeName,
                    opt => opt.MapFrom(src => src.ProcedureType != null ? src.ProcedureType.Name : string.Empty));

            configuration.CreateMap<CreateOrEditCaseDto, Case>()
                .ForMember(ent => ent.CaseProducts, opt => opt.Ignore())
                .ForMember(ent => ent.ProcedureType, opt => opt.Ignore());

            // CaseProduct mappings
            configuration.CreateMap<CaseProduct, CaseProductDto>()
                .ForMember(dto => dto.ProductName, opt => opt.MapFrom(src => src.Product.Name));

            // Product mappings
            configuration.CreateMap<Product, ProductDto>()
                .ForMember(dto => dto.ProductCategoryName,
                    opt => opt.MapFrom(src => src.ProductCategory != null ? src.ProductCategory.Name : string.Empty));

            configuration.CreateMap<CreateOrEditProductDto, Product>();

            // ProcedureType mappings
            configuration.CreateMap<ProcedureType, ProcedureTypeDto>();
            configuration.CreateMap<CreateOrEditProcedureTypeDto, ProcedureType>();

            // ProcedureQuota mappings
            configuration.CreateMap<ProcedureQuota, ProcedureQuotaDto>()
                .ForMember(dto => dto.ProcedureTypeName,
                    opt => opt.MapFrom(src => src.ProcedureType != null ? src.ProcedureType.Name : string.Empty));
            configuration.CreateMap<CreateOrEditProcedureQuotaDto, ProcedureQuota>();

            // ProcedureTransaction mappings
            configuration.CreateMap<ProcedureTransaction, ProcedureTransactionDto>()
                .ForMember(dto => dto.HospitalName,
                    opt => opt.MapFrom(src => src.Hospital != null ? src.Hospital.FacilityName : string.Empty))
                .ForMember(dto => dto.PhysicianName,
                    opt => opt.MapFrom(src => src.Physician != null ?
                        (src.Physician.FIRST_NAME + " " + src.Physician.LAST_NAME).Trim() : string.Empty))
                .ForMember(dto => dto.ProductName,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dto => dto.ProductCode,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductCode : string.Empty));

            configuration.CreateMap<CreateOrEditProcedureTransactionDto, ProcedureTransaction>()
                .ForMember(ent => ent.Hospital, opt => opt.Ignore())
                .ForMember(ent => ent.Physician, opt => opt.Ignore())
                .ForMember(ent => ent.Product, opt => opt.Ignore());

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