using BCMS.Api.Configurations;
using BCMS.Api.DataAccess;
using BCMS.Api.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BCMS.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);
        builder.Services
            .ConfigureApiRateLimiting()
            .AddOpenApi()
            .AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins("http://localhost").AllowAnyMethod().AllowAnyHeader()))
            .ConfigureDbContext(builder)
            .ConfigureServices()
            .ConfigureMappers()
            .ConfigureSignalR();
        
        var app = builder.Build();
        app.UseRateLimiter().UseCors();
        app.MapApi();
        
        // Easier use of OpenAPI/Swagger (also in Docker)
        app.MapOpenApi(); 
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
        
        if (app.Environment.IsDevelopment())
        {
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        }

        app.UseHttpsRedirection();
        app.MapHub<BooksHub>("/booksHub");

        await using (var scope = app.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        await app.RunAsync();
    }
}