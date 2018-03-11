using System;

namespace DeployMyContract.Core.Logic
{
    public class ArgumentParsingException : ApplicationException
    {
        public ArgumentParsingException(string name, string value, string message) 
            : base($"Argument '{name}' = '{value}': {message}")
        { }
    }
}
