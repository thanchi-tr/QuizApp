using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuizApp.Model.Domain;
using QuizApp.Services.Authentication.Token;
using QuizApp.Services.Authentication.Util;
using QuizApp.Services.Authentication;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;

namespace QuizApp.Configurations
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection ConfigureAuth(this IServiceCollection services
                                                        , IConfiguration configuration)
        {
            var secretKey = configuration["JwtSettings:Secret"];
            var jwtSettings = configuration.GetSection("JwtSettings");


            if (secretKey == null) // implement logging
                ;// Logging
            services.AddAuthentication(options =>
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
                        ValidateLifetime = true, // Validates the 'exp' and 'nbf' claims:: make sure the token expiration is validated
                        ValidateIssuerSigningKey = true, // Validates the token's signature
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), // chose symmetric security: (1) simplicity + performance (2) we dont need to share the key so the risk is minimal
                        ClockSkew = TimeSpan.Zero, // Removes default 5-minute clock skew

                        // Ensure only HmacSha256 is allowed <- align with our strate @see the Services/Authentication/Token/TokenService.cs::line 40
                        ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }
                    };

                    // Notify the client that their token has been expired
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
            services.AddSwaggerGen(
                opt => {
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
            // Register Auth services:
            services.AddTransient<IAuthService, UserService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddScoped<IValidate<String>, ConcretePasswordStrengthValidator>();
            services.AddScoped<IValidate<User>, ConcreteUserNameValidator>();
            services.AddScoped<IPasswordHash, BcryptHashPasswordAdapter>();
            services.AddScoped<IPasswordVerificate, BcryptHashPasswordAdapter>();

            return services;
        }
    }
}
