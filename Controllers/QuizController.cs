using Microsoft.AspNetCore.Mvc;
using QuizApp.Model.DTO.External;
using QuizApp.Services.ConcreteStrategies;
using QuizApp.Services.Operation.Provider;
using Swashbuckle.AspNetCore.Annotations;

namespace QuizApp.Controllers
{
    /// <summary>
    /// Handler the pathing for <only> request Quiz</only>
    /// </summary>
    [ApiController]
    [Route("Api/Collections")]
    public class QuizController : ControllerBase
    {
        /// <summary>
        /// Contructor Injection  
        /// </summary>
        private InformationProvider<TestDTO, Model.Domain.Question> _infoProvider;
        public QuizController(InformationProvider<TestDTO, Model.Domain.Question> infoProvider) => _infoProvider = infoProvider;

        [HttpGet("{Id}/Quiz")]
        [SwaggerOperation(Summary = "Get quiz !",
            Description = "The quiz will include (0..n) question of multiple type ")]
        [SwaggerResponse(
            200,
            "<Questions> is configured into a format that suit for question type /n",
            ContentTypes = new[] { "application/json" },
            Type = typeof(TestDTO))]
        [SwaggerResponse(
            400,
            "Bad Request:: the serializedCollectionId is empty"
        )]
        public IActionResult GetQuiz([SwaggerParameter(Description = "Serialized GUID of the Quiz.")] String Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return BadRequest("ID cannot be null or empty");
            }
            var packet = _infoProvider.Get(Id);
            return Ok(packet);
        }


        [HttpGet("Questions/Answer/Format")]
        [SwaggerOperation(Summary = "Get the supported question format",
            Description = "The quiz will include (0..n) question of multiple type ")]
        public IActionResult GetFormats()
        {
            return Ok(FormatDeclarer.Instance.GetRegisteredFormats());
        }
    }
}
