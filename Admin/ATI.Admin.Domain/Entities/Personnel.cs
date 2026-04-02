using Abp.Domain.Entities.Auditing;
using ATI.Admin.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATI.Admin.Domain.Entities
{
    /// <summary>
    /// Personnel/Employee entity for managing staff information
    /// </summary>
    public partial class Personnel : FullAuditedEntity
    {
        /// <summary>
        /// Personnel type classification (FullTime, PartTime, Contractor, etc.)
        /// </summary>
        public PersonnelType? PersonnelTypeID { get; set; }

        /// <summary>
        /// Employee status (Active, OnLeave, Terminated, etc.)
        /// </summary>
        public EmployeeStatus? EmployeeStatusID { get; set; }

        /// <summary>
        /// Associated facility ID
        /// </summary>
        public int? FacilityId { get; set; }

        /// <summary>
        /// Navigation property to Facility
        /// </summary>
        [ForeignKey("FacilityId")]
        public virtual Facility Facility { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        [MaxLength(100)]
        public string FIRST_NAME { get; set; }

        /// <summary>
        /// Properly formatted first name
        /// </summary>
        [MaxLength(100)]
        public string FIRST_NAME_PROPER { get; set; }

        /// <summary>
        /// Middle name
        /// </summary>
        [MaxLength(100)]
        public string MIDDLE_NAME { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        [MaxLength(100)]
        public string LAST_NAME { get; set; }

        /// <summary>
        /// Social Security Number (encrypted/secured)
        /// </summary>
        [MaxLength(11)]
        public string SSN { get; set; }

        /// <summary>
        /// Employee identifier/badge number
        /// </summary>
        [MaxLength(50)]
        public string EMPLOYEE_ID { get; set; }

        /// <summary>
        /// Date of birth
        /// </summary>
        public DateTime? DATE_BIRTH { get; set; }

        /// <summary>
        /// Date hired/employment start date
        /// </summary>
        public DateTime? DATE_HIRE { get; set; }

        /// <summary>
        /// Company name
        /// </summary>
        [MaxLength(200)]
        public string COMPANY_NAME { get; set; }

        /// <summary>
        /// Address line 1
        /// </summary>
        [MaxLength(200)]
        public string ADDRESS1 { get; set; }

        /// <summary>
        /// Address line 2
        /// </summary>
        [MaxLength(200)]
        public string ADDRESS2 { get; set; }

        /// <summary>
        /// City
        /// </summary>
        [MaxLength(100)]
        public string CITY { get; set; }

        /// <summary>
        /// State ID reference
        /// </summary>
        public int? StateID { get; set; }

        /// <summary>
        /// Navigation property to State
        /// </summary>
        [ForeignKey("StateID")]
        public virtual State State { get; set; }

        /// <summary>
        /// ZIP/Postal code
        /// </summary>
        [MaxLength(10)]
        public string ZIP_CODE { get; set; }

        /// <summary>
        /// Home phone number
        /// </summary>
        [MaxLength(20)]
        public string NUMBER_HOME { get; set; }

        /// <summary>
        /// Mobile phone number
        /// </summary>
        [MaxLength(20)]
        public string NUMBER_MOBILE { get; set; }

        /// <summary>
        /// Work email address
        /// </summary>
        [MaxLength(200)]
        public string EMAIL_WORK { get; set; }

        /// <summary>
        /// Personal/alternate email address
        /// </summary>
        [MaxLength(200)]
        public string EMAIL_OTHER { get; set; }

        /// <summary>
        /// General notes about the personnel
        /// </summary>
        [MaxLength(2000)]
        public string NOTES { get; set; }

        /// <summary>
        /// Employment/assignment start date
        /// </summary>
        public DateTime? START_DATE { get; set; }

        /// <summary>
        /// Employment/assignment end date
        /// </summary>
        public DateTime? END_DATE { get; set; }

        /// <summary>
        /// User who last modified this record
        /// </summary>
        [MaxLength(100)]
        public string MODIFIED_BY { get; set; }

        /// <summary>
        /// Date of last modification
        /// </summary>
        public DateTime? MODIFIED_DATE { get; set; }
    }
}
