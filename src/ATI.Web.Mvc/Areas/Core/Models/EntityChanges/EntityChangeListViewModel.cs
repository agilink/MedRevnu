using Abp.AutoMapper;
using ATI.EntityChanges.Dto;
using System.Collections.Generic;

namespace ATI.Web.Areas.Core.Models.EntityChanges
{
    [AutoMapFrom(typeof(EntityAndPropertyChangeListDto))]
    public class EntityChangeListViewModel
    {
        public List<EntityAndPropertyChangeListDto> EntityAndPropertyChanges { get; set; }
    }
}
