using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ATI.Revenue.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ATI.Revenue.Application.Cases.Dtos
{
    [AutoMapTo(typeof(Case))]
    public class CreateOrEditCaseDto : EntityDto<int>, IEntityDto<int>
    {
        [Required]
        [StringLength(50)]
        public string CaseNumber { get; set; }

        [Required]
        [StringLength(200)]
        public string ClientName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public DateTime CaseDate { get; set; }

        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }

        public List<CaseProductDto> CaseProducts { get; set; }
    }
}