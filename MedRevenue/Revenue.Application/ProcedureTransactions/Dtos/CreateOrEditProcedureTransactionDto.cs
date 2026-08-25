using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ATI.Revenue.Domain.Entities;
using ATI.Revenue.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ATI.Revenue.Application.ProcedureTransactions.Dtos
{
    [AutoMapTo(typeof(ProcedureTransaction))]
    [AutoMapFrom(typeof(ProcedureTransaction))]
    public class CreateOrEditProcedureTransactionDto : EntityDto<int>
    {
        [Required]
        [StringLength(50)]
        public string CaseNumber { get; set; }

        [Required]
        public DateTime ProcedureDate { get; set; }

        // HospitalId is auto-populated from Physician's FacilityId - not required in input
        public int? HospitalId { get; set; }

        [Required]
        public int PhysicianId { get; set; }

        [Required]
        public ImplantType ImplantType { get; set; } = ImplantType.DeNovo;

        /// <summary>
        /// Left at zero to be calculated from the device lines; set to override it with a
        /// negotiated case price.
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// The devices used. A procedure can involve two or three, and every one must
        /// belong to a subcategory matching this case's implant type.
        /// </summary>
        public List<CreateOrEditProcedureTransactionProductDto> Products { get; set; }

        public CreateOrEditProcedureTransactionDto()
        {
            Products = new List<CreateOrEditProcedureTransactionProductDto>();
        }
    }
}
