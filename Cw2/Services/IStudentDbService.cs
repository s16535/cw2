using Cw2.DTO;
using Cw2.Models;

namespace Cw2.Services
{
    public interface IStudentDbService
    {
        Response EnrollStudent(EnrollStudentRequest request);
        Response PromoteStudents(PromoteStudentRequest request);
        Response StudentExists(string indexNr);
    }
}
