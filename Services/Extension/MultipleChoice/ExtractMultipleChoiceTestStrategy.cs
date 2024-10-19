using QuizApp.Model.Domain;
using QuizApp.Model.DTO;
using QuizApp.Services.ConcreteStrategies.MultipleChoice.Model.Domain;
using QuizApp.Services.Operation.Provider;
using System.Collections.Generic;
using System.Text.Json;

namespace QuizApp.Services.ConcreteStrategies.MultipleChoice
{
    public class ExtractMultipleChoiceTestStrategy : IExtractStrategy<Question>
    {
        public string Extract(Question input)
        {
            // expect a question with answer in side
            var answer = JsonSerializer.Deserialize<DatabaseMultiplechoiceAnswer>(input.Answer.Value);

            if (answer == null)
                return "";

            List<string> Options = new List<string>();
            Options.Add(answer.Correct);
            Options.AddRange(answer.Incorrect);

            
            return "{ \"Question\": \"" + input.Value + "\",\"Id\":\"" + input.QuestionId + "\", \"Options\":" + JsonSerializer.Serialize(Options) + "}";
        }
    }
}
