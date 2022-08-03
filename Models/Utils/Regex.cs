namespace Models.Utils
{
    public class Regex
    {
        public const string Mail = @"^(?=^.{5,50}$)(\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+)$";

        public const string Phone = @"^(?=^.{9,25}$)([+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\.0-9]*)$";

        public const string Name = @"^([\d\w ,.'\-()_]*)$";

        public const string Password = @"^(?=.*?[A-Z])(?=.*?[a-z]).{8,40}$";

        public const string CustomId = @"^[\d\w \.,-_]+$";

        public const string Coordinate = @"^[\d+.\d+]{1,20}$";

        public const string State = @"^(UNDELIVERED|DELIVERED|CANCELLED)$";

        public const string Compelted = @"^(0|1)$";
    }
}
