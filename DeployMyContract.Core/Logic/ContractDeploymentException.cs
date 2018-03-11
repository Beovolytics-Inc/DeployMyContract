using System;

namespace DeployMyContract.Core.Logic
{
    public class ContractDeploymentException : ApplicationException
    {
        public ContractDeploymentException(string message)
            : base(message)
        { }
    }
}