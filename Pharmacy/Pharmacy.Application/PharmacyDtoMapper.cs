using ATI.Admin.Application.Addresses.Dtos;
using ATI.Admin.Application.Companies.Dtos;
using ATI.Admin.Application.CompanyUser.Dtos;
using ATI.Admin.Application.Facilities.Dtos;
using ATI.Admin.Domain.Entities;
using ATI.Authorization.Users;
using ATI.Authorization.Users.Dto;
using ATI.Pharmacy.Application.Alergy.Dtos;
using ATI.Pharmacy.Domain.Entities;
using ATI.Pharmacy.Dtos;
using AutoMapper;


namespace ATI.Pharmacy.Application
{
    internal static class PharmacyDtoMapper
    {
        public static void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<CreateOrEditDoctorDto, Address>().ReverseMap();
            configuration.CreateMap<CreateOrEditPatientDto, Address>().ReverseMap();
            configuration.CreateMap<AddressDto, Address>().ReverseMap();
            configuration.CreateMap<StateDto, State>().ReverseMap();
            configuration.CreateMap<CreateOrEditPatientDto, Insurance>().ReverseMap();
            configuration.CreateMap<CreateOrEditDoctorDto, Doctor>().ReverseMap();
            configuration.CreateMap<CreateOrEditDoctorDto, UserEditDto>().ReverseMap();
            configuration.CreateMap<CreateOrEditDoctorDto, User>().ReverseMap();
            configuration.CreateMap<CreateOrEditUserInputDto, User>().ReverseMap();
            configuration.CreateMap<CreateOrEditPrescriptionDto, Prescription>().ReverseMap();
            configuration.CreateMap<PrescriptionDto, Prescription>().ReverseMap();
            configuration.CreateMap<CreateOrEditPatientDto, Patient>().ReverseMap();
            configuration.CreateMap<CreateOrEditPatientDto, User>().ReverseMap();
            configuration.CreateMap<EmployeeListDto, User>().ReverseMap();
            configuration.CreateMap<PatientDto, Patient>().ReverseMap();
            configuration.CreateMap<PatientAllergyDto, PatientAllergy>().ReverseMap();
            configuration.CreateMap<AllergyDto, Allergy>().ReverseMap();
            configuration.CreateMap<CreateOrEditUserDto, User>().ReverseMap();
            configuration.CreateMap<DoctorDto, Doctor>().ReverseMap();
            configuration.CreateMap<UserDto, User>().ReverseMap();
            configuration.CreateMap<PrescriptionItemDto, PrescriptionItem>().ReverseMap();
            configuration.CreateMap<MedicationDto, Medication>().ReverseMap();
            configuration.CreateMap<UserCompanyDto, UserCompany>().ReverseMap();
            configuration.CreateMap<CompanyDto, Company>().ReverseMap();
            configuration.CreateMap<FacilityDto, Facility>().ReverseMap();


        }
    }
}