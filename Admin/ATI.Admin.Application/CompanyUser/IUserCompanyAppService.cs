using Abp.Application.Services;
using ATI.Admin.Domain.Entities;
using ATI.Authorization.Users;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATI.Admin.Application
{
    public interface IUserCompanyAppService : IApplicationService
    {
        UserCompany GetById(int id);
        List<long> GetUsersByFacility(int facilityId);

        List<long> GetUsersByCompany(int? companyId);

        List<long> GetOtherUsersOfCompany(long userId);

        UserCompany GetUserCompany(long? userId);
        List<UserCompany> GetUserCompanies(long? userId);

        int InsertUserCompany(long? userId, long? patientUserId);
        int UpdateUserCompany(UserCompany userCompany);
        int InsertUserCompany(UserCompany userCompany);
        void DeleteUserCompany(UserCompany userCompany);
        void DeleteUserUserCompany(int userId);
        Task<List<SelectListItem>> CompanySelectList(int userId, bool CompanySelectList = true);
        Task<List<SelectListItem>> FacilitySelectList(int userId, bool excludeDefault = true);
        Facility GetFacilityByCompany(int companyId);
        Company GetCompanyByFacility(int facilityId);
        Task<List<SelectListItem>> UserFacilitySelectList(int userId, bool excludewatermark = false);
        void ClearRepoCache(UserCompany userCompany);
    }
}
