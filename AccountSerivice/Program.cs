using Account.Serivice.Repositories;
using Account.Serivice.Services;
using AccountDiffService;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddScoped<AccountRepository>();
builder.Services.AddScoped<AppDbContext>();

builder.Services.AddGrpcClient<PersonsServiceRead.PersonsServiceReadClient>(o => {
    o.Address = new Uri("http://localhost:5002");
});
builder.Services.AddGrpcClient<PersonsServiceWrite.PersonsServiceWriteClient>(o => {
    o.Address = new Uri("http://localhost:5002");
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=hospital.db"));

var app = builder.Build();

app.MapGrpcService<ServiceQuery>();
app.MapGrpcService<ServiceCommand>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. " +
        "To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");


app.Run();