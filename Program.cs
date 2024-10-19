
using Microsoft.EntityFrameworkCore;
using QuizApp.Data;
using QuizApp.Model.DTO;
using QuizApp.Model.DTO.External;
using QuizApp.Services.Cache;
using QuizApp.Services.ConcreteStrategies.MultipleChoice;
using QuizApp.Services.ConcreteStrategies.MultipleChoice.Model.DTO;
using QuizApp.Services.CRUD;
using QuizApp.Services.Operation.Provider;
using QuizApp.Services.Operation.Validator;

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
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
            });
            // DB Context
            builder.Services.AddDbContext<IdeaSpaceDBContext>(
                options => options.UseSqlServer(
                    builder.Configuration.GetConnectionString("ISpaceDbConnectionString")
                )
            );
            builder.Services.AddControllers();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddMemoryCache();
            // add servive to DI
            builder.Services.AddTransient<IExtractStrategy<Model.Domain.Question>,ExtractMultipleChoiceTestStrategy>(); 
            builder.Services.AddTransient<IExtractStrategy<Model.Domain.Answer>, ExtractMultipleChoiceAnswerStrategy>();
            builder.Services.AddTransient<IValidateService, ValidatorServcie>();
            builder.Services.AddScoped<InformationProvider<TestDTO, Model.Domain.Question>, TestProvider>();
            builder.Services.AddScoped<InformationProvider<Model.DTO.AnswersDTO, Model.Domain.Answer>, AnswerProvider>();
            builder.Services.AddSingleton<IInformationCache<AnswersDTO>, AnswerCache>();

            // add validating strategy
            builder.Services.AddScoped<IValidatingStrategy<MultipleChoiceAnswerDTO>, ValidateMultipleChoiceStrategy>();
            builder.Services.AddScoped<ValidateMultipleChoiceStrategy>();

            builder.Services.AddScoped<ICRUDService<CollectionDTO>, CollectionService>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(
                c => c.EnableAnnotations() // enable Swagger Annotation    
            );


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseCors("AllowAllOrigins");
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
