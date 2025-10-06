using ATI.Pharmacy.Dtos;

using Abp.Extensions;

namespace ATI.Pharmacy.Web.PageModel.User
{
    public class CreateOrEditUserViewModel
    {
        public CreateOrEditUserDto User { get; set; }

        public CreateOrEditUserInputDto Users { get; set; }

        public bool IsEditMode => User.Id.HasValue;
    }
}