using Models.Utils;
using System.ComponentModel.DataAnnotations;

namespace ClientAdmin.Utils
{
    public class ExcelMapping : Model<ExcelMapping>
    {
        public int Rows { get; set; } = 0;

        public int Columns { get; set; } = 0;

        public int CustomId { get; set; } = 0;

        public int FirstName { get; set; } = 0;

        public int LastName { get; set; } = 0;

        public int Mail { get; set; } = 0;

        [Range(1, int.MaxValue)]
        public int Phone { get; set; } = 0;

        [Range(1, int.MaxValue)]
        public int LeftToPay { get; set; } = 0;

        [Range(1, int.MaxValue)]
        public int City { get; set; } = 0;

        [Range(1, int.MaxValue)]
        public int Address { get; set; } = 0;

        public void ConfigureField(string value, int index)
        {
            switch (value)
            {
                case "id":
                case "order id":
                    CustomId = index;
                    break;

                case "first name":
                    FirstName = index;
                    break;

                case "last name":
                    LastName = index;
                    break;

                case "mail":
                case "email":
                    Mail = index;
                    break;

                case "phone":
                case "phone number":
                    Phone = index;
                    break;

                case "left to pay":
                case "price":
                    LeftToPay = index;
                    break;

                case "city":
                    City = index;
                    break;

                case "address":
                    Address = index;
                    break;
            }
        }
    }
}
