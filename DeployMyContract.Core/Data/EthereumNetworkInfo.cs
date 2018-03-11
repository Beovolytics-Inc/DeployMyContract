using System;

namespace DeployMyContract.Core.Data
{
    public class EthereumNetworkInfo
    {
        public string Name { get; set; }
        public Uri Url { get; set; }
        public Uri ApiUrl { get; set; }
        public string VerificationUrl { get; set; }
        public double Fee { get; set; }
        public string FeeTargetAddress { get; set; }
    }
}
