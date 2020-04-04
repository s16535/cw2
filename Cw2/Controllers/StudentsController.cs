using System;
using System.Data.SqlClient;
using Cw2.DAL;
using Cw2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Text;

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
            List<Student> studentsList = new List<Student>();

            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s16535;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select * from Student s join Enrollment e on s.IdEnrollment = e.IdEnrollment join Studies st on e.IdStudy = st.IdStudy";
                con.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student
                    {
                        IndexNumber = dr["IndexNumber"].ToString(),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        BirthDate = dr["BirthDate"].ToString(),
                        StudiesName = dr["Name"].ToString(),
                        SemesterNo = dr["Semester"].ToString()
                    };
                    studentsList.Add(st);
                }
            }
            return Ok(studentsList);
        }

        [HttpGet("{id}")]
        public IActionResult GetStudentEnrollment(string id)
        {
            StringBuilder result = new StringBuilder();

            using (var con = new SqlConnection("Data Source=db-mssql;Initial Catalog=s16535;Integrated Security=True"))
            using (var com = new SqlCommand())
            {
                com.Connection = con;

                // com.CommandText = "select * from Enrollment e where e.IdEnrollment = (select IdEnrollment from Student s where s.IndexNumber = '" + id + "')";
                // SQL INJECTION ATTACK PARAM: s16535'); drop table Student; select * from Enrollment where IdStudy = (select IdStudy from Studies where IdStudy = '1 

                com.CommandText = "select * from Enrollment e where e.IdEnrollment = (select IdEnrollment from Student s where s.IndexNumber = @id)";
                com.Parameters.AddWithValue("id", id);
                con.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    result.Append("Student nr: ").Append(id).Append(", semestr nr: ").Append(dr["Semester"].ToString()).Append(", data rozpoczęcia: ")
                        .Append(dr["StartDate"].ToString());
                }
            }
            return Ok(result.ToString());
        }

        //[HttpGet("{id}")]
        //public IActionResult GetStudent(int id)
        //{
        //    if (id == 1)
        //    {
        //        return Ok("Kowalski");
        //    }
        //    else if (id == 2)
        //    {
        //        return Ok("Malewski");
        //    }
        //    else if (id == 3)
        //    {
        //        return Ok("Andrzejewski");
        //    }
        //    return NotFound("Nie znaleziono studenta!");
        //}

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
