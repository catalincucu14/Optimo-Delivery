using Models.Utils;
using System.ComponentModel.DataAnnotations;

namespace Models.Authentication
{
    public class LoginRequest : Model<LoginRequest>
    {
        [RegularExpression(Regex.Mail, ErrorMessage = "MAIL invalid")]
        [Required(ErrorMessage = "Mail required")]
        public string Mail { get; set; } = string.Empty;

        [RegularExpression(Regex.Password, ErrorMessage = "PASSWORD invalid")]
        [Required(ErrorMessage = "PASSWORD required")]
        public string Password { get; set; } = string.Empty;
    }
}
