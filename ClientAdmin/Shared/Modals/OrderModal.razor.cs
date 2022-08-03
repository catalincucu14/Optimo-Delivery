using ClientAdmin.Services.Api;
using ClientAdmin.Utils;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Models;
using Models.Resources;

#nullable disable

namespace ClientAdmin.Shared.Modals
{
    public partial class OrderModalBase : ComponentBase
    {
        [Inject]
        public ApiService ApiService { get; set; }

        [CascadingParameter]
        public IModalService Modal { get; set; }

        [CascadingParameter]
        public BlazoredModalInstance BlazoredModal { get; set; }

        [Parameter]
        public bool NoDelete { get; set; } = false;

        [Parameter]
        public bool Edit { get; set; } = false;

        [Parameter]
        public Order OrderModel { get; set; } = new();

        protected List<string> States { get; set; } = new() { "UNDELIVERED", "DELIVERED", "CANCELLED" };

        protected ComponentState OrderModalState { get; set; } = new();


        /// <summary>
        /// Function used to close the modal
        /// </summary>
        protected async Task CloseAsync() => await BlazoredModal.CancelAsync();

        /// <summary>
        /// Function used to create or update an order
        /// </summary>
        protected async Task CreateOrUpdateAsync()
        {
            // Validate the model 
            OrderModalState.Errors = OrderModel.Validate();
            if (OrderModalState.Errors.Count > 0)
            {
                return;
            }

            OrderModalState.Errors.Clear();
            OrderModalState.Processing = true;

            Response<Order> responseOrder;

            // Check if it is in edit mode
            if (Edit)
            {
                // Update the order
                responseOrder = await ApiService.Orders.UpdateAsync(OrderModel);
            }
            else
            {
                // Create the order
                responseOrder = await ApiService.Orders.CreateAsync(OrderModel);
            }

            // Check if the action was successful
            if (responseOrder.Success)
            {
                // Close the modal
                await BlazoredModal.CloseAsync(ModalResult.Ok(responseOrder.Data));
            }
            else
            {
                OrderModalState.Errors = responseOrder.Errors;
            }

            OrderModalState.Processing = false;
        }

        /// <summary>
        /// Function used to delete an order
        /// </summary>
        protected async Task DeleteAsync()
        {
            OrderModalState.Errors.Clear();
            OrderModalState.Processing = true;

            // Delete the order
            Response<Order> responseOrder = await ApiService.Orders.DeleteAsync(OrderModel);

            // Check if the action was successful
            if (responseOrder.Success)
            {
                // Close the modal
                await BlazoredModal.CloseAsync(ModalResult.Ok(false));
            }
            else
            {
                OrderModalState.Errors = responseOrder.Errors;
            }

            OrderModalState.Processing = false;
        }
    }
}
