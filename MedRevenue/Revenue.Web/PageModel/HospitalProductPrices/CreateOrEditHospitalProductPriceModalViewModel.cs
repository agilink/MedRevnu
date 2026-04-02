using ATI.Revenue.Application.HospitalProductPrices.Dtos;

namespace ATI.Revenue.Web.PageModel.HospitalProductPrices
{
    public class CreateOrEditHospitalProductPriceModalViewModel
    {
        public CreateOrEditHospitalProductPriceDto HospitalProductPrice { get; set; }
        public bool IsEditMode { get; set; }
    }
}
