namespace ATI.MedRevnu.Domain
{
    /// <summary>
    /// Constants for Lafayette Quota system
    /// </summary>
    public static class LafayetteQuotaConsts
    {
        // Product Categories
        public const string LowVoltageCategory = "Low Voltage";
        public const string HighVoltageCategory = "High Voltage";
        public const string LeadlessCategory = "Leadless";
        public const string IcmCategory = "ICM";

        // Case Status
        public const string ActiveCaseStatus = "Active";
        public const string CompletedCaseStatus = "Completed";
        public const string CancelledCaseStatus = "Cancelled";

        // Procedure Types
        public const string PpmProcedure = "PPM";
        public const string CrtpProcedure = "CRTP";
        public const string IcdProcedure = "ICD";
        public const string CrtdProcedure = "CRTD";
        public const string IlrProcedure = "ILR";

        // Personnel Types
        public const string DoctorPersonnelType = "Doctor";
        public const string TechnicianPersonnelType = "Technician";
        public const string NursePersonnelType = "Nurse";

        // Validation lengths
        public const int MaxNameLength = 200;
        public const int MaxDescriptionLength = 500;
        public const int MaxNotesLength = 1000;
        public const int MaxCaseNumberLength = 200;
        public const int MaxEmailLength = 200;
        public const int MaxPhoneLength = 50;
        public const int MaxTitleLength = 100;
        public const int MaxSpecialtyLength = 200;
        public const int MaxLicenseNumberLength = 100;
        public const int MaxStatusLength = 100;
        public const int MaxProcedureTypeLength = 200;
        public const int MaxCategoryLength = 200;
        public const int MaxModelNoLength = 200;

        // Default values
        public const int DefaultQuantity = 1;
        public const decimal DefaultRevenue = 0;
        public const bool DefaultIsActive = true;
    }
}