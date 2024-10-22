
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuizApp.Data;
using QuizApp.Model.Domain;
using QuizApp.Model.DTO;
using QuizApp.Model.DTO.External;
using QuizApp.Services.Authentication;
using QuizApp.Services.Authentication.Token;
using QuizApp.Services.Authentication.Util;
using QuizApp.Services.Cache;
using QuizApp.Services.ConcreteStrategies.MultipleChoice;
using QuizApp.Services.ConcreteStrategies.MultipleChoice.Model.DTO;
using QuizApp.Services.CRUD;
using QuizApp.Services.Operation.Provider;
using QuizApp.Services.Operation.Validator;
using System.Text;

namespace QuizApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var jwtSettings = builder.Configuration.GetSection("JwtSettings"); // access the secret-store @jwtsetting. @todo: move to secret vault for better security during production
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

            var secretKey = builder.Configuration["JwtSettings:Secret"];
            if (secretKey == null) // @todo: use OpenSSL to generate a new secret
                return;
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true, // Validates the 'iss' claim
                        ValidateAudience = true, // Validates the 'aud' claim
                        ValidateLifetime = true, // Validates the 'exp' and 'nbf' claims
                        ValidateIssuerSigningKey = true, // Validates the token's signature
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), // chose symmetric security: (1) simplicity + performance (2) we dont need to share the key so the risk is minimal
                        ClockSkew = TimeSpan.Zero // Removes default 5-minute clock skew
                    };
                });

            //builder.Services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowAllOrigins", policy =>
            //        policy.WithOrigins("https://localhost:5001/*") // indicate that we only interact with the policy with URL
            //              .AllowAnyMethod()
            //              .AllowAnyHeader());
            //});
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

            // Register Auth services:
            builder.Services.AddTransient<IAuthService, UserService>();
            builder.Services.AddTransient<ITokenService, TokenService>();
            builder.Services.AddScoped<IValidate<String>, ConcretePasswordStrengthValidator> ( );
            builder.Services.AddScoped<IValidate<User>, ConcreteUserNameValidator>();
            builder.Services.AddScoped<IPasswordHash, BcryptHashPasswordAdapter>();
            builder.Services.AddScoped<IPasswordVerificate, BcryptHashPasswordAdapter>();
            
            builder.Services.AddScoped<ICRUDService<CollectionDTO>, CollectionService>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(
                opt => {
                    opt.EnableAnnotations(); // enable Swagger Annotation    
                    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Quiz Backend", Version = "v1" });
                    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                        {
                            In = ParameterLocation.Header,
                            Description = "Please enter token",
                            Name = "Authorization",
                            Type = SecuritySchemeType.Http,
                            BearerFormat = "JWT",
                            Scheme = "bearer"
                        }
                    );
                    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type=ReferenceType.SecurityScheme,
                                    Id="Bearer"
                                }
                            },
                            new string[]{}
                        }
                    });

                });

            
            var app = builder.Build();
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
