# PowerShell script to rename all Blazor page components to add 'Component' suffix
# Run this from the repository root directory

$componentsToRename = @(
    # Admin folder
    @{Old="Users"; New="UsersComponent"; Path="ClientApp\Components\Pages\Admin"},
    
    # Routed folder
    @{Old="AddEmployee"; New="AddEmployeeComponent"; Path="ClientApp\Components\Pages\Routed"},
    @{Old="AddFleet"; New="AddFleetComponent"; Path="ClientApp\Components\Pages\Routed"},
    @{Old="AddTransportation"; New="AddTransportationComponent"; Path="ClientApp\Components\Pages\Routed"},
    @{Old="AddWarehouse"; New="AddWarehouseComponent"; Path="ClientApp\Components\Pages\Routed"},
    @{Old="CompanyDocuments"; New="CompanyDocumentsComponent"; Path="ClientApp\Components\Pages\Routed"},
    @{Old="EmployeeDocuments"; New="EmployeeDocumentsComponent"; Path="ClientApp\Components\Pages\Routed"},
    @{Old="EmployeesList"; New="EmployeesListComponent"; Path="ClientApp\Components\Pages\Routed"},
    @{Old="EmployeeSubscriptionList"; New="EmployeeSubscriptionListComponent"; Path="ClientApp\Components\Pages\Routed"},
    @{Old="FillClaims"; New="FillClaimsComponent"; Path="ClientApp\Components\Pages\Routed"},
    @{Old="FleetList"; New="FleetListComponent"; Path="ClientApp\Components\Pages\Routed"},
    @{Old="Home"; New="HomeComponent"; Path="ClientApp\Components\Pages\Routed"},
    @{Old="Login"; New="LoginComponent"; Path="ClientApp\Components\Pages\Routed"},
    @{Old="Register"; New="RegisterComponent"; Path="ClientApp\Components\Pages\Routed"},
    @{Old="ResetPassword"; New="ResetPasswordComponent"; Path="ClientApp\Components\Pages\Routed"},
    @{Old="SinistersList"; New="SinistersListComponent"; Path="ClientApp\Components\Pages\Routed"},
    @{Old="TransportationList"; New="TransportationListComponent"; Path="ClientApp\Components\Pages\Routed"},
    @{Old="WarehouseMaterials"; New="WarehouseMaterialsComponent"; Path="ClientApp\Components\Pages\Routed"},
    @{Old="WarehouseList"; New="WarehouseListComponent"; Path="ClientApp\Components\Pages\Routed"},
    
    # Selectables folder
    @{Old="CompanyDocumentPreview"; New="CompanyDocumentPreviewComponent"; Path="ClientApp\Components\Pages\Selectables"},
    @{Old="EmployeeDocumentPreview"; New="EmployeeDocumentPreviewComponent"; Path="ClientApp\Components\Pages\Selectables"},
    @{Old="EmployeeWordDocumentPreview"; New="EmployeeWordDocumentPreviewComponent"; Path="ClientApp\Components\Pages\Selectables"},
    @{Old="TreeViewNode"; New="TreeViewNodeComponent"; Path="ClientApp\Components\Pages\Selectables"},
    
    # Shared folder
    @{Old="Error"; New="ErrorComponent"; Path="ClientApp\Components\Pages\Shared"},
    @{Old="NotFound"; New="NotFoundComponent"; Path="ClientApp\Components\Pages\Shared"}
)

$fileExtensions = @(".razor", ".razor.cs", ".razor.css")

Write-Host "Starting component renaming process..." -ForegroundColor Green
Write-Host ""

foreach ($component in $componentsToRename) {
    $oldName = $component.Old
    $newName = $component.New
    $basePath = $component.Path
    
    Write-Host "Processing: $oldName -> $newName in $basePath" -ForegroundColor Cyan
    
    foreach ($ext in $fileExtensions) {
        $oldFile = Join-Path $basePath "$oldName$ext"
        $newFile = Join-Path $basePath "$newName$ext"
        
        if (Test-Path $oldFile) {
            # Rename the file
            Write-Host "  Renaming: $oldFile -> $newFile" -ForegroundColor Yellow
            Rename-Item -Path $oldFile -NewName "$newName$ext" -Force
            
            # Update class name in .cs file
            if ($ext -eq ".razor.cs") {
                $content = Get-Content $newFile -Raw
                $pattern = "public partial class $oldName\s*:"
                $replacement = "public partial class $newName :"
                $updatedContent = $content -replace $pattern, $replacement
                Set-Content -Path $newFile -Value $updatedContent -NoNewline
                Write-Host "  Updated class name in: $newFile" -ForegroundColor Green
            }
        }
        else {
            Write-Host "  File not found: $oldFile (skipping)" -ForegroundColor DarkGray
        }
    }
    
    Write-Host ""
}

Write-Host "Component renaming complete!" -ForegroundColor Green
Write-Host ""
Write-Host "IMPORTANT: You may need to:" -ForegroundColor Yellow
Write-Host "1. Update any direct references to these components in other files" -ForegroundColor Yellow
Write-Host "2. Rebuild the solution to ensure everything compiles correctly" -ForegroundColor Yellow
Write-Host "3. Check for any dynamic component loading that uses string names" -ForegroundColor Yellow
