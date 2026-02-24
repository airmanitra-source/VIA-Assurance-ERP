# PowerShell script to remove old component files
$oldFilesToRemove = @(
    "ClientApp\Components\Pages\Admin\Users.razor",
    "ClientApp\Components\Pages\Admin\Users.razor.cs",
    "ClientApp\Components\Pages\Admin\Users.razor.css",
    "ClientApp\Components\Pages\Routed\AddEmployee.razor",
    "ClientApp\Components\Pages\Routed\AddEmployee.razor.cs",
    "ClientApp\Components\Pages\Routed\AddEmployee.razor.css",
    "ClientApp\Components\Pages\Routed\AddFleet.razor",
    "ClientApp\Components\Pages\Routed\AddFleet.razor.cs",
    "ClientApp\Components\Pages\Routed\AddFleet.razor.css",
    "ClientApp\Components\Pages\Routed\AddTransportation.razor",
    "ClientApp\Components\Pages\Routed\AddTransportation.razor.cs",
    "ClientApp\Components\Pages\Routed\AddTransportation.razor.css",
    "ClientApp\Components\Pages\Routed\AddWarehouse.razor",
    "ClientApp\Components\Pages\Routed\AddWarehouse.razor.cs",
    "ClientApp\Components\Pages\Routed\AddWarehouse.razor.css",
    "ClientApp\Components\Pages\Routed\CompanyDocuments.razor",
    "ClientApp\Components\Pages\Routed\CompanyDocuments.razor.cs",
    "ClientApp\Components\Pages\Routed\EmployeeDocuments.razor",
    "ClientApp\Components\Pages\Routed\EmployeeDocuments.razor.cs",
    "ClientApp\Components\Pages\Routed\EmployeesList.razor",
    "ClientApp\Components\Pages\Routed\EmployeesList.razor.cs",
    "ClientApp\Components\Pages\Routed\EmployeesList.razor.css",
    "ClientApp\Components\Pages\Routed\EmployeeSubscriptionList.razor",
    "ClientApp\Components\Pages\Routed\EmployeeSubscriptionList.razor.cs",
    "ClientApp\Components\Pages\Routed\EmployeeSubscriptionList.razor.css",
    "ClientApp\Components\Pages\Routed\FillClaims.razor",
    "ClientApp\Components\Pages\Routed\FillClaims.razor.cs",
    "ClientApp\Components\Pages\Routed\FillClaims.razor.css",
    "ClientApp\Components\Pages\Routed\FleetList.razor",
    "ClientApp\Components\Pages\Routed\FleetList.razor.cs",
    "ClientApp\Components\Pages\Routed\FleetList.razor.css",
    "ClientApp\Components\Pages\Routed\Home.razor",
    "ClientApp\Components\Pages\Routed\Home.razor.cs",
    "ClientApp\Components\Pages\Routed\Home.razor.css",
    "ClientApp\Components\Pages\Routed\Login.razor",
    "ClientApp\Components\Pages\Routed\Login.razor.cs",
    "ClientApp\Components\Pages\Routed\Login.razor.css",
    "ClientApp\Components\Pages\Routed\Register.razor",
    "ClientApp\Components\Pages\Routed\Register.razor.cs",
    "ClientApp\Components\Pages\Routed\Register.razor.css",
    "ClientApp\Components\Pages\Routed\ResetPassword.razor",
    "ClientApp\Components\Pages\Routed\ResetPassword.razor.cs",
    "ClientApp\Components\Pages\Routed\ResetPassword.razor.css",
    "ClientApp\Components\Pages\Routed\SinistersList.razor",
    "ClientApp\Components\Pages\Routed\SinistersList.razor.cs",
    "ClientApp\Components\Pages\Routed\SinistersList.razor.css",
    "ClientApp\Components\Pages\Routed\TransportationList.razor",
    "ClientApp\Components\Pages\Routed\TransportationList.razor.cs",
    "ClientApp\Components\Pages\Routed\TransportationList.razor.css",
    "ClientApp\Components\Pages\Routed\WarehouseMaterials.razor",
    "ClientApp\Components\Pages\Routed\WarehouseMaterials.razor.cs",
    "ClientApp\Components\Pages\Routed\WarehouseMaterials.razor.css",
    "ClientApp\Components\Pages\Routed\WarehouseList.razor",
    "ClientApp\Components\Pages\Routed\WarehouseList.razor.cs",
    "ClientApp\Components\Pages\Routed\WarehouseList.razor.css",
    "ClientApp\Components\Pages\Shared\Error.razor",
    "ClientApp\Components\Pages\Shared\NotFound.razor"
)

Write-Host "Removing old component files..." -ForegroundColor Green
$removedCount = 0
foreach ($filePath in $oldFilesToRemove) {
    if (Test-Path $filePath) {
        Remove-Item -Path $filePath -Force
        Write-Host "Removed: $filePath" -ForegroundColor Yellow
        $removedCount++
    }
}
Write-Host "Complete! Removed $removedCount files" -ForegroundColor Green
