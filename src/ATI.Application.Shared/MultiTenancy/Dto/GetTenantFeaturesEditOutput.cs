using System.Collections.Generic;
using Abp.Application.Services.Dto;
using ATI.Editions.Dto;

namespace ATI.MultiTenancy.Dto
{
    public class GetTenantFeaturesEditOutput
    {
        public List<NameValueDto> FeatureValues { get; set; }

        public List<FlatFeatureDto> Features { get; set; }
    }
}