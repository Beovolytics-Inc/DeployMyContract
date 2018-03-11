using System;

namespace DeployMyContract.Core.Logic
{
    public class ContractCompilationException : ApplicationException
    {
        public ContractCompilationException(string message)
            : base(message)
        { }
    }
}
