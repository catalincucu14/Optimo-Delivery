using Models.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace Models.Resources
{
    public class Route : Model<Route>
    {
        [Key]
        [Column(TypeName = "int")]
        public int RouteId { get; set; }

        [ForeignKey("Admin")]
        [Column(TypeName = "int")]
        public int AdminId { get; set; }

        [ForeignKey("Driver")]
        [Column(TypeName = "int")]
        public int DriverId { get; set; }

        [RegularExpression(Regex.Name, ErrorMessage = "NAME invalid")]
        [StringLength(25, ErrorMessage = "NAME cannot exceeds 25 characters")]
        [Required(ErrorMessage = "NAME required")]
        [Column(TypeName = "varchar(25)")]
        public string Name { get; set; } = string.Empty;

        [RegularExpression(Regex.Compelted, ErrorMessage = "FIELD invalid")]
        [Required(ErrorMessage = "FIELD required")]
        [Column(TypeName = "int")]
        public int Completed { get; set; } = 0;

        public List<Order> Orders { get; set; } = new();

        // These fileds are unly unsed in UI
        // I could've just made another class that will extend this class but is too late now
        [NotMapped]
        public bool HideOrders { get; set; }

        /// <summary>
        /// Function used to check if any filed match a given filter
        /// </summary>
        public bool Search(string filter)
        {
            Func<string, bool> Contains = (field) => field is not null ? field.ToLower().Contains(filter.ToLower()) : false;

            return Contains(Name);
        }
    }
}
