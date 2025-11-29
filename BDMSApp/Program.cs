using ApplicationCore.Interfaces;
using Infrastructure.Repositories;
using ApplicationCore.Interfaces.Repositories;
using ApplicationCore.Interfaces.Services;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var cs = builder.Configuration.GetConnectionString("Default");

// Register repository so it can be injected
builder.Services.AddScoped<IBloodDriveRepository, BloodDriveRepository>();
builder.Services.AddScoped<IBloodDriveService, BloodDriveService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDonorRepository, DonorRepository>();
builder.Services.AddScoped<IDonorService, DonorService>();

// Cookie auth
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => 
    {
        options.Cookie.Name = "UserCookie";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.LoginPath = "/User/login";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();   // <-- required
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
