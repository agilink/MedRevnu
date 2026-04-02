using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ATI.Revenue.Domain.Entities;
using System;
using System.Collections.Generic;

namespace ATI.Revenue.Application.Cases.Dtos
{
    [AutoMapFrom(typeof(Case))]
    public class CaseDto : EntityDto<int>
    {
        public string? CaseNumber { get; set; }
        public string? ClientName { get; set; }
        public string? Description { get; set; }
        public DateTime CaseDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
        public string Notes { get; set; }

        // New fields for procedure tracking
        public int? ProcedureTypeId { get; set; }
        public string ProcedureTypeName { get; set; }
        public int? FacilityId { get; set; }
        public string FacilityName { get; set; }
        public string SurgeonName { get; set; }
        public DateTime? ProcedureDate { get; set; }

        public List<CaseProductDto> CaseProducts { get; set; }
    }
}