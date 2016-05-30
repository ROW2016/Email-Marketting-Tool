using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMT_WebApp.Helpers
{
    public class SenderExceptionBase : Exception
    {
        public SenderExceptionBase(string message) : base(message)
        {
        }
        public SenderExceptionBase(string message, Exception exception) : base(message, exception)
        {
        }
    }

    public class SenderCreationException : SenderExceptionBase
    {
        public SenderCreationException(string message) : base(message)
        { }
    }

    public class SenderInvocationException : SenderExceptionBase
    {
        public SenderInvocationException(string message) : base(message) { }

        public SenderInvocationException(string message, Exception exception) : base(message, exception) { }
    }

    public class NoParameterException : SenderExceptionBase
    {
        public NoParameterException() : base("Not Enough Supplied Parameters") { }
    }

}