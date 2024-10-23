namespace QuizApp.Configurations
{
    public static class CorsExtensions
    {
        public static IServiceCollection ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policy =>
                    policy.AllowAnyOrigin() // @Change this when ready to dev front
                          .AllowAnyMethod()
                          .AllowAnyHeader());
            });

            return services;
        }
    }
}
