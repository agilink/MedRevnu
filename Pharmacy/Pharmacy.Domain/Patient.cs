using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pharmacy.Domain
{
    [Table("Patient")]
    public class Patient:AuditedEntity
    {
        public int UserId { get; set; }
        public string PaitentName { get; set; } = string.Empty;
        public string PatientDescription { get; set; } = string.Empty;
    }
}
