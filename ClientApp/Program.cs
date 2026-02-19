using ClientApp.Components;
using ClientApp.Services;
using FileTable.Infrastructure;
using FileTable.Infrastructure.FileTableDb.DataProviders;
using FileTable.Infrastructure.Identities;
using Employee.Module;
using Company.Module;
using Subscription.Module;
using Employee.Module.Data.Providers;
using Company.Module.Data.Providers;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Components.Authorization;
using EmployeeDocuments.Module.Data.Providers;
using EmployeeDocuments.Module;
using Subscription.Module.Data.Providers;
using Company.Fleet.Module;
using Company.Fleet.Module.Data.Providers;
using Company.Warehouse.Module;
using Company.Warehouse.Module.Data.Providers;
using Company.Transportation.Module;
using Company.Transportation.Module.Data.Providers;
using Sinister.Module.Data.Providers;
using Sinister.Module;
using CompanyDocuments.Module.Data.Providers;
using CompanyDocuments.Module;
using CompanyDocuments.Module.Business;

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
builder.Services.AddScoped<IEmployeeDocumentReadOnly, EmployeeDocumentReadOnly>();
builder.Services.AddScoped<IEmployeeDocumentReadWrite, EmployeeDocumentReadWrite>();
builder.Services.AddScoped<IEmployeeReadOnly, EmployeeReadOnly>();
builder.Services.AddScoped<IEmployeeReadWrite, EmployeeReadWrite>();
builder.Services.AddScoped<ISouscriptionReadOnly, SouscriptionReadOnly>();
builder.Services.AddScoped<ISouscriptionReadWrite, SouscriptionReadWrite>();
builder.Services.AddScoped<IEntrepriseReadOnly, EntrepriseReadOnly>();
builder.Services.AddScoped<IEntrepriseFleetReadOnly, EntrepriseFleetReadOnly>();
builder.Services.AddScoped<IEntrepriseFleetReadWrite, EntrepriseFleetReadWrite>();
builder.Services.AddScoped<IEntrepriseWarehouseReadOnly, EntrepriseWarehouseReadOnly>();
builder.Services.AddScoped<IEntrepriseWarehouseReadWrite, EntrepriseWarehouseReadWrite>();
builder.Services.AddScoped<IEntrepriseMerchandiseTransportationReadOnly, EntrepriseMerchandiseTransportationReadOnly>();
builder.Services.AddScoped<IEntrepriseMerchandiseTransportationReadWrite, EntrepriseMerchandiseTransportationReadWrite>();
// Sinister policy providers (policy metadata storage)
builder.Services.AddScoped<ISinisterPolicyReadOnly, FileTable.Infrastructure.FileTableDb.DataProviders.SinisterPolicyReadOnly>();
builder.Services.AddScoped<ISinisterPolicyReadWrite, FileTable.Infrastructure.FileTableDb.DataProviders.SinisterPolicyReadWrite>();
builder.Services.AddScoped<ICompanyDocumentReadOnly, CompanyDocumentReadOnly>();
builder.Services.AddScoped<ICompanyDocumentReadWrite, CompanyDocumentReadWrite>();
builder.Services.AddScoped<ISinisterModule, SinisterModule>();
builder.Services.AddScoped<IEmployeeDocumentModule, EmployeeDocumentModule>();
builder.Services.AddScoped<IEmployeeModule, EmployeeModule>();
builder.Services.AddScoped<ISubscriptionModule, SubscriptionModule>();
builder.Services.AddScoped<ISubscriptionExternalService, FakeSubscriptionExternalService>();
builder.Services.AddScoped<ICompanyModule, CompanyModule>();
builder.Services.AddScoped<ICompanyFleetModule, CompanyFleetModule>();
builder.Services.AddScoped<ICompanyWarehouseModule, CompanyWarehouseModule>();
builder.Services.AddScoped<ICompanyTransportationModule, CompanyTransportationModule>();
builder.Services.AddScoped<ICompanyDocumentModule, CompanyDocumentModule>();
builder.Services.AddScoped<IPolicyGenerator, PolicyGenerator>();
builder.Services.AddScoped<ISignatureService, SignatureService>();
// Authentication service - registered as Scoped to isolate auth state per user session
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<UserManagementService>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthenticationStateProvider>());
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
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapControllers();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
