using System.ComponentModel.DataAnnotations;
using ATI.Validation;

namespace ATI.Maui.Models.Login
{
    public class EmailActivationModel
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}
