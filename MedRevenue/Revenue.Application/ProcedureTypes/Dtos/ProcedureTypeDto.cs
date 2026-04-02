using Abp.Application.Services.Dto;
using ATI.Revenue.Domain.Enums;
using System;

namespace ATI.Revenue.Application.ProcedureTypes.Dtos
{
    public class ProcedureTypeDto : EntityDto<int>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public CategoryGroup CategoryGroup { get; set; }
        public string CategoryGroupName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
