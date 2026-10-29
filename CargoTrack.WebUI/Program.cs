using CargoTrack.Business;
using CargoTrack.Business.Services.Abouts;
using CargoTrack.Business.Services.Branches;
using CargoTrack.Business.Services.Cities;
using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.Abouts;
using CargoTrack.DataAccess.Repositories.Branches;
using CargoTrack.DataAccess.Repositories.Cities;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using CargoTrack.DataAccess.Repositories.Cargo;
using CargoTrack.Business.Services.Cargos;
using CargoTrack.DataAccess.Repositories.CargoPrices;
using CargoTrack.Business.Services.CargoPricing;
using CargoTrack.DataAccess.Repositories.TransferCenters;
using CargoTrack.Business.Services.TransferCenters;
using CargoTrack.DataAccess.Repositories.Employees;
using CargoTrack.Business.Services.Employees;
using CargoTrack.DataAccess.Repositories.Customers;
using CargoTrack.Business.Services.Customers;
using CargoTrack.DataAccess.Repositories.Addresses;
using CargoTrack.Business.Services.Addresses;
using CargoTrack.Business.Services.CargoPrices;
using CargoTrack.WebUI.Consts;
using Microsoft.AspNetCore.Identity;
using CargoTrack.Business.Services.Auth;
using CargoTrack.Business.Services.CurrentUser;
using CargoTrack.WebUI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters()
                .AddValidatorsFromAssembly(typeof(BusinessAssembly).Assembly);

builder.Services.AddScoped<IAboutRepository, AboutRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();

builder.Services.AddScoped<ICargoRepository, CargoRepository>();

builder.Services.AddScoped<ICargoService, CargoService>();

builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddScoped<ICargoPriceRepository, CargoPriceRepository>();

builder.Services.AddScoped<ICargoPricingService, CargoPricingService>();

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

builder.Services.AddScoped<ITransferCenterService, TransferCenterService>();

builder.Services.AddScoped<ITransferCenterRepository, TransferCenterRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();

builder.Services.AddScoped<IAboutService, AboutService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<ICityService, CityService>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IAddressService, AddressService>();

builder.Services.AddScoped<ICargoPriceService, CargoPriceService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    options.UseLazyLoadingProxies();

});

builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<AppDbContext>();

builder.Services.ConfigureApplicationCookie(config =>
{
    config.LoginPath = "/Login/Index";
    config.LogoutPath = "/Login/Logout";
    config.AccessDeniedPath = "/ErrorPages/AccessDenied";
    config.Cookie.Name = "CargoTrackCookie";
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
          );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    context.Database.Migrate();

    if (!context.Cities.Any())
    {
        var cities = new List<City>
        {
            new City { Name = "İstanbul",      Region = Region.Marmara },
            new City { Name = "Ankara",        Region = Region.CentralAnatolia },
            new City { Name = "İzmir",         Region = Region.Aegean },
            new City { Name = "Bursa",         Region = Region.Marmara },
            new City { Name = "Antalya",       Region = Region.Mediterranean },
            new City { Name = "Adana",         Region = Region.Mediterranean },
            new City { Name = "Konya",         Region = Region.CentralAnatolia },
            new City { Name = "Şanlıurfa",     Region = Region.SoutheasternAnatolia },
            new City { Name = "Gaziantep",     Region = Region.SoutheasternAnatolia },
            new City { Name = "Kocaeli",       Region = Region.Marmara },
            new City { Name = "Mersin",        Region = Region.Mediterranean },
            new City { Name = "Diyarbakır",    Region = Region.SoutheasternAnatolia },
            new City { Name = "Hatay",         Region = Region.Mediterranean },
            new City { Name = "Kayseri",       Region = Region.CentralAnatolia },
            new City { Name = "Samsun",        Region = Region.BlackSea },
            new City { Name = "Balıkesir",     Region = Region.Marmara },
            new City { Name = "Kahramanmaraş", Region = Region.Mediterranean },
            new City { Name = "Van",           Region = Region.EasternAnatolia },
            new City { Name = "Aydın",         Region = Region.Aegean },
            new City { Name = "Tekirdağ",      Region = Region.Marmara }
        };

        context.Cities.AddRange(cities);
        context.SaveChanges();
    }

    if (!context.Roles.Any())
    {
        
        var roles = new List<AppRole>
            {
                new AppRole{Name="Admin",   NormalizedName="ADMIN"},
                new AppRole{Name="Manager", NormalizedName="MANAGER"},
                new AppRole{Name="User",    NormalizedName="USER"}
            };

        context.Roles.AddRange(roles);
        context.SaveChanges();
    }

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var adminSettings = builder.Configuration.GetSection("AdminUser");
    var adminUserName = adminSettings["UserName"]!;

    if (await userManager.FindByNameAsync(adminUserName) is null)
    {
        var admin = new AppUser
        {
            UserName = adminUserName,
            Email = adminSettings["Email"],
            FirstName = "Sistem",
            LastName = "Yöneticisi",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, adminSettings["Password"]!);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, Roles.Admin);
        }
        else
        {
            var errors = string.Join(", ", result.Errors.Select(x => x.Description));
            throw new Exception($"Admin kullanıcısı oluşturulamadı: {errors}");
        }
    }

    if (!context.CargoPrices.Any())
    {
        var prices = new List<CargoPrice>
        {
            new CargoPrice { DistanceZone = DistanceZone.SameCity,    MinDesi = 0,  MaxDesi = 1,  Price = 60  },
            new CargoPrice { DistanceZone = DistanceZone.SameCity,    MinDesi = 1,  MaxDesi = 5,  Price = 80  },
            new CargoPrice { DistanceZone = DistanceZone.SameCity,    MinDesi = 5,  MaxDesi = 10, Price = 110 },
            new CargoPrice { DistanceZone = DistanceZone.SameCity,    MinDesi = 10, MaxDesi = 20, Price = 160 },
            new CargoPrice { DistanceZone = DistanceZone.SameCity,    MinDesi = 20, MaxDesi = 50, Price = 250 },

            new CargoPrice { DistanceZone = DistanceZone.SameRegion,  MinDesi = 0,  MaxDesi = 1,  Price = 75  },
            new CargoPrice { DistanceZone = DistanceZone.SameRegion,  MinDesi = 1,  MaxDesi = 5,  Price = 100 },
            new CargoPrice { DistanceZone = DistanceZone.SameRegion,  MinDesi = 5,  MaxDesi = 10, Price = 135 },
            new CargoPrice { DistanceZone = DistanceZone.SameRegion,  MinDesi = 10, MaxDesi = 20, Price = 195 },
            new CargoPrice { DistanceZone = DistanceZone.SameRegion,  MinDesi = 20, MaxDesi = 50, Price = 300 },

            new CargoPrice { DistanceZone = DistanceZone.InterRegion, MinDesi = 0,  MaxDesi = 1,  Price = 90  },
            new CargoPrice { DistanceZone = DistanceZone.InterRegion, MinDesi = 1,  MaxDesi = 5,  Price = 120 },
            new CargoPrice { DistanceZone = DistanceZone.InterRegion, MinDesi = 5,  MaxDesi = 10, Price = 150 },
            new CargoPrice { DistanceZone = DistanceZone.InterRegion, MinDesi = 10, MaxDesi = 20, Price = 230 },
            new CargoPrice { DistanceZone = DistanceZone.InterRegion, MinDesi = 20, MaxDesi = 50, Price = 360 }
        };

        context.CargoPrices.AddRange(prices);
        context.SaveChanges();
    }

}

app.Run();