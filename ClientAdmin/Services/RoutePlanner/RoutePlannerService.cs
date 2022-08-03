using Models.Utils;

namespace ClientAdmin.Services.RoutePlanner
{
    public class RoutePlannerService : IRoutePlannerService
    {
        private static Random _random = new Random();

        private static int _interations = 10;

        /// <inheritdoc />
        public List<int> Plan(List<Coordinates> nodes, int quantity, int precision)
        {
            // Check if the number of requested nodes is less than the number of available nodes 
            if (quantity >= nodes.Count)
            {
                throw new Exception("The number of desired selected orders should be less than the number of available orders");
            }

            // This will be store the index of requested nodes
            List<int> selectedOrders = new();

            // This will be store the index of available nodes
            List<int> availableOrders = Enumerable
                .Range(0, nodes.Count)
                .ToList();

            // Repeat till a cluster with at least 'quantity' nodes is created
            while (precision >= 2)
            {
                // This will store the number of clusters used to group the nodes
                int noOfClusters = precision;

                // This will store the id of the centroid assigned to a node, at first, the nodes will be assigned to a random centroid
                List<int> clusterIds = Enumerable
                    .Range(0, nodes.Count)
                    .Select(_ => _random.Next(noOfClusters))
                    .ToList();

                // This will store the coordinates of the centroids, at first, the centroids will be chosen from the nodes
                List<Coordinates> centroids = Enumerable
                    .Range(0, noOfClusters)
                    .Select(_ =>
                    {
                        Coordinates node = nodes[_random.Next(nodes.Count)];
                        return new Coordinates()
                        {
                            Latitude = node.Latitude,
                            Longitude = node.Longitude
                        };
                    })
                    .ToList();

                // Loop for a number of iterations arbitrarily chosen
                foreach (int _ in Enumerable.Range(0, _interations))
                {
                    // This will store the nodes of a cluster (group of points near a centroid)
                    List<List<int>> clusters = Enumerable
                        .Range(0, noOfClusters)
                        .Select(_ => new List<int>())
                        .ToList();

                    // Create the clusters, empty lists at first
                    Enumerable
                        .Range(0, clusterIds.Count)
                        .ToList()
                        .ForEach(node => clusters[clusterIds[node]].Add(node));

                    for (int cluster = 0; cluster < noOfClusters; ++cluster)
                    {
                        // Skip if the cluster is empty
                        if (clusters[cluster].Count == 0)
                        {
                            continue;
                        }

                        // Calculate the latitute of the centroid based on the coordiantes of the nodes
                        double latitude = clusters[cluster]
                            .Select(i => Convert.ToDouble(nodes[i].Latitude))
                            .Sum();
                        centroids[cluster].Latitude = (latitude / clusters[cluster].Count).ToString();

                        // Calculate the longitude of the centroid based on the coordiantes of the nodes
                        double longitude = clusters[cluster]
                           .Select(i => Convert.ToDouble(nodes[i].Longitude))
                           .Sum();
                        centroids[cluster].Longitude = (longitude / clusters[cluster].Count).ToString();
                    }

                    // Update the centroid id for every node, it will be the closest one
                    for (int i = 0; i < nodes.Count; ++i)
                    {
                        int centroidId = -1;
                        double distanceFromCentroidToNode = double.MaxValue;

                        for (int cluster = 0; cluster < noOfClusters; ++cluster)
                        {
                            double distanceFromCentroidToNodeTemp = nodes[i].Distance(centroids[cluster]);

                            if (distanceFromCentroidToNode > distanceFromCentroidToNodeTemp)
                            {
                                distanceFromCentroidToNode = distanceFromCentroidToNodeTemp;
                                centroidId = cluster;
                            }
                        }

                        clusterIds[i] = centroidId;
                    }

                    // Check if a cluster with at least 'quantity' nodes is created, if not First() will throw an exception
                    try
                    {
                        List<int> cluster = clusters
                            .Where(cluster => cluster.Count >= quantity)
                            .OrderBy(cluster => cluster.Count)
                            .First();

                        selectedOrders = cluster;
                    }
                    catch { }
                }

                // If a cluster with at least 'numberOfOrders' nodes is created the stop the loop
                if (selectedOrders.Count >= quantity)
                {
                    break;
                }

                // Decrease the number of centroids
                precision--;
            }

            return Enumerable
                .Range(0, quantity)
                .Select(i => selectedOrders[i])
                .ToList();
        }
    }
}
