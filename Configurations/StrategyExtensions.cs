using QuizApp.Services.ConcreteStrategies.MultipleChoice;
using QuizApp.Services.ConcreteStrategies.MultipleChoice.Model.DTO;
using QuizApp.Services.Operation.Provider;
using QuizApp.Services.Operation.Validator;

namespace QuizApp.Configurations
{
    public static class StategyExtensions
    {
        public static IServiceCollection ConfigureStrategy(this IServiceCollection services)
        {
            services.AddTransient<IExtractStrategy<Model.Domain.Question>, ExtractMultipleChoiceTestStrategy>();
            services.AddTransient<IExtractStrategy<Model.Domain.Answer>, ExtractMultipleChoiceAnswerStrategy>();
            services.AddScoped<IValidatingStrategy<MultipleChoiceAnswerDTO>, ValidateMultipleChoiceStrategy>();
            services.AddScoped<ValidateMultipleChoiceStrategy>();

            return services;
        }
    }
}
