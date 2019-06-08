using System;

namespace PowerShellClient
{
    public class PowerShellException : Exception
    {
        public PowerShellException(string message) : base(message) { }
    }
}
