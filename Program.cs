using Garage88.Data;
using Garage88.Data.Entities;
using Garage88.Data.Repositories;
using Garage88.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Vereyon.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Config Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<DataContext>()
.AddDefaultTokenProviders();

// Calls ApplicationDbContext w/ SQL Server
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// SeedDb Register
builder.Services.AddScoped<SeedDb>();

// Calls Repositories & Helpers
builder.Services.AddTransient<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddTransient<IBrandRepository, BrandRepository>();
builder.Services.AddTransient<IClientRepository, ClientRepository>();
builder.Services.AddTransient<IEstimateRepository, EstimateRepository>();
builder.Services.AddTransient<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddTransient<IMechanicRepository, MechanicRepository>();
builder.Services.AddTransient<IMechanicsRolesRepository, MechanicsRolesRepository>();
builder.Services.AddTransient<IServiceRepository, ServiceRepository>();
builder.Services.AddTransient<IVehicleRepository, VehicleRepository>();
builder.Services.AddTransient<IWorkOrderRepository, WorkOrderRepository>();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddTransient<IUserHelper, UserHelper>();
builder.Services.AddTransient<IMailHelper, MailHelper>();
builder.Services.AddTransient<IConverterHelper, ConverterHelper>();
builder.Services.AddTransient<IBlobHelper, BlobHelper>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Cookie Auth
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/error/401";
    });

builder.Services.AddAuthorizationBuilder()
     .AddPolicy("Admin", policy => policy.RequireRole("Admin"))
     .AddPolicy("Client", policy => policy.RequireRole("Client"))
     .AddPolicy("Receptionist", policy => policy.RequireRole("Receptionist"))
     .AddPolicy("Technician", policy => policy.RequireRole("Technician"));

// Vereyon.Web Services
builder.Services.AddFlashMessage();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    //var seedDb = services.GetRequiredService<SeedDb>();
    //await seedDb.SeedAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error/404");
    app.UseHsts();

    // Redirect 404 errors
    app.UseStatusCodePagesWithReExecute("/error/{0}");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Active Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
