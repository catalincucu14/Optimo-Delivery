using Models.Utils;
using System.ComponentModel.DataAnnotations;

namespace Models.Authentication
{
    public class RegisterAdminRequest : Model<RegisterAdminRequest>
    {
        [RegularExpression(Regex.Mail, ErrorMessage = "MAIL invalid")]
        [Required(ErrorMessage = "MAIL required")]
        public string Mail { get; set; } = string.Empty;

        [RegularExpression(Regex.Phone, ErrorMessage = "PHONE invalid")]
        [Required(ErrorMessage = "PHONE required")]
        public string Phone { get; set; } = string.Empty;

        [RegularExpression(Regex.Name, ErrorMessage = "COUNTRY invalid")]
        [StringLength(50, ErrorMessage = "COUNTRY cannot exceeds 50 characters")]
        [Required(ErrorMessage = "COUNTRY required")]
        public string Country { get; set; } = "Romania";

        [RegularExpression(Regex.Password, ErrorMessage = "PASSWORD must have at least one uppercase and lowercase")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "PASSWORD must have between 8 and 40 characters")]
        [Required(ErrorMessage = "PASSWORD required")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "PASSWORD PASSWORD required")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
