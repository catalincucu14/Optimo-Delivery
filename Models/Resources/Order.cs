using Models.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Models.Resources
{
    public class Order : Model<Order>
    {
        [Key]
        [Column(TypeName = "int")]
        public int OrderId { get; set; }

        [ForeignKey("Admin")]
        [Column(TypeName = "int")]
        public int AdminId { get; set; }

        [ForeignKey("Route")]
        [Column(TypeName = "int")]
        public int? RouteId { get; set; }

        [RegularExpression(Regex.CustomId, ErrorMessage = "CUSTOM ID Invalid")]
        [StringLength(25, ErrorMessage = "CUSTOM ID cannot exceeds 25 characters")]
        [Column(TypeName = "varchar(25)")]
        public string? CustomId { get; set; }

        [RegularExpression(Regex.Name, ErrorMessage = "FIRST NAME invalid")]
        [StringLength(25, ErrorMessage = "FIRST NAME cannot exceeds 25 characters")]
        [Column(TypeName = "varchar(25)")]
        public string? FirstName { get; set; }

        [RegularExpression(Regex.Name, ErrorMessage = "LAST NAME Invalid")]
        [StringLength(25, ErrorMessage = "LAST NAME cannot exceeds 25 characters")]
        [Column(TypeName = "varchar(25)")]
        public string? LastName { get; set; }

        [RegularExpression(Regex.Mail, ErrorMessage = "MAIL invalid")]
        [Column(TypeName = "varchar(50)")]
        public string? Mail { get; set; }

        [RegularExpression(Regex.Phone, ErrorMessage = "PHONE invalid")]
        [Required(ErrorMessage = "PHONE required")]
        [Column(TypeName = "varchar(25)")]
        public string Phone { get; set; } = string.Empty;

        [Column(TypeName = "int")]
        [Required(ErrorMessage = "LEFT TO PAY required")]
        public double LeftToPay { get; set; } = 0;

        [RegularExpression(Regex.Name, ErrorMessage = "CITY invalid")]
        [StringLength(50, ErrorMessage = "CITY cannot exceeds 50 characters")]
        [Required(ErrorMessage = "CITY required")]
        [Column(TypeName = "varchar(50)")]
        public string City { get; set; } = string.Empty;

        [RegularExpression(Regex.Name, ErrorMessage = "Address Invalid")]
        [StringLength(50, ErrorMessage = "ADDRESS cannot exceeds 50 characters")]
        [Required(ErrorMessage = "ADDRESS required")]
        [Column(TypeName = "varchar(50)")]
        public string Address { get; set; } = string.Empty;

        [RegularExpression(Regex.Coordinate, ErrorMessage = "LATITUTDE must match the format [ digits.digits ]")]
        [Column(TypeName = "varchar(20)")]
        public string Latitude { get; set; } = string.Empty;

        [RegularExpression(Regex.Coordinate, ErrorMessage = "LONGITUDE must match the format [ digits.digits ]")]
        [Column(TypeName = "varchar(20)")]
        public string Longitude { get; set; } = string.Empty;

        [RegularExpression(Regex.State, ErrorMessage = "ORDER STATE can only be UNDELIVERED, DELIVERED, POSTPONED or CANCELLED")]
        [Required(ErrorMessage = "ORDER STATE required")]
        [Column(TypeName = "varchar(15)")]
        public string OrderState { get; set; } = "UNDELIVERED";

        // This fileds are unly unsed in UI
        // I could've just made another class that will extend this class but is too late now
        [NotMapped]
        public bool CheckBox { get; set; } = false;

        /// <summary>
        /// Function used to check if any filed match a given filter
        /// </summary>
        public bool Search(string filter)
        {
            Func<string, bool> Contains = (field) => field is not null ? field.ToLower().Contains(filter.ToLower()) : false;

            return Contains(CustomId) || Contains(FirstName) || Contains(LastName) || Contains(Mail) || Contains(Phone) || Contains(City) || Contains(Address) || Contains(OrderState);
        }

        /// <summary>
        /// Function used to get an object with the data of the order
        /// </summary>
        public object[] ToObject()
        {
            return new object[] { OrderId, FirstName, LastName, Mail, Phone, LeftToPay, City, Address, OrderState };
        }
    }
}
