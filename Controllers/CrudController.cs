using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QuizApp.Model.Domain;
using QuizApp.Model.DTO;
using QuizApp.Services.CRUD;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;

namespace QuizApp.Controllers
{
    [ApiController]
    [Route("Api/Collections")]
    public class CrudController : ControllerBase
    {
        public CrudController(ICRUDService<CollectionDTO> crudService)
        {
            _crudService = crudService;
        }

        private ICRUDService<CollectionDTO> _crudService { get; set; }

        [HttpGet("")]
        [SwaggerOperation(Summary = "return the HttpPacket with body contain serialized CollectionDTO:{ CollectionId: Guid, Name: String }!",
            Description = "the Quiz only have the ID and name,; @todo: when introduce the edit functionality, make sure checking the userId matching")]
        [SwaggerResponse(200, "<Quiz/CollectionDTO> will be return  /n"
            , typeof(CollectionDTO))]
        [SwaggerResponse(404)]
        public async Task<IActionResult> GetAllCollections()
        {
            return Ok(await _crudService.GetAllCollectionAsync());
        }


        [HttpPut("Collection/Questions/{QuestionId}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerOperation(
            Summary = "Edit Detail of Question with ID provided",
            Description = "Take payload of Serialized Json apply it to the exist entry in SQL DB",
            Tags = new[] { "Edit", "Crud", "Admin" }
            )]
        
        [SwaggerResponse(
            401,
            "Unauthorized Request<@Todo@21-10-24>:: the Collection is not accessible by current user")]
        [SwaggerResponse(
            400,
            "Bad Request:: Missing key property. Return a list of {\"SerializedQuestionAnswerDTO\" || \"QuestionId\" || \"FormatError\"} depend on which is missing <or> SerializedQuestionAnswerDTO is of incorrect format.",
            ContentTypes = new[] { "application/json" },
            Type = typeof(string[]))
          ]
        [SwaggerResponse(
            404,
            "Not Found:: The Question does not exist in the DataBase.",
            Type = typeof(void))
          ]
        [SwaggerResponse(204, "No Content:: The successfully updating the question without any <Error>")]
        public async Task<IActionResult> EditQuestion(
            [FromBody] [SwaggerRequestBody(
            Required = true,
            Description = "Expected Payload: Json.Serialized(MultipleChoiceQuestionAnswerDTO) <-> string value of {\"Question\":string, \"Type\": string, \"Correct\": string, \"Incorrect\": string[]}"
        )] string SerializedQuestionAnswerDTO,
            string QuestionId)
        {
            
            //delegate the task to the service
            var result = await _crudService.EditQuestion(SerializedQuestionAnswerDTO, QuestionId);
            if (result.Contains("Not found"))
            {
                return NotFound();
            }
            if (result.Length > 0)
            {
                return BadRequest();
            }
            return NoContent();
        }


        [HttpPost("{CollectionId}/Questions/Question")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerOperation(
            Summary = "Add question to an existed Collection",
            Description = "Take payload of Serialized MultipleChoiceQuestionAnswerDTO{\"Question\":string, \"Type\": string, \"Correct\": string, \"Incorrect\": string[]}"
        )]
        public IActionResult CreateQuestion([FromBody] string question, string CollectionId)
        {
            if(string.IsNullOrEmpty(question))
            {
                return BadRequest();
            }
            //delegate the task to the service
            var result =  _crudService.CreateQuestion(question, CollectionId);
            return Ok();
        }

        [HttpPost("Collection")]
        [SwaggerOperation(Summary ="Take payload that contain Name:string in the body")]
        public async Task<IActionResult> CreateCollection([FromBody] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return await Task.FromResult(BadRequest("Collection name cannot be empty."));
            }

            try
            {
                // Simulate some async logic for creating the collection.
                var status = await _crudService.CreateCollection(name);

                // Return 'Created' with the URI to the new collection resource.
                //return CreatedAtAction(nameof(GetCollection), new { id = collectionId }, new { id = collectionId, name = name });
                return (status) ? Ok() : BadRequest();
            }
            catch (Exception ex)
            {
                // Log the exception and return a 500 error.
                // In a real scenario, use proper logging mechanisms.
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while creating the collection.");
            }
        }


        [HttpDelete("/{CollectionId}/Questions/{QuestionId}")]
        [SwaggerOperation(Summary = "Remove question from the collection")]
        public async Task<bool> DeleteQuestion(string CollectionId, string QuestionId)
        {
            bool result = await _crudService.DeleteQuestion(CollectionId, QuestionId);

            return result;
        }

    }
}
