using Microsoft.AspNetCore.Mvc;
using QuizApp.Model.DTO;
using QuizApp.Services.CRUD;
using Swashbuckle.AspNetCore.Annotations;

namespace QuizApp.Controllers
{
    [ApiController]
    [Route("Api/crud")]
    public class CrudController : ControllerBase
    {
        public CrudController(ICRUDService<CollectionDTO> crudService)
        {
            _crudService = crudService;
        }

        private ICRUDService<CollectionDTO> _crudService { get; set; }

        [HttpGet("Collections/GetAll")]
        [SwaggerOperation(Summary = "Get quiz ID and tag !",
            Description = "the quiz only have the ID and name")]
        [SwaggerResponse(200, "<Quiz/CollectionDTO> will be return  /n"
            , typeof(CollectionDTO))]
        [SwaggerResponse(404)]
        public IActionResult GetAllCollections()
        {
            return Ok(_crudService.GetAll());
        }

        [HttpPut("Collections/question/edit/{questionId}")]
        [SwaggerOperation(Summary = "Edit the  question data")]
        public IActionResult EditQuestion([FromBody] string questionDetail)
        {
            /**
             * 
             * 
             */
            return Ok();
        }

        [HttpPost("Collections/question/add")]
        [SwaggerOperation(Summary = "Add the question to the exist data base")]
        
    }
}
