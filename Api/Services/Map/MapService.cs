using Models.Utils;

#nullable disable

namespace Api.Services.Map
{
    public class MapService : IMapService
    {
        private readonly Random _random = new Random();

        /// <inheritdoc />
        public Coordinates GetCoordinates(string country, string city, string address)
        {
            double latitude = _random.NextDouble() * (48 - 44) + 44;
            double longitude = _random.NextDouble() * (29 - 21) + 21;

            return new Coordinates
            {
                Latitude = latitude.ToString(),
                Longitude = longitude.ToString()
            };
        }

        /// <inheritdoc />
        public List<List<double>> GetAdjacencyMatrix(List<Coordinates> locations)
        {
            List<List<double>> adjacencyMatrix = new();

            for (int i = 0; i < locations.Count - 1; i++)
            {
                adjacencyMatrix[i][i] = 0;

                for (int j = i + 1; j < locations.Count; j++)
                {
                    adjacencyMatrix[i][j] = adjacencyMatrix[j][i] = Math.Sqrt(Math.Pow(Convert.ToDouble(locations[i].Latitude) - Convert.ToDouble(locations[i].Latitude), 2) + Math.Pow(Convert.ToDouble(locations[j].Longitude) - Convert.ToDouble(locations[j].Longitude), 2));
                }
            }

            return adjacencyMatrix;
        }
    }
}
