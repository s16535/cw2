using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw2.DAL;
using Cw2.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cw2.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IDbService _dbService;

        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudents(string orderBy)
        {
            return Ok(_dbService.GetStudents());
        }

        [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            if (id == 1)
            {
                return Ok("Kowalski");
            }
            else if (id == 2)
            {
                return Ok("Malewski");
            }
            else if (id == 3)
            {
                return Ok("Andrzejewski");
            }
            return NotFound("Nie znaleziono studenta!");
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id)
        {
            return Ok("Aktualizacja dokończona");
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveStudent(int id)
        {
            return Ok("Usuwanie ukończone");
        }
    }
}
