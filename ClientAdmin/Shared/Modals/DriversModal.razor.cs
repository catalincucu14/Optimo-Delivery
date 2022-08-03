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
    public class DriversModalBase : ComponentBase
    {
        [Inject]
        public ApiService ApiService { get; set; }

        [CascadingParameter]
        public IModalService Modal { get; set; }

        [CascadingParameter]
        protected BlazoredModalInstance BlazoredModal { get; set; }

        protected List<Driver> Drivers { get; set; } = new();

        protected ComponentState DriversModalState { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // Get the drivers from the API
            await GetDriversAsync();
        }

        /// <summary>
        /// Function used to close the modal
        /// </summary>
        protected async Task CloseAsync() => await BlazoredModal.CancelAsync();

        /// <summary>
        /// Function used to show the "create driver" modal
        /// </summary>
        protected async Task ShowCreateDriverModalAsync()
        {
            // Show the modal
            IModalReference responseModal = Modal.Show<DriverModal>("Create Driver");

            // Get the result from the modal
            dynamic resultModal = await responseModal.Result;

            // Check if it was cancelled
            if (!resultModal.Cancelled)
            {
                // Get the drivers from the API
                await GetDriversAsync();
            }
        }

        /// <summary>
        /// Function used to show the "edit driver" modal
        /// </summary>
        protected async Task ShowEditDriverModalAsync(Driver driver)
        {
            ModalParameters parameters = new ModalParameters();
            parameters.Add("DriverModel", driver);
            parameters.Add("Edit", true);

            // Show the modal
            IModalReference responseModal = Modal.Show<DriverModal>("Edit Driver", parameters);

            // Get the result from the modal
            dynamic resultModal = await responseModal.Result;

            // Check if it was cancelled
            if (!resultModal.Cancelled)
            {
                // Get the drivers from the API
                await GetDriversAsync();
            }
        }

        protected async Task GetDriversAsync()
        {
            DriversModalState.Processing = true;

            // Get the drivers from the API
            Response<List<Driver>> responseDrivers = await ApiService.Drivers.GetAsync();

            // Check if the action was successful
            if (responseDrivers.Success)
            {
                Drivers = responseDrivers.Data;
            }
            else
            {
                DriversModalState.Errors = responseDrivers.Errors;
            }

            DriversModalState.Processing = false;
        }
    }
}
