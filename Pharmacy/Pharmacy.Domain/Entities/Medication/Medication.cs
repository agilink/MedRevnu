namespace ATI.Pharmacy.Domain.Entities
{
    using Abp.Domain.Entities;
    using Abp.Domain.Entities.Auditing;
    using ATI.Admin.Domain.Entities;
    using System;
    using System.Collections.Generic;

    public partial class Medication : FullAuditedEntity<int>, IMayHaveTenant
    {
        public Medication()
        {
            Contraindications = new HashSet<Contraindication>();
            Inventories = new HashSet<Inventory>();
            PrescriptionItems = new HashSet<PrescriptionItem>();
        }
        public int? TenantId { get; set; }
        public string MedicationName { get; set; }
        public string Manufacturer { get; set; }
        public string BrandName { get; set; }
        public string GenericName { get; set; }
        public string Description { get; set; }
        public decimal? Concentration { get; set; }
        public virtual Uom ConcentrationUom { get; set; }
        //public int? ConcentrationUomId { get; set; }
        public decimal? Volume { get; set; }
        public virtual Uom VolumeUom { get; set; }        
        //public int? VolumeUomId { get; set; }
        public decimal? Dosage { get; set; }
        public int? DosageFormId { get; set; }
        public decimal? DosageUnits { get; set; }
        public int? DosageRouteId { get; set; }
        public int? DosageFequencyId { get; set; }
        public int? DosageStrengthId { get; set; }
        public int? DispenseContainerId { get; set; }
        public int? DispenseQty { get; set; }
        public int? DispenseDoses { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public decimal? Cost { get; set; }
        public string Instructions { get; set; }
        public int? FormulationTypeId { get; set; }
        public string ExternalID { get; set; }

        public virtual DosageForm DosageForm { get; set; }
        public virtual DosageFrequency DosageFrequency { get; set; }
        public virtual DosageRoute DosageRoute { get; set; }
        public virtual FormulationType FormulationType { get; set; }
        
        public DosageStrength Strength { get; set; }

        public int MedicationCategoryId { get; set; }

        public virtual ICollection<Contraindication> Contraindications { get; set; }
        public virtual ICollection<Inventory> Inventories { get; set; }
        public virtual ICollection<PrescriptionItem> PrescriptionItems { get; set; }

        

    }
}
