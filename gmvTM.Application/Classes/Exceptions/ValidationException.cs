using System;

namespace gmvTM.Application.Classes.Exceptions
{
    public sealed class ValidationException : Exception
    {
        public ValidationException(string message) : base(message)
        {
        }
    }
}
