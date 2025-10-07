using Abp.Domain.Entities;
using ATI.Admin.Domain.Entities;
using ATI.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATI.Pharmacy.Domain.Entities
{
    public partial class UserCompanyDoctor:Entity<int>
    {
        public required UserCompany UserCompany { get; set; }
        public required User DoctorUser { get; set; }

    }
}
