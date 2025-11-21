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
        public List<CaseProductDto> CaseProducts { get; set; }
    }
}