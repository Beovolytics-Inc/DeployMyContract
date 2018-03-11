using System.IO;
using DeployMyContract.Core.Data;

namespace DeployMyContract.Core.Contracts
{
    public interface IContractSourceParser
    {
        ContractSource Parse(string source, DirectoryInfo currentDirectory);
    }
}
