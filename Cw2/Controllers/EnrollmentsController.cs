using Microsoft.AspNetCore.Mvc;

using Cw2.Models;
using Cw2.Services;
using Cw2.DTO;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;

namespace cw2.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly s16535Context Db_context;

        public EnrollmentsController(s16535Context context)
        {
            Db_context = context;
        }

        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            var studies = Db_context.Studies.Single(s => s.Name == request.Studies);

            if (studies == null)
            {
                return NotFound("Studia " + request.Studies + " nie istnieja");
            }

            var enrollment = Db_context.Enrollment.Single(e => e.IdStudy == studies.IdStudy && e.Semester == 1);

            if (enrollment == null)
            {
                enrollment = new Enrollment
                {
                    Semester = 1,
                    IdStudy = studies.IdStudy,
                    StartDate = DateTime.Now
                };

                Db_context.Enrollment.Add(enrollment);
                Db_context.SaveChanges();
            }

            if (Db_context.Student.Any(s => s.IndexNumber == request.IndexNumber))
            {
                return BadRequest("Id " + request.IndexNumber + " jest przypisane do innego studenta!");
            }

            var student = new Student
            {
                IndexNumber = request.IndexNumber,
                FirstName = request.FirstName,
                LastName = request.LastName,
                BirthDate = request.BirthDate,
                IdEnrollment = enrollment.IdEnrollment
            };

            Db_context.Student.Add(student);
            Db_context.SaveChanges();

            return Ok(student);
        }

        [HttpPost("{promotions}")]
        public IActionResult PromoteStudents(PromoteStudentRequest request)
        {
            var studies = Db_context.Studies.Single(s => s.Name == request.Studies);

            if (studies == null)
            {
                return NotFound("Studia " + request.Studies + " nie istnieja");
            }

            var enrollment = Db_context.Enrollment.Where(e => e.Semester == request.Semester && e.IdStudy == studies.IdStudy).ToList();

            if (enrollment.Count() == 0)
            {
                return NotFound("Wpis na " + request.Semester + " nie istnieje");
            }

            var newEnrollment = new Enrollment();
            {
                newEnrollment.IdEnrollment = Db_context.Enrollment.Max(e => e.IdEnrollment) + 1;
                newEnrollment.Semester = enrollment.First().Semester + 1;
                newEnrollment.IdStudy = enrollment.First().IdStudy;
                newEnrollment.StartDate = DateTime.Now;
                Db_context.Enrollment.Add(newEnrollment);
            }

            Db_context.Student.Where(s => s.IdEnrollment == enrollment.First().IdEnrollment)
                            .ToList()
                            .ForEach(s => s.IdEnrollment = newEnrollment.IdEnrollment);

            Db_context.SaveChanges();
            return Ok("Promocja zakończona sukcesem!");
        }
    }

        /*        [HttpPost]
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
                }*/
}