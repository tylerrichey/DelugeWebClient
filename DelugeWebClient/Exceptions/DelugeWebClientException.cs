using System;
using System.Collections.Generic;
using System.Text;

namespace Deluge
{
    public class DelugeWebClientException : Exception
    {
        public int Code { get; set; }

        public DelugeWebClientException(String message, int code) : base(message)
        {
        }

        public DelugeWebClientException() : base()
        {
        }

        public DelugeWebClientException(string message) : base(message)
        {
        }

        public DelugeWebClientException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
