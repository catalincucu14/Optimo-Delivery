using Models.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Models.Resources
{
    public class Store : Model<Store>
    {
        [Key]
        [Column(TypeName = "int")]
        public int StoreId { get; set; }

        [ForeignKey("Admin")]
        [Column(TypeName = "int")]
        public int AdminId { get; set; }

        [RegularExpression(Regex.Name, ErrorMessage = "CITY invalid")]
        [StringLength(50, ErrorMessage = "CITY cannot exceeds 50 characters")]
        [Required(ErrorMessage = "CITY required")]
        [Column(TypeName = "varchar(50)")]
        public string City { get; set; } = string.Empty;

        [RegularExpression(Regex.Name, ErrorMessage = "ADDRESS invalid")]
        [StringLength(50, ErrorMessage = "ADDRESS cannot exceeds 50 characters")]
        [Required(ErrorMessage = "ADDRESS required")]
        [Column(TypeName = "varchar(50)")]
        public string Address { get; set; } = string.Empty;

        [RegularExpression(Regex.Coordinate, ErrorMessage = "LATITUDE must match the format [ digits.digits ]")]
        [Column(TypeName = "varchar(20)")]
        public string Latitude { get; set; } = string.Empty;

        [RegularExpression(Regex.Coordinate, ErrorMessage = "LONGITUDE must match the format [ digits.digits ]")]
        [Column(TypeName = "varchar(20)")]
        public string Longitude { get; set; } = string.Empty;

        public Store() { }
    }
}
