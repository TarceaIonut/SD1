
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
builder.Services.AddGrpcClient<ChatRead.ChatReadClient>(o => {
    o.Address = new Uri("http://localhost:5004");
});
builder.Services.AddGrpcClient<ChatWrite.ChatWriteClient>(o => {
    o.Address = new Uri("http://localhost:5004");
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

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<NotificationService>();
builder.Services.AddSignalR();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
    .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

var app = builder.Build();

var supportedCultures = new[] { "en-US", "ro-RO" };

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);



app.MapHub<Hospital.Hubs.ChatHub>("/chatHub");

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

