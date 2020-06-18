using System;
using System.ComponentModel.DataAnnotations;

namespace Cw2.DTO.Requests
{
    public class UpdateStudentRequest
    {
        [Required]
        public string IndexNumber { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public int IdEnrollment { get; set; }

    }
}
