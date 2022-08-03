using Models.Utils;

namespace Api.Services.Map
{
    public interface IMapService
    {
        /// <summary>
        /// Function used to get the random coodinates of a location 
        /// Now is generating random coodinates but it should use a map geocoding API
        /// </summary>
        public Coordinates GetCoordinates(string country, string city, string address);

        /// <summary>
        /// Function used to get the adjacency matrix of distances for a given set of locations, used to optimize the route (the order of delivering the orders)
        /// Now is calculating the distance between the points but it should use a map directions API
        /// </summary>
        public List<List<double>> GetAdjacencyMatrix(List<Coordinates> locations);
    }
}
