using Abp;
using System.Threading.Tasks;

namespace ATI.Authorization.Users.DataCleaners
{
    public interface IUserDataCleaner
    {
        Task CleanUserData(UserIdentifier userIdentifier);
    }
}
