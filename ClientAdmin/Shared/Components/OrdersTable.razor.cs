using ClientAdmin.Utils;
using Microsoft.AspNetCore.Components;
using Models.Resources;

namespace ClientAdmin.Shared.Components
{
    public class OrdersTableBase : ComponentBase
    {
        [Parameter]
        public Func<Order, bool, Task> ShowEditOrderModalAsync { get; set; } = default!;

        [Parameter]
        public bool CheckBox { get; set; } = false;

        [Parameter]
        public bool NoDelete { get; set; } = false;

        [Parameter]
        public bool Processing { get; set; } = false;

        [Parameter]
        public bool Update { get; set; } = false;

        [Parameter]
        public string Filter { get; set; } = string.Empty;

        [Parameter]
        public List<Order> Orders { get; set; } = new();

        protected List<Order> OrdersFiltered => Orders
            .Where(order => order.Search(Filter))
            .ToList();

        protected FiltersEnum SelectedField { get; set; } = FiltersEnum.CustomId;

        public bool SortAscending { get; set; } = false;
    }
}
