using Cw2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw2.DAL
{
    public interface IDbService
    {
        public IEnumerable<Student> GetStudents();
    }
}
