using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.EntityHistory;
using Microsoft.EntityFrameworkCore;
using ATI.Auditing;
using ATI.Authorization.Users;
using ATI.EntityChanges.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace ATI.EntityChanges
{
    public class EntityChangeAppService : ATIAppServiceBase, IEntityChangeAppService
    {
        private readonly IRepository<EntityChange, long> _entityChangeRepository;
        private readonly IRepository<EntityChangeSet, long> _entityChangeSetRepository;
        private readonly IRepository<EntityPropertyChange, long> _entityPropertyChangeRepository;
        private readonly IRepository<User, long> _userRepository;

        public EntityChangeAppService(
            IRepository<EntityChange, long> entityChangeRepository,
            IRepository<EntityChangeSet, long> entityChangeSetRepository,
            IRepository<User, long> userRepository,
            IRepository<EntityPropertyChange, long> entityPropertyChangeRepository)
        {
            _entityChangeRepository = entityChangeRepository;
            _entityChangeSetRepository = entityChangeSetRepository;
            _userRepository = userRepository;
            _entityPropertyChangeRepository = entityPropertyChangeRepository;
        }

        public async Task<ListResultDto<EntityAndPropertyChangeListDto>> GetEntityChangesByEntity(GetEntityChangesByEntityInput input)
        {
            var entityId = "\"" + input.EntityId + "\"";

            var query = from entityChange in _entityChangeRepository.GetAll()
                        join entityChangeSet in _entityChangeSetRepository.GetAll() on entityChange.EntityChangeSetId equals entityChangeSet.Id
                        join user in _userRepository.GetAll() on entityChangeSet.UserId equals user.Id
                        join entityPropertyChange in _entityPropertyChangeRepository.GetAll() on entityChange.Id equals entityPropertyChange.EntityChangeId into propertyChanges
                        where entityChange.EntityTypeFullName == input.EntityTypeFullName &&
                              (entityChange.EntityId == input.EntityId || entityChange.EntityId == entityId)
                        select new EntityChangePropertyAndUser
                        {
                            EntityChange = entityChange,
                            User = user,
                            PropertyChanges = propertyChanges.ToList()
                        };


            var results = await query.OrderByDescending(ec => ec.EntityChange.ChangeTime).ToListAsync();

            var dtoList = ConvertToEntityAndPropertyChangeListDtos(results);

            return new ListResultDto<EntityAndPropertyChangeListDto>(dtoList);
        }


        private List<EntityAndPropertyChangeListDto> ConvertToEntityAndPropertyChangeListDtos(List<EntityChangePropertyAndUser> results)
        {
            return results.Select(entityChange =>
            {
                var entityChangeDto = ObjectMapper.Map<EntityChangeListDto>(entityChange.EntityChange);
                entityChangeDto.UserName = entityChange.User?.UserName;
                var entityPropertyChangeDtos = entityChange.PropertyChanges.Select(epc => ObjectMapper.Map<EntityPropertyChangeDto>(epc)).ToList();

                return new EntityAndPropertyChangeListDto
                {
                    EntityChange = entityChangeDto,
                    EntityPropertyChanges = entityPropertyChangeDtos
                };
            }).ToList();
        }
    }
}
