using ClientApp.Models;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Components.Shared
{
    public partial class PaySlipPreviewComponent : ComponentBase
    {
        [Parameter, EditorRequired]
        public PaySlipViewModel PaySlip { get; set; } = default!;

        [Parameter]
        public RenderFragment? Actions { get; set; }

        [Parameter]
        public string? PeriodLabel { get; set; }
    }
}
