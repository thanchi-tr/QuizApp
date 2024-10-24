
using QuizApp.Configurations;

namespace QuizApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            // Override what specify in the properties/launchSettings.json
            builder.WebHost.ConfigureKestrel(options =>
            {
                // indicate which port we want to preserve for api
                //options.ListenAnyIP(5000); // use http only
                options.ListenAnyIP(5001, listenOption => // use https only
                {
                    listenOption.UseHttps();
                });

            });
            builder.Services.ConfigureDependancy(builder.Configuration);
            builder.Services.ConfigureCors();
            builder.Services.ConfigureService();
            builder.Services.ConfigureStrategy();
            builder.Services.ConfigureAuth(builder.Configuration);

            // DB Context


            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            // Health Check Endpoint
            app.MapGet("/health", () => Results.Ok("Service is healthy"));
            app.Run();
        }
    }
}
