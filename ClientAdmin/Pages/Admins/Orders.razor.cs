using ClientAdmin.Services.Api;
using ClientAdmin.Shared.Modals;
using ClientAdmin.Utils;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Models;
using Models.Resources;

#nullable disable

namespace ClientAdmin.Pages.Admins
{
    public partial class OrdersBase : ComponentBase
    {
        [Inject]
        public ApiService ApiService { get; set; }

        [CascadingParameter]
        public IModalService Modal { get; set; }

        protected List<Order> Orders = new();

        protected string Filter { get; set; } = string.Empty;

        protected ComponentState OrdersState { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // Get the orders from the API
            await GetOrdersAsync();
        }

        /// <summary>
        /// Function used to show the "create order" modal
        /// </summary>
        protected async Task ShowCreateOrderModalAsync()
        {
            // Show the modal
            IModalReference responseModal = Modal.Show<OrderModal>("Create Order");

            // Get the result from the modal
            dynamic resultModal = await responseModal.Result;

            // Check if it was not cancelled
            if (!resultModal.Cancelled)
            {
                // Get the orders from the API 
                await GetOrdersAsync();
            }
        }

        /// <summary>
        /// Function used to show the "edit order" modal
        /// </summary>
        protected async Task ShowEditOrderModalAsync(Order order, bool noDelete)
        {
            ModalParameters parameters = new ModalParameters();
            parameters.Add("OrderModel", order);
            parameters.Add("NoDelete", noDelete);
            parameters.Add("Edit", true);

            // Show the modal
            IModalReference responseModal = Modal.Show<OrderModal>("Edit Order", parameters);

            // Get the result from the modal
            dynamic resultModal = await responseModal.Result;

            // Check if it was not cancelled
            if (!resultModal.Cancelled)
            {
                // Get the orders from the API 
                await GetOrdersAsync();

                // Refresh the component
                StateHasChanged();
            }
        }

        /// <summary>
        /// Function used to show the "import orders" modal
        /// </summary>
        protected async Task ShowImportOrdersModalAsync()
        {
            // SHow th modal
            IModalReference responseModal = Modal.Show<ImportOrdersModal>();

            // Get the result from the modal
            dynamic resultModal = await responseModal.Result;

            // Check if it was not cencelled
            if (!resultModal.Cancelled)
            {
                // Get the orders from the API
                await GetOrdersAsync();
            }
        }

        /// <summary>
        /// Function used to get the undelivered orders from the API
        /// </summary>
        protected async Task GetOrdersAsync()
        {
            OrdersState.Errors.Clear();
            OrdersState.Processing = true;

            // Get the undelivered orders
            Response<List<Order>> responseOrders = await ApiService.Orders.GetAsync(null);

            // Check if the action was successful
            if (responseOrders.Success)
            {
                Orders = responseOrders.Data;
            }
            else
            {
                OrdersState.Errors = responseOrders.Errors;
            }

            OrdersState.Processing = false;
        }
    }
}
