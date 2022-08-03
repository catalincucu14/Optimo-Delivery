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
    public class StoreModalBase : ComponentBase
    {
        [Inject]
        public ApiService ApiService { get; set; }

        [CascadingParameter]
        public BlazoredModalInstance BlazoredModal { get; set; }

        protected Store StoreModel { get; set; } = new();

        protected ComponentState StoreModalState { get; set; } = new();

        protected bool Update { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            // Get the store from the API
            await GetStoreAsync();
        }

        /// <summary>
        /// Function used to close the modal
        /// </summary>
        protected async Task CloseAsync() => await BlazoredModal.CloseAsync(ModalResult.Ok(true));

        /// <summary>
        /// Function used to create or update the store
        /// </summary>
        protected async Task CreateOrUpdateAsync()
        {
            StoreModalState.Errors.Clear();
            StoreModalState.Processing = true;

            Response<Store> responseStore;

            // Check if it is in edit mode
            if (Update)
            {
                // Update the store
                responseStore = await ApiService.Stores.UpdateAsync(StoreModel);
            }
            else
            {
                // Create the store
                responseStore = await ApiService.Stores.CreateAsync(StoreModel);
            }

            StoreModalState.Processing = false;

            // Check if the action was successful
            if (responseStore.Success)
            {
                // Close the modal
                await BlazoredModal.CloseAsync(ModalResult.Ok(true));
            }
            else
            {
                StoreModalState.Errors = responseStore.Errors;
            }
        }

        /// <summary>
        /// Function used to get the store from the API
        /// </summary>
        protected async Task GetStoreAsync()
        {
            StoreModalState.Errors.Clear();
            StoreModalState.Processing = true;

            // Get the store
            Response<Store> responseStore = await ApiService.Stores.GetAsync();

            // Check if the action was successful
            if (responseStore.Success)
            {
                // Check if the store is null
                // If it is null the it has to be created
                if (responseStore.Data is not null)
                {
                    StoreModel = responseStore.Data;

                    Update = true;
                }
            }
            else
            {
                StoreModalState.Errors = responseStore.Errors;
            }

            StoreModalState.Processing = false;
        }
    }
}
