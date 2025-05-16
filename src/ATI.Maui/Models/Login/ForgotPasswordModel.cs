using System.ComponentModel.DataAnnotations;

namespace ATI.Maui.Models.Login
{
    public class ForgotPasswordModel
    {
        [EmailAddress]
        [Required]
        public string EmailAddress { get; set; }
    }
}
