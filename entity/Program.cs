using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using entity.Db;


string env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", EnvironmentVariableTarget.Process);

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllersWithViews();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", EnvironmentVariableTarget.Process)}.json", optional: true)
    .AddEnvironmentVariables()
    .Build();


services.AddDbContext<MsSqlDbContext>(options => options.UseSqlServer(config.GetConnectionString("MsSQL")));
services.AddDbContext<PgDbContext>(options => options.UseNpgsql(config.GetConnectionString("PgSQL")));

if (config.GetValue<string>("DbProvider") == "Ms")
{
    services.AddScoped<IAppDbContext>(provider => provider.GetService<MsSqlDbContext>());
}
else
{
    services.AddScoped<IAppDbContext>(provider => provider.GetService<PgDbContext>());
}

var app = builder.Build();

app.UseHttpsRedirection();

//if (app.Environment.IsDevelopment())
//{

//};

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/env", () => env);

app.MapGet("/articles", (IAppDbContext context) => context.Articles.ToListAsync<Article>());

app.MapGet("/article/{id}", (IAppDbContext context, int id) =>
{
    return context.Articles.Where(article => article.Id == id).ToList();
});

app.MapPost("/articles", (IAppDbContext context, [FromBody] Article article) =>
{
    context.Articles.Add(article);
    context.SaveChanges();
    return article;
});

app.MapPut("/article", (IAppDbContext context, [FromBody] Article article) =>
{
    var findArticle = context.Articles.Find(article.Id);
    if (findArticle is null) return null;
    context.Articles.Remove(findArticle);
    context.Articles.Update(article);
    context.SaveChanges();
    return article;
});

app.MapDelete("/article/{id}", (IAppDbContext context, int id) => {
    var findArticle = context.Articles.Find(id);

    if(findArticle is null) return null;

    context.Articles.Remove(findArticle);
    context.SaveChanges();
    return findArticle;
});

app.Run();
