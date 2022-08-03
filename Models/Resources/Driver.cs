using Models.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Models.Resources
{
    public class Driver : Model<Driver>
    {
        [Key]
        [Column(TypeName = "int")]
        public int DriverId { get; set; }

        [ForeignKey("Admin")]
        [Column(TypeName = "int")]
        public int AdminId { get; set; }

        [RegularExpression(Regex.Mail, ErrorMessage = "MAIL invalid")]
        [Required(ErrorMessage = "MAIL required")]
        [Column(TypeName = "varchar(50)")]
        public string Mail { get; set; } = string.Empty;

        [RegularExpression(Regex.Name, ErrorMessage = "NAME invalid")]
        [StringLength(50, ErrorMessage = "NAME cannot exceeds 50 characters")]
        [Required(ErrorMessage = "NAME Required")]
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "varchar(500)")]
        public string Password { get; set; }

        public List<Route> Routes { get; set; } = new();
    }
}
