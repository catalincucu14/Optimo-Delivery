using Models.Utils;

namespace ClientAdmin.Services.RoutePlanner
{
    public interface IRoutePlannerService
    {
        /// <summary>
        /// Function used to select a group of nodes of a specified size, the group should contain nodes close together
        /// Algorithm used: K-Means
        /// Why: Is the only solution the group of nodes without a reference point
        /// Logic: The optimal number of clusters can't determied for every case
        /// So we'll just cluster from a high number of clusters till we get the desired result (a cluster with the specified size)
        /// If the selected cluster has more nodes than the specified size we'll just remove the excess
        /// As we go to fewer clusters the clusters will get bigger and will lose precision
        /// </summary>
        public List<int> Plan(List<Coordinates> nodes, int quantity, int precision);
    }
}
