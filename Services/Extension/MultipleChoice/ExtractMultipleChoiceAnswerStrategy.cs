    using QuizApp.Model.Domain;
using QuizApp.Services.ConcreteStrategies.MultipleChoice.Model.Domain;
using QuizApp.Services.Operation.Provider;
using System.Text.Json;

namespace QuizApp.Services.ConcreteStrategies.MultipleChoice
{
    public class ExtractMultipleChoiceAnswerStrategy : IExtractStrategy<Answer>
    {
        public string Extract(Answer input)
        {
            // expect a question with answer in side
            var answer = JsonSerializer.Deserialize<DatabaseMultiplechoiceAnswer>(input.Value);

            if (answer == null)
                return "";


            return "{ \"QuestionId\":\"" + input.QuestionId + "\", \"Answer\":\"" + answer.Correct + "\"}";

        }
    }
}
