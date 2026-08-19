namespace ATI.Revenue.Domain.Enums
{
    /// <summary>
    /// Distinguishes a new implant from a generator/battery replacement.
    /// The business tracks case counts, revenue and quota targets separately for
    /// each, and the two carry different pricing.
    /// </summary>
    public enum ImplantType
    {
        DeNovo = 1,
        GenChange = 2
    }
}
