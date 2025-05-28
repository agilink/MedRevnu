using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATI.Admin.Domain.Entities
{
    public partial class State: Entity<int>
    {
        public string Description { get; set; }
        public string Abbreviation { get; set; }

        public bool isActive { get; set; } = true;

    }
}
