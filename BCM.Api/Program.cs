using BCM.Api.Configurations;
using BCM.Api.DataAccess;
using BCM.Api.SignalR;
using Microsoft.EntityFrameworkCore;

namespace BCM.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateSlimBuilder(args);
        builder.Services
            .ConfigureApiRateLimiting()
            .AddOpenApi()
            .AddCors()
            .ConfigureDbContext(builder)
            .ConfigureServices()
            .ConfigureMappers()
            .ConfigureSignalR();
        
        var app = builder.Build();
        app.UseRateLimiter()
            .UseCors();
        app.MapApi();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        }

        app.UseHttpsRedirection();
        app.MapHub<BookHub>("/bookHub");

        await using (var scope = app.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        await app.RunAsync();
    }
}