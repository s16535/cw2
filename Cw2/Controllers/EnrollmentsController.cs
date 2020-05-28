using Microsoft.AspNetCore.Mvc;

using Cw2.Models;
using Cw2.Services;
using Cw2.DTO;
using Microsoft.AspNetCore.Authorization;

namespace cw2.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentDbService _service;

        public EnrollmentsController(IStudentDbService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            var response = _service.EnrollStudent(request);

            return response.Type switch
            {
                "201 Created" => Created(response.Message, response.ResponseObject),
                "400 Bad Request" => BadRequest(response.Message),
            };
        }

        [HttpPost("{promotions}")]
        [Authorize(Roles = "employee")]
        public IActionResult PromoteStudent(PromoteStudentRequest request)
        {
            var response = _service.PromoteStudents(request);

            return response.Type switch
            {
                "201 Created" => Created(response.Message, response.ResponseObject),
                "400 Bad Request" => BadRequest(response.Message),
                "404 Not Found" => NotFound(response.Message),
            };
        }
    }
}