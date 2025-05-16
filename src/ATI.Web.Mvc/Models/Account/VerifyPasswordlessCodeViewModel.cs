using Abp.Localization;
using System.ComponentModel.DataAnnotations;

namespace ATI.Web.Models.Account
{
    public class VerifyPasswordlessCodeViewModel
    {
        [Required]
        [AbpDisplayName(ATIConsts.LocalizationSourceName, "Code")]
        public string Code { get; set; }
        
        public string ProviderValue { get; set; }
        
        public string ProviderType { get; set; }
        
    }
}
