using Models;
using Models.Resources;

namespace Api.Services.Database.Orders
{
    public interface IOrderService
    {
        /// <summary>
        /// Function used to check if an order exists based on the id
        /// </summary>
        public Task<bool> ExistsAsync(int orderId, int adminId);

        /// <summary>
        /// Function used to read the orders from the database
        /// </summary>
        public Task<Response<List<Order>>> ReadAllAsync(int adminId, string State);

        /// <summary>
        /// Function used to create an order
        /// </summary>
        public Task<Response<Order>> CreateAsync(int adminId, Order order);

        /// <summary>
        /// Function used to update an order
        /// </summary>
        public Task<Response<Order>> UpdateAsync(int orderId, int adminId, Order order);

        /// <summary>
        /// Function used to delete an order
        /// </summary>
        public Task<Response<Order>> DeleteAsync(int orderId, int adminId);
    }
}
