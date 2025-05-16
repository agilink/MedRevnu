using ATI.Dto;
using System;

namespace ATI.EntityChanges.Dto
{
    public class GetEntityChangesByEntityInput
    {
        public string EntityTypeFullName { get; set; }
        public string EntityId { get; set; }
    }
}
