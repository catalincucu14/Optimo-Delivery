using Blazored.Modal;
using Blazored.Modal.Services;
using ClientAdmin.Services.Api;
using ClientAdmin.Utils;
using Microsoft.AspNetCore.Components;
using Models;
using Models.Authentication;
using Models.Resources;

#nullable disable

namespace ClientAdmin.Shared.Modals
{
    public class DriverModalBase : ComponentBase
    {
        [Inject]
        public ApiService ApiService { get; set; }

        [CascadingParameter]
        public IModalService Modal { get; set; }

        [CascadingParameter]
        public BlazoredModalInstance BlazoredModal { get; set; }

        [Parameter]
        public bool Edit { get; set; } = false;

        [Parameter]
        public Driver DriverModel { get; set; } = new();

        public RegisterDriverRequest RegisterRequestModel { get; set; } = new();

        protected string Password { get; set; } = string.Empty;

        protected bool DontUpdatePassword { get; set; } = false;

        protected ComponentState DriverModalState { get; set; } = new();

        protected override void OnInitialized()
        {
            base.OnInitialized();

            RegisterRequestModel = new()
            {
                Mail = DriverModel.Mail,
                Name = DriverModel.Name
            };

            if (Edit)
            {
                DontUpdatePassword = true;
            }
        }

        /// <summary>
        /// Function used to close the modal
        /// </summary>
        protected async Task CloseAsync() => await BlazoredModal.CancelAsync();

        /// <summary>
        /// Function used to create or update a driver
        /// </summary>
        protected async Task CreateOrUpdateAsync()
        {
            Console.WriteLine(1);
            // Set the password to the model
            RegisterRequestModel.Password = Password;
            if (string.IsNullOrEmpty(RegisterRequestModel.Password))
            {
                RegisterRequestModel.Password = null;
            }

            // Validate the model
            DriverModalState.Errors = RegisterRequestModel.Validate();
            if (DriverModalState.Errors.Count > 0)
            {
                Console.WriteLine(1);
                return;
            }

            if (!Edit || !DontUpdatePassword)
            {
                if (string.IsNullOrEmpty(RegisterRequestModel.Password))
                {
                    DriverModalState.Errors = new() { "PASSWORD required" };

                    return;
                }
            }
            else if (DontUpdatePassword)
            {
                RegisterRequestModel.Password = null;
            }

            DriverModalState.Errors.Clear();
            DriverModalState.Processing = true;

            Response<Driver> responseDriver;

            // Check if it is in edit mode
            if (Edit)
            {
                if (DontUpdatePassword)
                {
                    RegisterRequestModel.Password = null;
                }

                // Update the driver
                responseDriver = await ApiService.Drivers.UpdateAsync(RegisterRequestModel, DriverModel.DriverId);
            }
            else
            {
                // Create the driver
                responseDriver = await ApiService.Drivers.CreateAsync(RegisterRequestModel);
            }

            // Check if the action was successful
            if (responseDriver.Success)
            {
                // Close the modal
                await BlazoredModal.CloseAsync(ModalResult.Ok(responseDriver.Data));
            }
            else
            {
                DriverModalState.Errors = responseDriver.Errors;
            }

            DriverModalState.Processing = false;
        }
    }
}
