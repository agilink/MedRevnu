using Abp.Application.Services;
using Abp.Application.Services.Dto;
using ATI.EntityChanges.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ATI.EntityChanges
{
    public interface IEntityChangeAppService : IApplicationService
    {
        Task<ListResultDto<EntityAndPropertyChangeListDto>> GetEntityChangesByEntity(GetEntityChangesByEntityInput input);
    }
}
