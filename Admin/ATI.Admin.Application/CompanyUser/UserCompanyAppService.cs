using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Runtime.Caching;
using ATI.Admin.Domain.Entities;
using ATI.Authorization.Users;
using ATI.Configuration;

using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATI.Admin.Application
{
    public class UserCompanyAppService : ATIAppServiceBase, IUserCompanyAppService
    {
        private readonly IRepository<UserCompany> _userCompanyRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<Facility> _facilityRepository;
        private readonly IAppConfigurationAccessor _appConfigurationAccessor;

        public UserCompanyAppService(IRepository<UserCompany> userCompanyRepository,
            IRepository<Company> companyRepository,
            IRepository<Facility> facilityRepository,
            IAppConfigurationAccessor appConfigurationAccessor)
        {
            _userCompanyRepository = userCompanyRepository;
            _companyRepository = companyRepository;
            _facilityRepository = facilityRepository;
            _appConfigurationAccessor = appConfigurationAccessor;
        }
        public UserCompany GetById(int id)
        {
            return _userCompanyRepository.GetAll().WhereIf(id != null, i => i.Id == id).FirstOrDefault();
        }

        public List<long> GetOtherUsersOfCompany(long userId)
        {
            var query = _userCompanyRepository.GetAll();
            if (userId != null) query = query.Where(i => i.UserId == userId);
            var list = query.ToList();
            var facilityId = (query.FirstOrDefault(a => a.IsDefaultFacility) ?? query.FirstOrDefault())?.FacilityId;
            return GetUsersByFacility(facilityId ?? 0);
        }

        public List<long> GetUsersByCompany(int? companyId)
        {
            if (companyId == 0) return new List<long> { };
            return _userCompanyRepository.GetAll().WhereIf(companyId != null, i => i.CompanyId == companyId).Select(i => (long)i.UserId).ToList();
        }

        public List<long> GetUsersByFacility(int facilityId)
        {
            return _userCompanyRepository.GetAll().WhereIf(facilityId != null && facilityId > 0, i => i.FacilityId == facilityId).Select(i => (long)i.UserId).ToList();
        }

        public UserCompany GetUserCompany(long? userId)
        {
            var userCompanies = _userCompanyRepository.GetAllIncluding(a => a.Facility, b => b.Company, c => c.Facility.Address, d => d.Facility.Address.State).Where(a => a.UserId == userId);
            return userCompanies.FirstOrDefault(a => a.IsDefaultFacility) ?? userCompanies.FirstOrDefault(a => a.UserId == userId);
        }
        public List<UserCompany> GetUserCompanies(long? userId)
        {
            var userCompanies = _userCompanyRepository.GetAllIncluding(a => a.Facility, b => b.Company, c => c.Facility.Address);
            return userCompanies.Where(a => a.UserId == userId).Distinct().ToList();
        }

        public int InsertUserCompany(long? userId, long? patientUserId)
        {
            var userCompanies = _userCompanyRepository.GetAll();
            var userCompany = userCompanies.FirstOrDefault(a => a.UserId == userId && a.IsDefaultFacility) ?? userCompanies.FirstOrDefault(a => a.UserId == userId);

            var currentUserCompaies = userCompanies.Where(a => a.UserId == patientUserId && a.IsDefaultFacility);
            foreach (var usercomp in currentUserCompaies)
            {
                //Set all the default false to set the new one to be the default one
                usercomp.IsDefaultFacility = false;
            }

            if (userCompany != null)
            {
                var uc = new UserCompany();
                uc.UserId = patientUserId;
                uc.CompanyId = userCompany.CompanyId;
                uc.FacilityId = userCompany.FacilityId;
                uc.IsDefaultFacility = true;
                _userCompanyRepository.InsertAsync(uc);
            }
            return userCompany?.FacilityId ?? 0;
        }
        public int InsertUserCompany(UserCompany userCompany)
        {
            var userCompanies = _userCompanyRepository.GetAll().Where(a => a.UserId == userCompany.UserId);
            foreach (var usercomp in userCompanies)
            {
                //Set all the default false to set the new one to be the default one
                usercomp.IsDefaultFacility = false;
            }
            _userCompanyRepository.InsertOrUpdateAndGetId(userCompany);
            return userCompany.Id;
        }

        public int UpdateUserCompany(UserCompany userCompany)
        {
            _userCompanyRepository.InsertOrUpdateAndGetId(userCompany);
            var id = userCompany.Id;             
            return id;
        }

        public void DeleteUserCompany(UserCompany userCompany)
        {
            _userCompanyRepository.Delete(userCompany);
        }

        public void DeleteUserUserCompany(int userId)
        {
            var userComapnies = _userCompanyRepository.GetAll();
            var selected = userComapnies.Where(c => c.UserId == userId);
            foreach (var userCompany in selected)
            {
                DeleteUserCompany(userCompany);
            }
        }

        public virtual async Task<List<SelectListItem>> CompanySelectList(int userId, bool excludeDefault = false)
        {
            var proArtsCompanyId = int.Parse(_appConfigurationAccessor.Configuration["DefaultCompany:ProfessioalArtsCompanyId"]);
            var companies = await _companyRepository.GetAllIncludingAsync(a => a.UserCompanies);
            if (excludeDefault)
                companies = companies.Where(a => a.Id != proArtsCompanyId);//Exclude pro arts to show here.
            var result = new List<SelectListItem>();
            result.Add(new SelectListItem() { Text = "Select Company", Value = "" });
            result.AddRange(companies.Select(a => new SelectListItem { Text = a.CompanyName, Value = a.Id.ToString(), Selected = a.UserCompanies.Any(b => b.UserId == userId) }).ToList());
            return result;

        }

        public virtual async Task<List<SelectListItem>> FacilitySelectList(int userId, bool excludeDefault = false)
        {
            var userCompanies = GetUserCompanies(userId);
            var userFacilityIds = userCompanies.Select(a => a.FacilityId);
            var proArtsCompanyId = int.Parse(_appConfigurationAccessor.Configuration["DefaultCompany:ProfessioalArtsCompanyId"]);
            var facilities = await _facilityRepository.GetAllIncludingAsync(a => a.Company);
            if (excludeDefault)
                facilities = facilities.Where(a => a.Company.Id != proArtsCompanyId);//Exclude pro arts to show here.
            var result = new List<SelectListItem>();
            result.Add(new SelectListItem() { Text = "Select Facility", Value = "" });

            result.AddRange(facilities.Select(a => new SelectListItem { Text = $"{a.Company.CompanyName} {a.FacilityName}", Value = a.Id.ToString(), Selected = ((userFacilityIds != null) && userFacilityIds.Contains(a.Id)) }).ToList());
            return result;

        }

        public Facility GetFacilityByCompany(int companyId)
        {
            return _facilityRepository.GetAll().Where(i => i.Company.Id == companyId).FirstOrDefault();
        }
        public Company GetCompanyByFacility(int facilityId)
        {
            return _facilityRepository.GetAllIncluding(a => a.Company).Where(i => i.Id == facilityId).Select(a => a.Company).FirstOrDefault();
        }

        public virtual async Task<List<SelectListItem>> UserFacilitySelectList(int userId, bool excludewatermark = false)
        {
            var userCompanies = GetUserCompanies(userId).DistinctBy(a => a.FacilityId);
            var result = new List<SelectListItem>();
            if (!excludewatermark)
                result.Add(new SelectListItem() { Text = "Select Location", Value = "" });
            result.AddRange(userCompanies.Select(a => new SelectListItem { Text = $"{a.Facility.FacilityName} {a.Facility.Address?.Address1}", Value = a.FacilityId.ToString(), Selected = a.IsDefaultFacility }).ToList());
            return result;
        }
        public void ClearRepoCache(UserCompany userCompany) {
            _userCompanyRepository.DetachFromDbContext(userCompany);
        }
    }
}
