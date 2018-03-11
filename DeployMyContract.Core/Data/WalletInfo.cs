using System;
using DeployMyContract.Core.Helpers;
using Nethereum.KeyStore;
using Nethereum.Signer;

namespace DeployMyContract.Core.Data
{
    public class WalletInfo
    {
        public string Address { get; }
        public string PrivateKey { get; }

        public WalletInfo(string keyStoreJson, string password)
        {
            if (keyStoreJson == null)
                throw new ArgumentNullException(nameof(keyStoreJson));
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            PrivateKey = HexHelper.ToHex(new KeyStoreService()
                .DecryptKeyStoreFromJson(password, keyStoreJson), true);
            Address = EthECKey.GetPublicAddress(PrivateKey);
        }
    }
}
