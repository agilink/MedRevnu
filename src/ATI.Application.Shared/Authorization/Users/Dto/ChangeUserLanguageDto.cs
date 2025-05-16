using System.ComponentModel.DataAnnotations;

namespace ATI.Authorization.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}
