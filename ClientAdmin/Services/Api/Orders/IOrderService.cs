using Models;
using Models.Resources;

namespace ClientAdmin.Services.Api.Orders
{
    public interface IOrderService
    {
        /// <summary>
        /// Function used to get orders using the API
        /// </summary>
        public Task<Response<List<Order>>> GetAsync(string? state);

        /// <summary>
        /// Function used to create an order using the API
        /// </summary>
        public Task<Response<Order>> CreateAsync(Order order);

        /// <summary>
        /// Function used to update an order using the API
        /// </summary>>
        public Task<Response<Order>> UpdateAsync(Order order);

        /// <summary>
        /// Function used to delete an order using the API
        /// </summary>
        public Task<Response<Order>> DeleteAsync(Order order);
    }
}
