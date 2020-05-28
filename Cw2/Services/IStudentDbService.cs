using Cw2.DTO;
using Cw2.DTO.Requests;
using Cw2.Models;
using System;
using System.Collections.Generic;

namespace Cw2.Services
{
    public interface IStudentDbService
    {
        Response EnrollStudent(EnrollStudentRequest request);
        Response PromoteStudents(PromoteStudentRequest request);
        Response StudentExists(string indexNr);
        Response CheckCredentials(LoginRequest loginRequest);
        bool IsTokenAuth(string token);
        void SaveToken(SaveTokenRequest request);
        void SaveToken(string previousToken, string token);

        //List<Student> GetStudents();
    }
}
