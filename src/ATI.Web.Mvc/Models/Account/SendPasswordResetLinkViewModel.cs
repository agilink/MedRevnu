using System.ComponentModel.DataAnnotations;

namespace ATI.Web.Models.Account
{
    public class SendPasswordResetLinkViewModel
    {
        [Required]
        public string EmailAddress { get; set; }
    }
}