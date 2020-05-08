using System;
using System.Threading.Tasks;

namespace Cw2.DTO
{
    public class EnrollStudentResponse 
    {
        public string LastName { get; set; }
        public int Semester { get; set; }
        public DateTime StartDate { get; set; }
    }
}
