using Microsoft.EntityFrameworkCore;
using Hospital.Controllers;
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


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<DoctorRepository>();
builder.Services.AddScoped<AccountRepository>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<DoctorsService>();
builder.Services.AddScoped<AppDbContext>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=hospital.db"));

var app = builder.Build();

app.UseSession();
app.UseRouting();   

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();