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
    public partial class RoutesBase : ComponentBase
    {

        [Inject]
        public ApiService ApiService { get; set; }

        [CascadingParameter]
        public IModalService Modal { get; set; }

        protected List<Route> Routes { get; set; } = new();

        protected List<Driver> Drivers { get; set; } = new();

        protected List<Route> RoutesFiltered => Routes
            .Where(route => route.Search(Filter))
            .ToList();

        protected string Filter { get; set; } = string.Empty;

        protected ComponentState RoutesState { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // Get the routes from the API
            await GetRoutesAsync();

            // Get the drivers from the API
            await GetDriversAsync();
        }

        /// <summary>
        /// Function used to show the "create route" modal
        /// </summary>
        protected async Task ShowCreateRouteModalAsync()
        {
            // Get the store and check if the store is configured
            // If not the user can't create routes yet
            Response<Store> responseStore = await ApiService.Stores.GetAsync();

            if (responseStore.Success)
            {
                // Check if the store is null
                // If it is null the it has to be created
                if (responseStore.Data is null)
                {
                    ModalParameters parameters = new ModalParameters();
                    parameters.Add("Messages", new List<string> { "The Store Is Not Configured Yet" });

                    // Show the modal
                    Modal.Show<MessageModal>("Message", parameters);

                    return;
                }
            }
            else
            {
                ModalParameters parameters = new ModalParameters();
                parameters.Add("Messages", responseStore.Errors);

                // Show the modal
                Modal.Show<MessageModal>("Message", parameters);

                return;
            }

            // Get the drivers and check if at least one is created
            // If not the user can't create routes yet
            Response<List<Driver>> responseDrivers = await ApiService.Drivers.GetAsync();

            if (responseDrivers.Success)
            {
                // Check if the store is null
                // If it is null the it has to be created
                if (responseDrivers.Data.Count == 0)
                {
                    ModalParameters parameters = new ModalParameters();
                    parameters.Add("Messages", new List<string> { "No Driver Created Yet" });

                    // Show the modal
                    Modal.Show<MessageModal>("Message", parameters);

                    return;
                }
            }
            else
            {
                ModalParameters parameters = new ModalParameters();
                parameters.Add("Messages", responseDrivers.Errors);

                // Show the modal
                Modal.Show<MessageModal>("Message", parameters);

                return;
            }

            // Get the undelivered orders and check if are at least 5 undelivered orders
            // If not the user can't create routes 
            Response<List<Order>> responseOrders = await ApiService.Orders.GetAsync("UNDELIVERED");

            if (responseOrders.Success)
            {
                // Check if are at least 5 unrdelivered orders
                if (responseOrders.Data.Count < 5)
                {
                    ModalParameters parameters = new ModalParameters();
                    parameters.Add("Messages", new List<string> { $"A route should have at least 5 ({responseOrders.Data.Count} available) orders assigned" });

                    // Show the modal
                    Modal.Show<MessageModal>("Edit Order", parameters);

                    return;
                }
            }
            else
            {
                ModalParameters parameters = new ModalParameters();
                parameters.Add("Messages", responseOrders.Errors);

                // Show the modal
                Modal.Show<MessageModal>("Edit Order", parameters);

                return;
            }

            // Show the modal
            IModalReference responseModal = Modal.Show<RouteModal>("Create Route");

            // Get the result from the modal
            dynamic resultModal = await responseModal.Result;

            // Check if it was cancelled
            if (!resultModal.Cancelled)
            {
                // Get the routes from the API
                await GetRoutesAsync();
            }
        }

        /// <summary>
        /// Function used to show the "edit route" modal
        /// </summary>
        protected async Task ShowEditRouteModalAsync(Route route)
        {
            ModalParameters parameters = new ModalParameters();
            parameters.Add("RouteModel", route);
            parameters.Add("Edit", true);

            // Show the modal
            IModalReference responseModal = Modal.Show<RouteModal>("Edit Route", parameters);

            // Get the result from the modal
            dynamic resultModal = await responseModal.Result;

            // Check if it was noy cancelled
            if (!resultModal.Cancelled)
            {
                // Get the routes from the API
                await GetRoutesAsync();
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

            // Check if it was cancelled
            if (!resultModal.Cancelled)
            {
                // Get the routes from the API 
                await GetRoutesAsync();

                // Refresh the component
                StateHasChanged();
            }
        }

        /// <summary>
        /// Function used to get the routes
        /// </summary>
        protected async Task GetRoutesAsync()
        {
            RoutesState.Errors.Clear();
            RoutesState.Processing = true;

            // Get the routes from the API
            Response<List<Route>> responseRoutes = await ApiService.Routes.GetAsync();

            // Check if the action was successful
            if (responseRoutes.Success)
            {
                Routes = responseRoutes.Data;
            }
            else
            {
                RoutesState.Errors = responseRoutes.Errors;
            }

            RoutesState.Processing = false;
        }

        /// <summary>
        /// Function used to get the drivers
        /// </summary>
        protected async Task GetDriversAsync()
        {
            RoutesState.Errors.Clear();
            RoutesState.Processing = true;

            RoutesState.Processing = true;

            // Get the drivers from the API
            Response<List<Driver>> responseDrivers = await ApiService.Drivers.GetAsync();

            // Check if the action was successful
            if (responseDrivers.Success)
            {
                Drivers = responseDrivers.Data;
            }
            else
            {
                RoutesState.Errors = responseDrivers.Errors;
            }

            RoutesState.Processing = false;
        }
    }
}
