using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cw2.DTO.Requests
{
    public class SaveTokenRequest
    {
        [Required]
        public string Token { get; set; }
        public string IndexNumber { get; set; }
    }
}