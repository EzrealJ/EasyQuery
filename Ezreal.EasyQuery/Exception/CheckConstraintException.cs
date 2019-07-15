using System;
using System.Runtime.Serialization;

namespace Ezreal.EasyQuery.Exception
{
    public class CheckConstraintException : System.Exception
    {
        public CheckConstraintException()
        {
        }

        public CheckConstraintException(string message) : base(message)
        {
        }

        public CheckConstraintException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        protected CheckConstraintException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
