namespace ATI.Admin.Domain.Enums
{
    /// <summary>
    /// Employee status classification
    /// </summary>
    public enum EmployeeStatus
    {
        /// <summary>
        /// Currently employed and active
        /// </summary>
        Active = 1,

        /// <summary>
        /// On temporary leave
        /// </summary>
        OnLeave = 2,

        /// <summary>
        /// Employment terminated
        /// </summary>
        Terminated = 3,

        /// <summary>
        /// Retired from employment
        /// </summary>
        Retired = 4,

        /// <summary>
        /// Suspended temporarily
        /// </summary>
        Suspended = 5,

        /// <summary>
        /// Inactive/Not currently working
        /// </summary>
        Inactive = 6
    }
}
