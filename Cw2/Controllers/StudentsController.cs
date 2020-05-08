using System.Data.SqlClient;
using Cw2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Cw2.Services;

namespace Cw2.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentDbService _dbService;

        public StudentsController(IStudentDbService dbService)
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


    }
}