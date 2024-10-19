using QuizApp.Data;
using QuizApp.Services.ConcreteStrategies;

namespace QuizApp.Services.ConcreteStrategies.MultipleChoice
{
    public class MutipleChoice :
        Singleton<MutipleChoice>,
        IFormatDeclare
    {


        public List<(string, string)> Declare() =>
            new List<(string, string)> {
                (@"""MultipleChoiceQuestion""",
                    @"{""Question"":""Id"", ""Options"": [/*4 option*/ ]}"),
                (@"""MultipleChoiceAnswer""", // just for now: the front end does not need to know about the AnswerDTO
                    @"{""QuestionId"":""Id"", ""Answer"": ""Correct Answer""}"),
            };

    }
}
