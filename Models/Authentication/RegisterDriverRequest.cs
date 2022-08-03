using Models.Utils;
using System.ComponentModel.DataAnnotations;

namespace Models.Authentication
{
    public class RegisterDriverRequest : Model<RegisterDriverRequest>
    {
        [RegularExpression(Regex.Mail, ErrorMessage = "MAIL invalid")]
        [Required(ErrorMessage = "MAIL required")]
        public string Mail { get; set; } = string.Empty;

        [RegularExpression(Regex.Name, ErrorMessage = "NAME invalid")]
        [StringLength(50, ErrorMessage = "NAME cannot exceeds 50 characters")]
        [Required(ErrorMessage = "NAME required")]
        public string Name { get; set; } = string.Empty;

        [RegularExpression(Regex.Password, ErrorMessage = "PASSWORD must have at least one uppercase and lowercase")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "PASSWORD must have between 8 and 40 characters")]
        public string? Password { get; set; }
    }
}
