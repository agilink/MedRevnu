using Abp.Application.Services.Dto;
using ATI.Revenue.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ATI.Revenue.Application.ProcedureTypes.Dtos
{
    public class CreateOrEditProcedureTypeDto : EntityDto<int>
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Code { get; set; }

        [Required]
        public CategoryGroup CategoryGroup { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int DisplayOrder { get; set; }
    }
}
