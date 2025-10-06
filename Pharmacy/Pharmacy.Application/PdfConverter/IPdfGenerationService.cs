using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.Pharmacy.Dtos;
using ATI.Dto;

using System.Collections.Generic;

namespace ATI.Pharmacy
{

    public interface IPdfGenerationService : IApplicationService
    {
        Task<byte[]> GeneratePdfFromPartialViewAsync(string viewPath,string viewName, object model);
    }
}