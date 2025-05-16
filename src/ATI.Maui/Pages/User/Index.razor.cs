using ATI.Authorization.Users;
using ATI.Authorization.Users.Dto;
using ATI.Maui.Models.User;
using ATI.Maui.Services.User;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using ATI.Maui.Core.Components;
using ATI.Maui.Core.Threading;
using ATI.Maui.Services.UI;

namespace ATI.Maui.Pages.User
{
    public partial class Index : ATIMainLayoutPageComponentBase
    {
        protected IUserAppService UserAppService { get; set; }
        protected IUserProfileService UserProfileService { get; set; }

        private CreateOrEditUserModal createOrEditUserModal { get; set; }

        private ItemsProviderResult<UserListModel> users;

        private readonly GetUsersInput _filter = new GetUsersInput();

        private Virtualize<UserListModel> UserListContainer { get; set; }

        public Index()
        {
            UserAppService = Resolve<IUserAppService>();
            UserProfileService = Resolve<IUserProfileService>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPageHeader(L("Users"), L("UsersHeaderInfo"),[new PageHeaderButton(L("Create"), OpenCreateModal)]);
        }

        private async Task RefreshList()
        {
            await UserListContainer.RefreshDataAsync();
            StateHasChanged();
        }

        private async ValueTask<ItemsProviderResult<UserListModel>> LoadUsers(ItemsProviderRequest request)
        {
            _filter.MaxResultCount = request.Count;
            _filter.SkipCount = request.StartIndex;

            await UserDialogsService.Block();

            await WebRequestExecuter.Execute(
                async () => await UserAppService.GetUsers(_filter),
                async (result) =>
                {
                    if (result == null)
                    {
                        await UserDialogsService.UnBlock();
                        return;
                    }

                    var userList = ObjectMapper.Map<List<UserListModel>>(result.Items);
                    foreach (var user in userList)
                    {
                        await SetUserImageSourceAsync(user);
                    }

                    users = new ItemsProviderResult<UserListModel>(userList, result.TotalCount);

                    await UserDialogsService.UnBlock();
                }
             );

            return users;
        }

        private async Task SetUserImageSourceAsync(UserListModel userListModel)
        {
            if (userListModel.Photo != null)
            {
                return;
            }

            if (!userListModel.ProfilePictureId.HasValue)
            {
                userListModel.Photo = UserProfileService.GetDefaultProfilePicture();
                return;
            }

            userListModel.Photo = await UserProfileService.GetProfilePicture(userListModel.Id);
        }

        public async Task EditUser(UserListModel user)
        {
            await createOrEditUserModal.OpenFor(user);
        }

        public async Task OpenCreateModal()
        {
            await createOrEditUserModal.OpenFor(null);
        }
    }
}
