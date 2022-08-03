using Blazored.Modal;
using Microsoft.AspNetCore.Components;

#nullable disable

namespace ClientAdmin.Shared.Modals
{
    public class MessageModalBase : ComponentBase
    {
        [CascadingParameter]
        public BlazoredModalInstance BlazoredModal { get; set; }

        [Parameter]
        public List<string> Messages { get; set; } = new();

        /// <summary>
        /// Function used to return close the modal
        /// </summary>
        protected async Task OkAsync() => await BlazoredModal.CancelAsync();
    }
}
