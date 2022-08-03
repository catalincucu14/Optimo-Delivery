using Models.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Models.Resources
{
    public class Admin : Model<Admin>
    {
        [Key]
        [Column(TypeName = "int")]
        public int AdminId { get; set; }

        [RegularExpression(Regex.Mail, ErrorMessage = "MAIL Invalid")]
        [Required(ErrorMessage = "MAIL Required")]
        [Column(TypeName = "varchar(50)")]
        public string Mail { get; set; } = string.Empty;

        [RegularExpression(Regex.Phone, ErrorMessage = "PHONE Invalid")]
        [Required(ErrorMessage = "PHONE Required")]
        [Column(TypeName = "varchar(25)")]
        public string Phone { get; set; } = string.Empty;

        [RegularExpression(Regex.Name, ErrorMessage = "COUNTRY Invalid")]
        [Required(ErrorMessage = "COUNTRY Required")]
        [Column(TypeName = "varchar(50)")]
        public string Country { get; set; } = string.Empty;

        [Column(TypeName = "varchar(500)")]
        public string Password { get; set; }

        public List<Driver> Drivers = new();

        public List<Route> Routes = new();

        public List<Order> Orders = new();

        public Store Store = new();
    }
}
