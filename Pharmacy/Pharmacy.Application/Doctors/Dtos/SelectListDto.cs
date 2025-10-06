using ATI.Pharmacy.Dtos;

namespace ATI.Pharmacy.Dtos;

public class SelectListDto : Abp.Application.Services.Dto.EntityDto
{
    public string Value { get; set; }
    public string Label { get; set; }
}