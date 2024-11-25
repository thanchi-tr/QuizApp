﻿using Microsoft.AspNetCore.Mvc;
using QuizApp.Model.Domain;
using QuizApp.Model.DTO;
using QuizApp.Model.DTO.External;
using QuizApp.Model.DTO.External.Resquest;
using QuizApp.Model.DTO.Internal;
using QuizApp.Services.Operation.Provider;
using QuizApp.Services.Operation.Validator;
using Swashbuckle.AspNetCore.Annotations;

namespace QuizApp.Controllers
{
    [ApiController]
    [Route("Api/Validation")]
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


        /// <summary>
        /// @todo: the method need to be revised in order to avoid nested json (since i already serialized parent json 
        /// in the payload. WHY: to avoid complication later on
        /// </summary>
        /// <param name="answer"></param>
        /// <returns></returns>
        [HttpPost("Collections/{CollectionId}/Questions/{QuestionId}/Answer")]
        [SwaggerOperation(Summary = "Compare the attempt with validated answer!",
            Description = "Compare the attempt according to its type.",
            Tags = new[] { "Validate", "Player" })
        ]
        [SwaggerResponse(
            200,
            "Ok :: Successfully complete the comparison, compute result.", 
            typeof(ResponseValidatePayload),
            ContentTypes = new[] { "application/json" })]
        [SwaggerResponse(
            400,
            "Bad Request:: Mising Url param.(client side error)", 
            ContentTypes = new[] { "application/json" },
            Type = typeof(string[])   
        )]
        [SwaggerResponse(
            404,
            "Not Found:: the [\"Collection\" and/or \"Question\"] does not exist",
            ContentTypes = new[] { "application/json" },
            Type = typeof(string[]) // notify client which resource is missing

        )]
        public IActionResult ValidateAnswer(
            string CollectionId,
            string QuestionId,
            [FromBody]
            [SwaggerParameter(
                Required=true, 
                Description = "Expected Payload:Serialized JSON object with <CollectionId>, <Type>, <an attempt>")]
                 SerializedAttemptDTO abstractAttempt)
        {
            /**/
            BusinessToPresentationLayerDTO<ResponseValidatePayload> result =  _validateService.Validate(abstractAttempt, CollectionId, QuestionId);

            return (result.Status) ? Ok(result.Data) :
                    (result.Message.Contains("Not found") ? NotFound() :
                    BadRequest());
        }   
    }
    

}
