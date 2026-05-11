using Account.Serivice.Repositories;
using AccountDiffService;
using Microsoft.EntityFrameworkCore;
using PersonService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddScoped<AppDbContext>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=hospital.db"));

var app = builder.Build();



app.MapGrpcService<ServiceQuery>();
//app.MapGrpcService<ServiceCommand>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");


app.Run();