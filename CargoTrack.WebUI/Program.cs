using CargoTrack.Business;
using CargoTrack.Business.Services.Abouts;
using CargoTrack.Business.Services.Branches;
using CargoTrack.Business.Services.Cities;
using CargoTrack.DataAccess.Context;
using CargoTrack.DataAccess.Repositories.Abouts;
using CargoTrack.DataAccess.Repositories.Branches;
using CargoTrack.DataAccess.Repositories.Cities;
using CargoTrack.Entity.Entities;
using CargoTrack.Entity.Entities.Enums; // ✏️ Region enum'u için eklendi
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//IOC Container

builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters()
                .AddValidatorsFromAssembly(typeof(BusinessAssembly).Assembly);


builder.Services.AddScoped<IAboutRepository, AboutRepository>();
builder.Services.AddScoped<IBranchRepository, BranchRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();


builder.Services.AddScoped<IAboutService, AboutService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<ICityService, CityService>();


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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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


// Uygulama başlarken Seed Data ekleme işlemi
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // ✏️ 1) EnsureCreated yerine Migrate: migration geçmişini takip eder, sadece eksik migration'ları uygular
    context.Database.Migrate();

    // Eğer veritabanında hiç şehir yoksa ekle
    if (!context.Cities.Any())
    {
        // ✏️ 2) Her şehre Region eklendi (fiyatlandırmada "bölge içi / bölgeler arası" hesabı için gerekli)
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

   
}


app.Run();