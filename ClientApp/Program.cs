using ClientApp.Components;
using ClientApp.Services;
using ClientApp.Controllers;
using ClientApp;
using FileTable.Infrastructure;
using FileTable.Infrastructure.FileTableDb.DataProviders;
using FileTable.Infrastructure.Identities;
using Employee.Module;
using Company.Module;
using Subscription.Module;
using Employee.Module.Data.Providers;
using CompanyPayroll.Module;
using CompanyPayroll.Module.Data.Providers;
using EmployeePayroll.Module;
using EmployeePayroll.Module.Data.Providers;
using EmployeeTimesheet.Module;
using EmployeeTimesheet.Module.Data.Providers;
using PaySlip.Module;
using PaySlip.Module.Data.Providers;
using Project.Module;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Components.Authorization;
using EmployeeDocuments.Module.Data.Providers;
using EmployeeDocuments.Module;
using Company.Fleet.Module;
using Company.Warehouse.Module;
using Company.Transportation.Module;
using Sinister.Module.Data.Providers;
using Sinister.Module;
using CompanyDocuments.Module.Data.Providers;
using CompanyDocuments.Module;
using CompanyDocuments.Module.Business;
using Company.Sinister.Module;
using Company.Sinister.Module.Data.Providers;
using FileTable.Infrastructure.Abstractions;
using CompanySinisterDocument.Module;
using CompanySinisterDocument.Module.Data.Providers;
using Project.Module.Data.Providers;
using Subscription.Module.Data.Providers;
using Company.Module.Data.Providers;
using Company.Fleet.Module.Data.Providers;
using Company.Warehouse.Module.Data.Providers;
using Company.Transportation.Module.Data.Providers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
    .AddUserStore<ApplicationUserStore>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
})
.AddCookie(IdentityConstants.ApplicationScheme, options =>
{
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
});
// Register FileTable services with proper layer separation
builder.Services.AddScoped<FileTableDbContext>();
builder.Services.AddScoped<ApplicationUserStore>();
builder.Services.AddScoped<IEmployeeDocumentReadOnlyDataProvider, EmployeeDocumentReadOnly>();
builder.Services.AddScoped<IEmployeeDocumentReadWriteDataProvider, EmployeeDocumentReadWrite>();
builder.Services.AddScoped<IEmployeeReadOnlyDataProvider, EmployeeReadOnly>();
builder.Services.AddScoped<IEmployeeReadWriteDataProvider, EmployeeReadWrite>();
builder.Services.AddScoped<IReadEmployeeTimesheetDataProvider, EmployeeTimesheetReadWrite>();
builder.Services.AddScoped<IReadWriteEmployeeTimesheetDataProvider, EmployeeTimesheetReadWrite>();
builder.Services.AddScoped<IReadEmployeePayrollDataProvider, EmployeePayrollReadOnly>();
builder.Services.AddScoped<IEmployeePayrollReadWriteDataProvider, EmployeePayrollReadWrite>();
builder.Services.AddScoped<ICompanyPayrollSettingsReadOnlyDataProvider, CompanyPayrollSettingsReadOnly>();
builder.Services.AddScoped<ICompanyPayrollSettingsReadWriteDataProvider, CompanyPayrollSettingsReadWrite>();
builder.Services.AddScoped<IEntrepriseReadOnlyDataProvider, EntrepriseReadOnly>();
builder.Services.AddScoped<IEntrepriseFleetReadOnlyDataProvider, EntrepriseFleetReadOnly>();
builder.Services.AddScoped<IEntrepriseFleetReadWriteDataProvider, EntrepriseFleetReadWrite>();
builder.Services.AddScoped<IEntrepriseWarehouseReadOnlyDataProvider, EntrepriseWarehouseReadOnly>();
builder.Services.AddScoped<IEntrepriseWarehouseReadWriteDataProvider, EntrepriseWarehouseReadWrite>();
builder.Services.AddScoped<IEntrepriseMerchandiseTransportationReadOnlyDataProvider, EntrepriseMerchandiseTransportationReadOnly>();
builder.Services.AddScoped<IEntrepriseMerchandiseTransportationReadWriteDataProvider, EntrepriseMerchandiseTransportationReadWrite>();
builder.Services.AddScoped<IProjectReadOnlyDataProvider, ProjectReadOnly>();
builder.Services.AddScoped<ISouscriptionReadOnlyDataProvider, SouscriptionReadOnly>();
builder.Services.AddScoped<ISouscriptionReadWriteDataProvider, SouscriptionReadWrite>();
builder.Services.AddScoped<IPayrollPeriodReadOnlyDataProvider, PayrollPeriodReadOnly>();
builder.Services.AddScoped<IPayrollPeriodReadWriteDataProvider, PayrollPeriodReadWrite>();
builder.Services.AddScoped<IPaySlipLineReadOnlyDataProvider, PaySlipLineReadOnly>();
builder.Services.AddScoped<IPaySlipLineReadWriteDataProvider, PaySlipLineReadWrite>();
builder.Services.AddScoped<IPaySlipReadWriteDataProvider, PaySlipReadWrite>();
builder.Services.AddScoped<IPaySlipModificationRequestReadOnlyDataProvider, PaySlipModificationRequestReadOnly>();
builder.Services.AddScoped<IPaySlipModificationRequestReadWriteDataProvider, PaySlipModificationRequestReadWrite>();
builder.Services.AddScoped<IPaySlipSecondEntryReadOnlyDataProvider, PaySlipSecondEntryReadOnly>();
builder.Services.AddScoped<IPaySlipSecondEntryReadWriteDataProvider, PaySlipSecondEntryReadWrite>();
// Sinister policy providers (policy metadata storage)
builder.Services.AddScoped<ISinisterPolicyReadOnlyDataProvider, SinisterPolicyReadOnly>();
builder.Services.AddScoped<ISinisterPolicyReadWriteDataProvider, SinisterPolicyReadWrite>();
// Company sinister (claims) providers
builder.Services.AddScoped<ICompanySinisterReadOnlyDataProvider, CompanySinisterReadOnly>();
builder.Services.AddScoped<ICompanySinisterReadWriteDataProvider, CompanySinisterReadWrite>();
builder.Services.AddScoped<ICompanySinisterTypeReadonlyDataProvider, CompanySinisterTypeReadOnly>();
builder.Services.AddScoped<ICompanySinisterTypeReadWriteDataProvider, CompanySinisterTypeReadWrite>();
builder.Services.AddScoped<ISinisterTypeReadonlyDataProvider, SinisterTypeReadOnly>();
builder.Services.AddScoped<ITransactionDetector, TransactionDetector>();
builder.Services.AddScoped<ITransactionHandler, TransactionHandler>();
builder.Services.AddScoped<ICompanyDocumentReadOnlyDataProvider, CompanyDocumentReadOnly>();
builder.Services.AddScoped<ICompanyDocumentReadWriteDataProvider, CompanyDocumentReadWrite>();
builder.Services.AddScoped<ICompanySinisterDocumentReadOnlyDataProvider, CompanySinisterDocumentReadOnly>();
builder.Services.AddScoped<ICompanySinisterDocumentReadWriteDataProvider, CompanySinisterDocumentReadWrite>();
builder.Services.AddScoped<ISinisterModule, SinisterModule>();
builder.Services.AddScoped<ICompanySinisterModule, CompanySinisterModule>();
builder.Services.AddScoped<IEmployeeDocumentModule, EmployeeDocumentModule>();
builder.Services.AddScoped<IEmployeeModule, EmployeeModule>();
builder.Services.AddScoped<ICompanyPayrollModule, CompanyPayrollModule>();
builder.Services.AddScoped<IEmployeePayrollModule, EmployeePayrollModule>();
builder.Services.AddScoped<IEmployeeTimesheetModule, EmployeeTimesheetModule>();
builder.Services.AddScoped<IPaySlipModule, PaySlipModule>();
builder.Services.AddScoped<IProjectModule, ProjectModule>();
builder.Services.AddScoped<ISubscriptionModule, SubscriptionModule>();
builder.Services.AddScoped<ISubscriptionExternalService, FakeSubscriptionExternalService>();
builder.Services.AddScoped<ICompanyModule, CompanyModule>();
builder.Services.AddScoped<ICompanyFleetModule, CompanyFleetModule>();
builder.Services.AddScoped<ICompanyWarehouseModule, CompanyWarehouseModule>();
builder.Services.AddScoped<ICompanyTransportationModule, CompanyTransportationModule>();
builder.Services.AddScoped<ICompanyDocumentModule, CompanyDocumentModule>();
builder.Services.AddScoped<ICompanySinisterDocumentModule, CompanySinisterDocumentModule>();
builder.Services.AddScoped<IPolicyGenerator, PolicyGenerator>();
builder.Services.AddScoped<ISignatureService, SignatureService>();
// UI Controllers - handle mapping and non-business logic for Razor components
builder.Services.AddScoped<EmployeeController>();
builder.Services.AddScoped<EmployeeDashboardController>();
builder.Services.AddScoped<McpTools>();
builder.Services.AddScoped<ClaimController>();
builder.Services.AddScoped<CompanyController>();
builder.Services.AddScoped<FleetController>();
builder.Services.AddScoped<ResetPasswordController>();
builder.Services.AddScoped<TransportationController>();
builder.Services.AddScoped<WarehouseController>();
builder.Services.AddScoped<SubscriptionController>();
builder.Services.AddScoped<CompanyDocumentsController>();
builder.Services.AddScoped<SinisterListController>();
builder.Services.AddScoped<EmployeeDocumentsController>();
builder.Services.AddScoped<PayrollController>();
builder.Services.AddScoped<CompanyPayrollSettingsController>();
// Authentication service - registered as Scoped to isolate auth state per user session
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<UserManagementService>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthenticationStateProvider>());
builder.Services.AddScoped<FileTable.Infrastructure.Services.IEmailService, FileTable.Infrastructure.Services.EmailService>();

builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

// Add localization services
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddScoped<LocalizationService>();
var supportedCultures = new[] { "en-US", "fr-FR" };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("fr-FR");
    options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
    options.SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
    options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddControllers();

var app = builder.Build();

// Use localization
var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
// app.UseHttpsRedirection(); // Disable for Local MCP Debugging

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapMcp("/mcp");
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

