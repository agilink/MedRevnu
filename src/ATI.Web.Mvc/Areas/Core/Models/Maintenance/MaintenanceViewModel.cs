using System.Collections.Generic;
using ATI.Caching.Dto;

namespace ATI.Web.Areas.Core.Models.Maintenance
{
    public class MaintenanceViewModel
    {
        public IReadOnlyList<CacheDto> Caches { get; set; }
        
        public bool CanClearAllCaches { get; set; }
    }
}