using Microsoft.AspNetCore.Mvc;
using QuizApp.Model.DTO;
using QuizApp.Services.CRUD;
using Swashbuckle.AspNetCore.Annotations;

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
        [SwaggerOperation(Summary = "Get quiz ID and tag !",
            Description = "the quiz only have the ID and name")]
        [SwaggerResponse(200, "<Quiz/CollectionDTO> will be return  /n"
            , typeof(CollectionDTO))]
        [SwaggerResponse(404)]
        public IActionResult GetAllCollections()
        {
            return Ok(_crudService.GetAll());
        }

        [HttpPut("Collections/{CollectionID}/questions/{QuestionId}/Edit")]
        [SwaggerOperation(Summary = "Edit the  question data")]
        public IActionResult EditQuestion([FromBody] string questionDetail)
        {
            /**
             * 
             * 
             */
            return Ok();
        }

        [HttpPost("Collections/{collectionId}/Questions/Add")]
        [SwaggerOperation(Summary = "Add the question to the exist data base")]
        public IActionResult CreateQuestion([FromBody] string question, string collectionID)
        {
            if(string.IsNullOrEmpty(question))
            {
                return BadRequest();
            }
            //delegate the task to the service
            var result =  _crudService.CreateQuestion(question, collectionID);
            return Ok();
        }
        
    }
}
