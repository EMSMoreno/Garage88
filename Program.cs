using Garage88.Data;
using Garage88.Data.Entities;
using Garage88.Data.Repositories;
using Garage88.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Vereyon.Web;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao contêiner.
builder.Services.AddControllersWithViews();

// Configurar Identity
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

// Configurar autenticação JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(cfg =>
{
    // Configurar parâmetros de validação do token JWT
    //cfg.TokenValidationParameters = new TokenValidationParameters
    //{
    //    ValidIssuer = builder.Configuration["Tokens:Issuer"],
    //    ValidAudience = builder.Configuration["Tokens:Audience"],
    //    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Tokens:Key"])),
    //    ValidateIssuer = true,
    //    ValidateAudience = true,
    //    ValidateLifetime = true,
    //    ValidateIssuerSigningKey = true
    //};
});

// Chamar o ApplicationDbContext com o SQL Server
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Chamar os repositórios e os helpers
builder.Services.AddTransient<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddTransient<IBrandRepository, BrandRepository>();
builder.Services.AddTransient<IClientRepository, ClientRepository>();
builder.Services.AddTransient<IEstimateRepository, EstimateRepository>();
//builder.Services.AddTransient<IGenericRepository, GenericRepository>();
builder.Services.AddTransient<IMailHelper, MailHelper>();
builder.Services.AddTransient<IMechanicRepository, MechanicRepository>();
builder.Services.AddTransient<IMechanicsRolesRepository, MechanicsRolesRepository>();
builder.Services.AddTransient<IServiceRepository, ServiceRepository>();
builder.Services.AddTransient<IVehicleRepository, VehicleRepository>();
builder.Services.AddTransient<IWorkOrderRepository, WorkOrderRepository>();

builder.Services.AddTransient<IUserHelper, UserHelper>();
builder.Services.AddTransient<IConverterHelper, ConverterHelper>();


// Configurar autenticação por cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// Registar Serviços do Vereyon.Web
builder.Services.AddFlashMessage();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Ativar autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();