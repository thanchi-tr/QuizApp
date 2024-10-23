using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using QuizApp.Data;

namespace QuizApp.Configurations
{
    public static class DependancyExtensions
    {
        public static IServiceCollection ConfigureDependancy(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<IdeaSpaceDBContext>(
                options => options.UseSqlServer(
                    configuration.GetConnectionString("ISpaceDbConnectionString")
                )
            );
            services.AddControllers();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddMemoryCache();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(
                opt => {
                    opt.EnableAnnotations(); // enable Swagger Annotation    
                    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Quiz Backend", Version = "v1" });

                });
            return services;
        }
    }
}
