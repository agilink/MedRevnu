namespace ATI.Revenue.Domain.Enums
{
    /// <summary>
    /// Where a case has reached in the billing cycle.
    /// </summary>
    /// <remarks>
    /// Recording the status does not yet change any figure: every report and quota
    /// comparison still counts all cases regardless of status. The gap analysis argued
    /// that only Completed or Billed cases should count toward quota actuals, which is a
    /// business decision nobody has confirmed - so the field is captured now and the
    /// reporting rule stays as it was until someone decides.
    /// </remarks>
    public enum CaseStatus
    {
        Open = 1,
        Scheduled = 2,
        Completed = 3,
        Billed = 4,
        Paid = 5,
        Closed = 6
    }
}
