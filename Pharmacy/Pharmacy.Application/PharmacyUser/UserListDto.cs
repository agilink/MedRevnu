using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;
using ATI.Authorization.Users.Dto;

namespace ATI.Pharmacy.Dtos
{
    public class EmployeeListDto : EntityDto<long>, IPassivable, IHasCreationTime
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }

        public string PhoneNumber { get; set; }

        public Guid? ProfilePictureId { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public List<UserListRoleDto> Roles { get; set; }
        public string Company { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreationTime { get; set; }
    }
}