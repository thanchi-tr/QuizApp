using QuizApp.Model.DTO;
using QuizApp.Model.DTO.External;
using QuizApp.Services.Cache;
using QuizApp.Services.CRUD;
using QuizApp.Services.Operation.Provider;
using QuizApp.Services.Operation.Validator;

namespace QuizApp.Configurations
{
    public static class ServiceExtensions
    {
        public static IServiceCollection ConfigureService(this IServiceCollection services)
        {
            services.AddTransient<IValidateService, ValidatorServcie>();
            services.AddScoped<ICRUDService<CollectionDTO>, CollectionService>();
            services.AddScoped<InformationProvider<Model.DTO.AnswersDTO, Model.Domain.Answer>, AnswerProvider>();
            services.AddScoped<InformationProvider<TestDTO, Model.Domain.Question>, TestProvider>();
            services.AddSingleton<IInformationCache<AnswersDTO>, AnswerCache>();
            return services;
        }
    }
}
