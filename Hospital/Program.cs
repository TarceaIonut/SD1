
using Hospital.Models;
using Hospital.Service;

using AccountDiffService;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddGrpcClient<AccountServiceRead.AccountServiceReadClient>(o => {
    o.Address = new Uri("http://localhost:5001");
});
builder.Services.AddGrpcClient<AccountServiceWrite.AccountServiceWriteClient>(o => {
    o.Address = new Uri("http://localhost:5001");
});
builder.Services.AddGrpcClient<PersonsServiceRead.PersonsServiceReadClient>(o => {
    o.Address = new Uri("http://localhost:5002");
});
builder.Services.AddGrpcClient<PersonsServiceWrite.PersonsServiceWriteClient>(o => {
    o.Address = new Uri("http://localhost:5002");
});
builder.Services.AddGrpcClient<DoctorCheckupRead.DoctorCheckupReadClient>(o => {
    o.Address = new Uri("http://localhost:5003");
});
builder.Services.AddGrpcClient<DoctorCheckupWrite.DoctorCheckupWriteClient>(o => {
    o.Address = new Uri("http://localhost:5003");
});


builder.Services.AddLogging();
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
builder.Services.AddSingleton<NotificationService>();


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

