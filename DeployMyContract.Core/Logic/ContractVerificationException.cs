using System;

namespace DeployMyContract.Core.Logic
{
    public class ContractVerificationException : ApplicationException
    {
        public ContractVerificationException(string message)
            : base(message)
        { }
    }
}
