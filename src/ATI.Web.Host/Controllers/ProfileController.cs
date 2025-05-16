using Abp.AspNetCore.Mvc.Authorization;
using ATI.Authorization.Users.Profile;
using ATI.Graphics;
using ATI.Storage;

namespace ATI.Web.Controllers
{
    [AbpMvcAuthorize]
    public class ProfileController : ProfileControllerBase
    {
        public ProfileController(
            ITempFileCacheManager tempFileCacheManager,
            IProfileAppService profileAppService,
            IImageValidator imageValidator) :
            base(tempFileCacheManager, profileAppService, imageValidator)
        {
        }
    }
}