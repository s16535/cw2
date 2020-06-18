using Cw2.Models;
using Microsoft.AspNetCore.Mvc;
using Cw2.DTO.Requests;
using System.Linq;

namespace Cw2.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly s16535Context Db_context;

        public StudentsController(s16535Context context)
        {
            Db_context = context;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            return Ok(Db_context.Student.ToList());
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(string id, UpdateStudentRequest request)
        {
            var s = Db_context.Student.Single(s => s.IndexNumber == id);

            if (s == null)
            {
                return NotFound("Student o nr " + id + " nie został odnaleziony");
            }

            else
            {
                s.FirstName = request.FirstName;
                s.LastName = request.LastName;
                s.BirthDate = request.BirthDate;
                s.IdEnrollment = request.IdEnrollment;

                Db_context.SaveChanges();
                return Ok(s);
            }

        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(string id)
        {
            var s = Db_context.Student.Single(s => s.IndexNumber == id);


            if (s == null)
            {
                return NotFound("Student o nr " + id + " nie został odnaleziony");
            }

            else
            {
                Db_context.Attach(s);
                Db_context.Student.Remove(s);
                Db_context.SaveChanges();

                return Ok("Usuwanie ukończone");
            }
        }
    }
}
