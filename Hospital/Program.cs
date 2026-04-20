using Microsoft.EntityFrameworkCore;
using Hospital.Controllers;
using Hospital.Models;
using Hospital.Repositories;
using Hospital.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.MaxValue;
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUserService, UserService>();


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<DoctorRepository>();
builder.Services.AddScoped<AccountRepository>();
builder.Services.AddScoped<FunctionsRepository>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<DoctorsService>();
builder.Services.AddScoped<FunctionsService>();
builder.Services.AddScoped<AppDbContext>();
builder.Services.AddSingleton<NotificationService>();

// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseSqlite("Data Source=hospital.db"),ServiceLifetime.Singleton);
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(DatabaseConnectionManager.Instance.ConnectionString));



var app = builder.Build();

app.Services.GetRequiredService<NotificationService>();

app.UseSession();
app.UseRouting();   

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();