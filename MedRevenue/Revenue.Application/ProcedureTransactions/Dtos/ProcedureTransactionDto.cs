using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ATI.Revenue.Domain.Entities;
using ATI.Revenue.Domain.Enums;
using System;
using System.Collections.Generic;

namespace ATI.Revenue.Application.ProcedureTransactions.Dtos
{
    [AutoMapFrom(typeof(ProcedureTransaction))]
    public class ProcedureTransactionDto : EntityDto<int>
    {
        public string CaseNumber { get; set; }
        public DateTime ProcedureDate { get; set; }
        public int? HospitalId { get; set; }
        public string HospitalName { get; set; }
        public int PhysicianId { get; set; }
        public string PhysicianName { get; set; }
        public ImplantType ImplantType { get; set; }
        public decimal TotalAmount { get; set; }

        /// <summary>Number of distinct devices on the case, for the grid summary.</summary>
        public int DeviceCount { get; set; }

        /// <summary>Total units across all device lines.</summary>
        public int TotalUnits { get; set; }

        /// <summary>Short description of the devices, e.g. "PM1272 +2 more".</summary>
        public string ProductSummary { get; set; }

        public List<ProcedureTransactionProductDto> Products { get; set; }

        public ProcedureTransactionDto()
        {
            Products = new List<ProcedureTransactionProductDto>();
        }
    }
}
