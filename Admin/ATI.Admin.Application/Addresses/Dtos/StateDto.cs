using Abp.Application.Services.Dto;

namespace ATI.Admin.Application.Addresses.Dtos;

public partial class StateDto : EntityDto<int>
{
    public string Description { get; set; }
    public string Abbreviation { get; set; }
}

