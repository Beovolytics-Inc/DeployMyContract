namespace DeployMyContract.Core.Contracts
{
    public interface IArgumentValueParser
    {
        object Parse(string name, string type, string valueStr);
    }
}
