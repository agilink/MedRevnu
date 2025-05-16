using System.Collections.Generic;
using System.Threading.Tasks;
using Abp;
using ATI.Dto;

namespace ATI.Gdpr
{
    public interface IUserCollectedDataProvider
    {
        Task<List<FileDto>> GetFiles(UserIdentifier user);
    }
}
