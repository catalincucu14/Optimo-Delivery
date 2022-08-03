namespace Models.Utils
{
    public class Coordinates : Model<Coordinates>
    {
        public string Latitude { get; set; }

        public string Longitude { get; set; }

        public double Distance(Coordinates coordiantes)
        {
            return Math.Sqrt(Math.Pow(Convert.ToDouble(Latitude) - Convert.ToDouble(coordiantes.Latitude), 2) + Math.Pow(Convert.ToDouble(Longitude) - Convert.ToDouble(coordiantes.Longitude), 2));
        }
    }
}
