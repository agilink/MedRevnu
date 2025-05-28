using System;
using Abp.Application.Services.Dto;

namespace ATI.Admin.Application.Companies.Dtos;

public class CompanyDto : EntityDto
{
    public int? CompanyTypeId { get; set; }
    public int? CompanyStatusId { get; set; }
    public string CompanyName { get; set; }

    public int BillTo { get; set; }
    public string BillToName
    {
        get
        {
            // Get the name of the enum value
            if (BillTo == 0) { return string.Empty; }
            else
                return Enum.GetName(typeof(BillToEnum), BillTo);
        }
    }

    public int DeliveryTypeId { get; set; }
}