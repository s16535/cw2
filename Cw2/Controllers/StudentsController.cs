using System.Data.SqlClient;
using Cw2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Cw2.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Cw2.DTO.Requests;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using System;

namespace Cw2.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        //private readonly IStudentDbService _dbService;
        public IConfiguration Configuration { get; set; }
/*        public StudentsController(IStudentDbService dbService)
        {
            _dbService = dbService;
        }*/

        [HttpGet]
        [Authorize]
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

        [HttpPost]
        public IActionResult Login(LoginRequest request)
        {
            var claims = new[]
{
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "jan123"),
                new Claim(ClaimTypes.Role, "admin"),
                new Claim(ClaimTypes.Role, "student")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = Guid.NewGuid()
            });
        }
    }
}