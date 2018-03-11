using System;

namespace DeployMyContract.Core.Logic
{
    public class ContractParsingException : ApplicationException
    {
        public ContractParsingException(string message) 
            : base(message)
        { }
    }
}
