using Account.Serivice.Repositories;
using AccountDiffService;
using ChatService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=hospital.db"));

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddGrpcClient<AccountServiceRead.AccountServiceReadClient>(o => {
    o.Address = new Uri("http://localhost:5001");
});
builder.Services.AddGrpcClient<AccountServiceWrite.AccountServiceWriteClient>(o => {
    o.Address = new Uri("http://localhost:5001");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ChatServiceQuery>();
app.MapGrpcService<ChatServiceCommand>();

app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();