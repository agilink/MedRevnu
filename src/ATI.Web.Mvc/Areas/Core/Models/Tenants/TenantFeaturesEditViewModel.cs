using Abp.AutoMapper;
using ATI.MultiTenancy;
using ATI.MultiTenancy.Dto;
using ATI.Web.Areas.Core.Models.Common;

namespace ATI.Web.Areas.Core.Models.Tenants
{
    [AutoMapFrom(typeof (GetTenantFeaturesEditOutput))]
    public class TenantFeaturesEditViewModel : GetTenantFeaturesEditOutput, IFeatureEditViewModel
    {
        public Tenant Tenant { get; set; }
    }
}