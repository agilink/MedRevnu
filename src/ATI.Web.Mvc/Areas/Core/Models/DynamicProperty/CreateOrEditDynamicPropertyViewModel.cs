using System.Collections.Generic;
using ATI.DynamicEntityProperties.Dto;

namespace ATI.Web.Areas.Core.Models.DynamicProperty
{
    public class CreateOrEditDynamicPropertyViewModel
    {
        public DynamicPropertyDto DynamicPropertyDto { get; set; }

        public List<string> AllowedInputTypes { get; set; }
    }
}
