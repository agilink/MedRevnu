namespace ATI.Revenue.Application.Physicians.Dtos
{
    /// <summary>
    /// What was created, so the screen can tell the user their sign-in details.
    /// </summary>
    public class CreatePhysicianUserOutput
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }

        /// <summary>The default password the account was created with.</summary>
        public string Password { get; set; }

        /// <summary>
        /// True when the physician had no email address and a placeholder was generated,
        /// which the screen warns about because password reset will not reach it.
        /// </summary>
        public bool UsedFallbackEmail { get; set; }
    }
}
