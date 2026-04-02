using ATI.Revenue.Application.ProductQuotas.Dtos;

namespace ATI.Revenue.Web.PageModel.ProductQuotas
{
    public class CreateOrEditProductQuotaModalViewModel
    {
        public CreateOrEditProductQuotaDto ProductQuota { get; set; }
        public bool IsEditMode { get; set; }
    }
}
