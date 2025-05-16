using System.Collections.Generic;
using Abp.Application.Services.Dto;
using ATI.Editions.Dto;

namespace ATI.Web.Areas.Core.Models.Common
{
    public interface IFeatureEditViewModel
    {
        List<NameValueDto> FeatureValues { get; set; }

        List<FlatFeatureDto> Features { get; set; }
    }
}