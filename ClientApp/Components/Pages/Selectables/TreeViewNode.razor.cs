using ClientApp.Models;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Components.Pages.Selectables
{
    public partial class TreeViewNode : ComponentBase
    {
        [Parameter]
        public EmployeeDocumentViewModel Node { get; set; } = null!;

        [Parameter]
        public EventCallback<EmployeeDocumentViewModel> OnNodeSelected { get; set; }

        [Parameter]
        public bool IsSelected { get; set; }

        [Parameter]
        public EmployeeDocumentViewModel? SelectedNode { get; set; }

        private void ToggleExpand()
        {
            Node.IsExpanded = !Node.IsExpanded;
        }

        private async Task SelectNode(EmployeeDocumentViewModel node)
        {
            await OnNodeSelected.InvokeAsync(node);
        }
    }
}
