using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using QuizApp.Model.Domain;
using QuizApp.Model.DTO;
using QuizApp.Model.DTO.External.Resquest;
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
        [SwaggerOperation(Summary = "Return all the collections!",
            Description = "the Quiz only have the ID and name,; @todo: when introduce the edit functionality, make sure checking the userId matching")]
        [SwaggerResponse(200, "<Quiz/CollectionDTO> will be return  /n"
            , typeof(CollectionDTO))]
        [SwaggerResponse(
            404,
            "Not found:: @todo when add user indicate that the user does not exist"
        )]
        public async Task<IActionResult> GetAllCollections()
        {
            return Ok(
                (await _crudService.GetAllCollectionAsync()).Data // there will only be one type of data available (and probably server down error)
            );
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
        )] string SerializedQuestionAnswerDTO, // allow for reuse the Route and let the controller inject the parsing strategy at run time
            string QuestionId)
        {
            
            //delegate the task to the service
            var result = await _crudService.EditQuestion(SerializedQuestionAnswerDTO, QuestionId);
            return
                (result.Status) ? NoContent() :
                (result.Message.Contains("Not found")) ? NotFound() :
                BadRequest(result.Data);
            
        }


        [HttpPost("{CollectionId}/Questions/Question")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerOperation(
            Summary = "Add question to an existed Collection",
            Description = "Take payload of Serialized MultipleChoiceQuestionAnswerDTO{\"Question\":string, \"Type\": string, \"Correct\": string, \"Incorrect\": string[]}"
        )]
        [SwaggerResponse(
            401,
            "Unauthorized Request<@Todo@21-10-24>:: the Collection is not accessible by current user"
        )]
        [SwaggerResponse(
            400,
            "Bad Request:: Missing key property",
            ContentTypes = new[] { "application/json" },
            Type = typeof(string[])

        )]
        [SwaggerResponse(
            404,
            "Not found:: the Collection does not exist"
        )]
        [SwaggerResponse(
            201,
            "Successfully create the question and correspond answer"
        )]
        public async Task<IActionResult> CreateQuestion([FromBody] QuestionAnswerDTO questionWithAnswer, string CollectionId)
        {
           
            //delegate the task to the service
            var result =  await _crudService.CreateQuestion(questionWithAnswer, CollectionId);
            return
                (result.Status) ? Created() :
                (result.Message.Equals("Not found")) ? NotFound() :
                BadRequest(result.Data);

        }


        [HttpPost("Collection")]
        [SwaggerOperation(Summary ="Take payload that contain Name:string in the body")]
        [SwaggerResponse(
            401,
            "Unauthorized Request<@Todo@21-10-24>:: the Collection is not accessible by current user"
        )]
        [SwaggerResponse(
            400,
            "Bad Request:: Missing key property name <- it is empty string",
            ContentTypes = new[] { "application/json" },
            Type = typeof(string[])

        )]
        [SwaggerResponse(
            201,
            "Created:: Collection has been successfully create without any error"
        )]
        public async Task<IActionResult> CreateCollection([FromBody] string name)
        {
            var result = await _crudService.CreateCollection(name);
            return
                (result.Status) ? Created() :
                BadRequest(result.Data);
        }


        [HttpDelete("/Collection/Questions/{QuestionId}")]
        [SwaggerOperation(Summary = "Remove question from the collection")]
        [SwaggerResponse(
            401,
            "Unauthorized:: the user does not have the policy @todo"
        )]
        [SwaggerResponse(
            400,
            "Bad Request:: Missing key prop",
            ContentTypes = new[] { "application/json" },
            Type = typeof(string[])
        )]
        [SwaggerResponse(
            404,
            "Not found:: The Question does not exist"
        )]
        [SwaggerResponse(
            204,
            "No Content:: The operation was successfull."
        )]
        public async Task<IActionResult> DeleteQuestion( string QuestionId)
        {
            var result = await _crudService.DeleteQuestion( QuestionId);

            return
                (result.Status) ? NoContent() :
                (result.Message.Contains("Not found")) ? NotFound() :
                BadRequest(result.Data);
        }

    }
}
