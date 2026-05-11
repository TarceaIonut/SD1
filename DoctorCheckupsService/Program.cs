using AccountDiffService;
using DoctorCheckupsService.Repositories;
using DoctorCheckupsService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

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


builder.Services.AddScoped<AppDbContext>();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=doctor_checkup.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ServiceRead>();
app.MapGrpcService<ServiceWrite>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();