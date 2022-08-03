using ClientAdmin.Services.Api;
using ClientAdmin.Services.RoutePlanner;
using ClientAdmin.Utils;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Models;
using Models.Resources;
using Models.Utils;
using OfficeOpenXml;

#nullable disable

namespace ClientAdmin.Shared.Modals
{
    public class RouteModalBase : ComponentBase
    {
        [Inject]
        public ApiService ApiService { get; set; }

        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        public IRoutePlannerService RouteGeneratorService { get; set; }

        [CascadingParameter]
        public IModalService Modal { get; set; }

        [CascadingParameter]
        public BlazoredModalInstance BlazoredModal { get; set; }

        [Parameter]
        public bool Edit { get; set; } = false;

        [Parameter]
        public Route RouteModel { get; set; } = new();

        protected List<Order> Orders { get; set; } = new();

        protected List<Driver> Drivers { get; set; } = new();

        protected Dictionary<string, int> States { get; set; } = new();

        protected RouteGeneratorConfiguration Configuration { get; set; } = new();

        protected string Filter { get; set; } = string.Empty;

        protected List<Order> OrdersFiltered => Orders
           .Where(order => order.Search(Filter))
           .ToList();

        protected ComponentState RouteModalState { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            States = new() { { "UNCOMPLETED", 0 }, { "COMPLETED", 1 } };

            RouteModalState.Processing = true;

            // Get the drivers from the API
            Response<List<Driver>> responseDrivers = await ApiService.Drivers.GetAsync();

            // Check if the action was successful
            if (responseDrivers.Success)
            {
                Drivers = responseDrivers.Data;
            }
            else
            {
                RouteModalState.Errors = responseDrivers.Errors;
                RouteModalState.Processing = false;

                return;
            }

            // Get the undelivered orders from the API
            Response<List<Order>> responseOrders = await ApiService.Orders.GetAsync("UNDELIVERED");

            // Check if the action was successful
            if (responseOrders.Success)
            {
                Orders = responseOrders.Data;
            }
            else
            {
                RouteModalState.Errors = responseOrders.Errors;
                RouteModalState.Processing = false;

                return;
            }

            // Check if it is in edit mode
            if (Edit)
            {
                // Merge the undelivered orders and the orders of the route
                Orders.AddRange(RouteModel.Orders);

                // Check in the orders already assigned to the route
                foreach (Order order in Orders)
                {
                    if (order.RouteId > 0)
                    {
                        order.CheckBox = true;
                    }
                }
            }
            else
            {
                RouteModel.DriverId = Drivers[0].DriverId;
            }

            RouteModalState.Processing = false;
        }

        /// <summary>
        /// Function used to close the modal
        /// </summary>
        protected async Task CloseAsync() => await BlazoredModal.CancelAsync();

        /// <summary>
        /// Function used to create or update a route
        /// </summary>
        protected async Task CreateOrUpdateAsync()
        {
            // Validate the model 
            RouteModalState.Errors = RouteModel.Validate();
            if (RouteModalState.Errors.Count > 0)
            {
                return;
            }

            RouteModalState.Errors.Clear();
            RouteModalState.Processing = true;

            // Update route's orders
            RouteModel.Orders = Orders
                .Where(order => order.CheckBox)
                .ToList();

            // Check to be at least 5 orders selected (checked)
            if (RouteModel.Orders.Count < 5)
            {
                RouteModalState.Errors = new List<string> { "A Route Should Have At Least 5 Orders Assigned" };
                RouteModalState.Processing = false;

                return;
            }

            // Check to be at most 24 orders selected (checked)
            if (RouteModel.Orders.Count > 24)
            {
                RouteModalState.Errors = new List<string> { "A Route Should Have At Most 24 Orders Assigned" };
                RouteModalState.Processing = false;

                return;
            }

            Response<Route> responseRoute;

            // Check if it is in edit mode
            if (Edit)
            {
                // Update the route
                responseRoute = await ApiService.Routes.UpdateAsync(RouteModel);
            }
            else
            {
                // Create the route
                responseRoute = await ApiService.Routes.CreateAsync(RouteModel);
            }

            // Check if the action was successful
            if (responseRoute.Success)
            {
                // Close the modal
                await BlazoredModal.CloseAsync(ModalResult.Ok(responseRoute.Data));
            }
            else
            {
                RouteModalState.Errors = responseRoute.Errors;
            }

            RouteModalState.Processing = false;
        }

        /// <summary>
        /// Function used to delete a route
        /// </summary>
        protected async Task DeleteAsync()
        {
            RouteModalState.Errors.Clear();
            RouteModalState.Processing = true;

            // Delete the route
            Response<Route> responseRoute = await ApiService.Routes.DeleteAsync(RouteModel);

            // Check if the action was successful
            if (responseRoute.Success)
            {
                // Close the modal
                await BlazoredModal.CloseAsync(ModalResult.Ok(false));
            }
            else
            {
                RouteModalState.Errors = responseRoute.Errors;
            }

            RouteModalState.Processing = false;
        }

        /// <summary>
        /// Fucntion used to export an Excel file with the orders of the route
        /// </summary>
        protected async Task DownloadRouteAsync()
        {
            using (ExcelPackage package = new())
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets.Add("Sheet1");

                List<object[]> data = new()
                {
                    new object[] {"ID", "FIRST NAME", "LAST NAME", "MAIL", "PHONE", "LEFT TO PAY", "CITY", "ADDRESS", "STATE"}
                };

                // Create an array of objects with the data from all orders asigned to a route
                RouteModel.Orders.ForEach(order => data.Add(order.ToObject()));

                // Load the data into the sheet
                sheet.Cells.LoadFromArrays(data);

                using (MemoryStream bytes = new())
                {
                    package.SaveAs(bytes);

                    // Open a download window 
                    await JSRuntime.InvokeAsync<object>("saveAsFile", $"{RouteModel.Name}.xlsx", Convert.ToBase64String(bytes.ToArray()));
                }
            }
        }

        /// <summary>
        /// Function used to show the "route planner" modal
        /// </summary>
        protected async Task ShowRouteGeneratorModalAsync()
        {
            ModalParameters parameters = new ModalParameters();
            parameters.Add("Configuration", Configuration);
            parameters.Add("QuantityAvailable", Orders.Count);

            // Show the modal
            IModalReference responseModal = Modal.Show<RoutePlannerModal>("Route Planner", parameters);

            // Get the result from the modal
            dynamic resultModal = await responseModal.Result;

            // Check if it was cancelled
            if (resultModal.Cancelled)
            {
                return;
            }

            // Get the coordinates from the orders
            List<Coordinates> nodes = Orders
                .Select(order => new Coordinates()
                {
                    Latitude = order.Latitude,
                    Longitude = order.Longitude
                })
                .ToList();

            // Get the index of the selected orders
            List<int> result = RouteGeneratorService.Plan(nodes, Configuration.Quantity, Configuration.Precision);

            // Set the selected orders
            Enumerable
                .Range(0, Orders.Count)
                .ToList()
                .ForEach(node =>
                {
                    if (result.Contains(node))
                    {
                        Orders[node].CheckBox = true;
                    }
                    else
                    {
                        Orders[node].CheckBox = false;
                    }
                });

            // DontUpdatePassword the configuration
            Configuration.Quantity = 0;
            Configuration.Precision = 0;
        }
    }

    public class RouteGeneratorConfiguration
    {
        public int Quantity { get; set; }

        public int Precision { get; set; }
    }
}