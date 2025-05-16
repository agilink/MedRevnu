using System.Collections.Generic;
using ATI.Editions.Dto;

namespace ATI.Web.Areas.Core.Models.Tenants
{
    public class TenantIndexViewModel
    {
        public List<SubscribableEditionComboboxItemDto> EditionItems { get; set; }
    }
}