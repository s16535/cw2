using System;

namespace Cw2.DTO.Exceptions
{
    public class StudentCannotDefendException : Exception
    {
        public StudentCannotDefendException(string message) : base(message)
        {
        }

        public StudentCannotDefendException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
