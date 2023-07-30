using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using entity.Db;

var builder = WebApplication.CreateBuilder(args);

//var configBuilder = builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
//    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
//    .AddJsonFile("appsettings.Development.json", optional: true);

//IConfigurationRoot config = configBuilder.Build();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var services = builder.Services;

services.AddDbContext<MsSqlDbContext>(options => options.UseSqlServer("Server=localhost;Database=TestData;User Id=sa;Password=dev_2020!;TrustServerCertificate=True;Encrypt=false"));
services.AddDbContext<PgDbContext>(options => options.UseNpgsql("Host=localhost;Database=postgres;Port=5432;Username=postgres;Password=1111"));

var dbProvider = "Ms";
if (dbProvider == "Ms")
{
    services.AddScoped<IAppDbContext>(provider => provider.GetService<MsSqlDbContext>());
}
else
{
    services.AddScoped<IAppDbContext>(provider => provider.GetService<PgDbContext>());
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
};

app.MapGet("/", ([FromServices] IAppDbContext context) => context.Articles.ToList());

app.Run();
