using System.Threading.Tasks;
using ATI.Sessions.Dto;

namespace ATI.Web.Session
{
    public interface IPerRequestSessionCache
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformationsAsync();
    }
}
