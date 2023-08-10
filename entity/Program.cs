using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using entity.Db;
using entity.Models;
using Microsoft.Data.SqlClient;
using Dapper;

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

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/articles", (IAppDbContext context) => context.Articles.ToListAsync<Article>());

app.MapGet("/article/{id}", (IAppDbContext context, int id) =>
{
    return context.Articles.Where(article => article.Id == id).ToList();
});

app.MapPost("/articles", (IAppDbContext context, [FromBody] Article article) =>
{
    var validator = new ArticleValidator();

    var result = validator.Validate(article);
    if (!result.IsValid)
    {
        return new ApiResponse()
        {
            Code = "Failse",
            Data = result
        };
    }
    else
    {
        context.Articles.Add(article);
        context.SaveChanges();
        return new ApiResponse()
        {
            Code = "Failse",
            Data = article
        };
    }
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


app.MapGet("/dapper/article", () =>
{
    string sql = "SELECT * FROM Article";
    using SqlConnection con = new SqlConnection(config.GetConnectionString("MsSQL"));

    var res = con.Query<Article>(sql);

    return res;

})
.WithName("GetArticle")
.WithOpenApi();

app.MapGet("/dapper/article/{id}", (int id) => {
    string sql = "SELECT * FROM Article WHERE id = @Id";
    var categories = new List<Article>();
    SqlConnection con = new SqlConnection(config.GetConnectionString("MsSQL"));

    var res = con.Query<Article>(sql, new { Id = id });

    return res;

});

app.MapPost("/dapper/article", (Article article) =>
{
    string sql = "INSERT INTO Article (Title, Url, Content) VALUES (@Title, @Url, @Content)";
    var categories = new List<Article>();
    SqlConnection con = new SqlConnection(config.GetConnectionString("MsSQL"));

    var res = con.Execute(sql, new { Title = article.Title, Url = article.Url, Content = article.Content });

    return article;
});

app.MapPut("/dapper/article/{id}", (Article article, int id) =>
{
    string sql = "UPDATE Article SET Title = @Title, Url = @Url, Content = @Content WHERE Id = @Id";
    var categories = new List<Article>();
    SqlConnection con = new SqlConnection(config.GetConnectionString("MsSQL"));

    var res = con.Execute(sql, new { Title = article.Title, Url = article.Url, Content = article.Content, Id = id });

    return res;
});

app.MapDelete("/dapper/article/{id}", (int id) =>
{
    string sql = "DELETE FROM Article WHERE Id = @Id";
    var categories = new List<Article>();
    SqlConnection con = new SqlConnection(config.GetConnectionString("MsSQL"));

    var res = con.Query<Article>(sql, new { Id = id });

    return res;
});

app.Run();
public class ArticleValidator : AbstractValidator<Article>
{
    public ArticleValidator()
    {
        RuleFor(v => v.Title).NotEmpty().WithMessage("Title is required.").Must(UpperCase).WithMessage("Must upper case.");
    }

    private bool UpperCase(string state) {
        return state.Where(x => char.IsLower(x)).ToList().Count() <= 0;
    }
};

