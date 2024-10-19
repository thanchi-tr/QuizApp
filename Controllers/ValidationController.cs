using Microsoft.AspNetCore.Mvc;
using QuizApp.Model.Domain;
using QuizApp.Model.DTO;
using QuizApp.Services.Operation.Provider;
using QuizApp.Services.Operation.Validator;
using Swashbuckle.AspNetCore.Annotations;

namespace QuizApp.Controllers
{
    [ApiController]
    [Route("Api/Validate")]
    public class ValidationController : ControllerBase
    {
        /// <summary>
        /// Construction injection
        /// </summary>
        private InformationProvider<Model.DTO.AnswersDTO, Model.Domain.Answer> _infoProvider;
        private IValidateService _validateService;
        public ValidationController(InformationProvider<AnswersDTO, Answer> infoProvider, IValidateService validateService) {
            _infoProvider = infoProvider;
            _validateService = validateService;
        }

        [HttpGet("Get/{id}")]
        public IActionResult GetQuiz([SwaggerParameter(Description = "Serialized GUID of the Quiz.")] String id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID cannot be null or empty");
            }
            var packet = _infoProvider.Get(id);
            return (packet != null)
                    ? Ok(packet)
                    : NotFound("Quiz not found");
        }


        [HttpPost("Answer")]
        [SwaggerOperation(Summary = "Check answer !",
            Description = "Request is in form of {Type: questionType, CollectionId: id of quiz, Answer: <Serialized string of actual strategy validation class>}")]
        [SwaggerResponse(200, "<Answer> should be read in correspondent with Concrete format for the specific question type /n"
            , typeof(ValidateResultDTO))]
        [SwaggerResponse(404)]
        public IActionResult ValidateAnswer(
            [SwaggerParameter(Description = "Serialized JSON object with <CollectionId>, <Type>, <an attempt>")]
                [FromBody] string answer)
        {
            if (string.IsNullOrEmpty(answer)) {
                return BadRequest();
            }
            ValidateResultDTO result = _validateService.Validate(answer);
            if (string.IsNullOrEmpty(result.QuesitonId)) // encounter error 
            {
                return BadRequest();
            }
            return Ok(result);
        }   
    }
    

}
