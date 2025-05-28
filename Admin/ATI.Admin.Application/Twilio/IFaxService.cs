using Abp.Application.Services;
using ATI.Admin.Domain.Entities;
using ATI.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATI.Admin.Application
{
    public interface IFaxService : IApplicationService
    {
        Task SendAsync(byte[] data);

    }
}
