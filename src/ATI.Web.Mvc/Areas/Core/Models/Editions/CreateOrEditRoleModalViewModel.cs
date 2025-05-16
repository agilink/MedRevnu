using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using ATI.Editions.Dto;
using ATI.Web.Areas.Core.Models.Common;

namespace ATI.Web.Areas.Core.Models.Editions
{
    [AutoMapFrom(typeof(GetEditionEditOutput))]
    public class CreateEditionModalViewModel : GetEditionEditOutput, IFeatureEditViewModel
    {
        public IReadOnlyList<ComboboxItemDto> EditionItems { get; set; }

        public IReadOnlyList<ComboboxItemDto> FreeEditionItems { get; set; }
    }
}