using ClientAdmin.Utils;
using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;

#nullable disable

namespace ClientAdmin.Shared.Modals
{
    public class RoutePlannerModalBase : ComponentBase
    {
        [CascadingParameter]
        public BlazoredModalInstance BlazoredModal { get; set; }

        [Parameter]
        public int QuantityAvailable { get; set; } = 0;

        [Parameter]
        public RouteGeneratorConfiguration Configuration { get; set; } = new();

        protected ComponentState RouteGeneratorModalState { get; set; } = new();

        /// <summary>
        /// Function used to close the modal
        /// </summary>
        protected async Task CloseAsync() => await BlazoredModal.CancelAsync();

        /// <summary>
        /// Function used to submit the configuration
        /// </summary>
        protected async Task SubmitAsync()
        {
            RouteGeneratorModalState.Errors.Clear();

            if (Configuration.Quantity > 24)
            {
                RouteGeneratorModalState.Errors.Add($"The quantity should be at most 24");
            }

            if (Configuration.Quantity < 5)
            {
                RouteGeneratorModalState.Errors.Add($"The quantity should be at least 5");
            }

            if (Configuration.Precision > 15)
            {
                RouteGeneratorModalState.Errors.Add($"The precision should be at most 15");
            }

            if (Configuration.Precision < 3)
            {
                RouteGeneratorModalState.Errors.Add($"The precision should be at least 3");
            }

            if (RouteGeneratorModalState.Errors.Count == 0)
            {
                await BlazoredModal.CloseAsync(ModalResult.Ok(true));
            }
        }
    }
}
