using Abp.EntityHistory;
using ATI.Authorization.Users;
using System.Collections.Generic;

namespace ATI.EntityChanges
{
    public class EntityChangePropertyAndUser
    {
        public EntityChange EntityChange { get; set; }
        public List<EntityPropertyChange> PropertyChanges { get; set; }
        public User User { get; set; }
    }
}
