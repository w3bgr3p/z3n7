using System;
using System.Collections.Generic;

using System.Text;
using Nethereum.Signer;
using Nethereum.Web3.Accounts;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7.Wallets
{
    public class WalletConnect : IWallet
    {
        private readonly IZennoPosterProjectModel _project;
        private readonly Instance _instance;
        private readonly Logger _log;
        private readonly string _key;
        private readonly Account _account;

        public string Topic { get; private set; }
        public string Version { get; private set; }
        public long ExpiryTimestamp { get; private set; }
        public string RelayProtocol { get; private set; }
        public string SymKey { get; private set; }
        public string WalletConnectUri { get; private set; }
        public string Address { get; private set; }

        public WalletConnect(IZennoPosterProjectModel project, Instance instance, string wcUri, Logger log = null, string key = null)
        {
            _project = project;
            _instance = instance;
            _log = log;

            _key = KeyLoad(key);
            _account = new Account(_key);
            Address = _account.Address;

            ParseUri(wcUri);
        }

        private string KeyLoad(string key)
        {
            if (string.IsNullOrEmpty(key)) key = "key";

            switch (key)
            {
                case "key":
                    key = _project.DbKey("evm");
                    break;
                case "seed":
                    key = _project.DbKey("seed");
                    break;
                default:
                    return key;
            }

            if (string.IsNullOrEmpty(key))
                _project.warn("keyIsEmpty", thrw: true);

            return key;
        }

        private void ParseUri(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
                throw new ArgumentException("WalletConnect URI cannot be empty");

            if (!uri.StartsWith("wc:", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Invalid WalletConnect URI format. Must start with 'wc:'");

            WalletConnectUri = uri;

            var uriWithoutPrefix = uri.Substring(3);
            var atIndex = uriWithoutPrefix.IndexOf('@');

            if (atIndex == -1)
                throw new ArgumentException("Invalid WalletConnect URI format. Missing '@' separator");

            Topic = uriWithoutPrefix.Substring(0, atIndex);
            var versionAndParams = uriWithoutPrefix.Substring(atIndex + 1);
            var questionIndex = versionAndParams.IndexOf('?');

            if (questionIndex == -1)
            {
                Version = versionAndParams;
                return;
            }

            Version = versionAndParams.Substring(0, questionIndex);
            var queryString = versionAndParams.Substring(questionIndex + 1);
            var parameters = ParseQueryString(queryString);

            foreach (var param in parameters)
            {
                switch (param.Key.ToLower())
                {
                    case "expirytimestamp":
                        if (long.TryParse(param.Value, out long expiry))
                            ExpiryTimestamp = expiry;
                        break;
                    case "relay-protocol":
                        RelayProtocol = param.Value;
                        break;
                    case "symkey":
                        SymKey = param.Value;
                        break;
                }
            }

            _log?.Send($"WalletConnect parsed: Topic={Topic}, Version={Version}, Relay={RelayProtocol}");
        }

        private static Dictionary<string, string> ParseQueryString(string queryString)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(queryString)) return result;

            var pairs = queryString.Split('&');
            foreach (var pair in pairs)
            {
                var keyValue = pair.Split(new[] { '=' }, 2);
                if (keyValue.Length == 2)
                {
                    var key = Uri.UnescapeDataString(keyValue[0]);
                    var value = Uri.UnescapeDataString(keyValue[1]);
                    result[key] = value;
                }
            }
            return result;
        }

        public void Launch()
        {
            _log?.Send($"WalletConnect session initialized for address {Address}");
            Connect();
        }

        public void Unlock()
        {
            _log?.Send("WalletConnect unlock not required - using private key signing");
        }

        public void Connect()
        {
            try
            {
                _log?.Send($"Connecting to WalletConnect session: {Topic}");

                // Navigate to WalletConnect URI in browser
                _instance.ActiveTab.Navigate(WalletConnectUri, "");
                System.Threading.Thread.Sleep(2000);

                _log?.Send("WalletConnect session connected successfully");
            }
            catch (Exception ex)
            {
                _log?.Send($"Failed to connect WalletConnect: {ex.Message}");
                throw;
            }
        }

        public string SignMessage(string message)
        {
            try
            {
                _log?.Send($"Signing message: {message}");

                var signer = new EthereumMessageSigner();
                var signature = signer.EncodeUTF8AndSign(message, new EthECKey(_key));

                _log?.Send($"Message signed: {signature}");
                return signature;
            }
            catch (Exception ex)
            {
                _log?.Send($"Failed to sign message: {ex.Message}");
                throw;
            }
        }

        public string SignTypedData(string typedData)
        {
            try
            {
                _log?.Send($"Signing typed data");

                var signer = new EthereumMessageSigner();
                var signature = signer.HashAndSign(typedData, _key);

                _log?.Send($"Typed data signed: {signature}");
                return signature;
            }
            catch (Exception ex)
            {
                _log?.Send($"Failed to sign typed data: {ex.Message}");
                throw;
            }
        }

        public string PersonalSign(string message)
        {
            try
            {
                _log?.Send($"Personal sign: {message}");

                var signer = new EthereumMessageSigner();
                var signature = signer.Sign(Encoding.UTF8.GetBytes(message), new EthECKey(_key));

                _log?.Send($"Personal sign completed: {signature}");
                return signature;
            }
            catch (Exception ex)
            {
                _log?.Send($"Failed to personal sign: {ex.Message}");
                throw;
            }
        }

        public string SendTransaction(string rpc, string to, string data, string value = "0", int chainId = 1, string proxy = "", bool useNetHttp = false)
        {
            try
            {
                _log?.Send($"Sending transaction via WalletConnect to {to}");

                var tx = new z3n7.Web3.Tx(_project, useNetHttp, logger: _log);
                var hash = tx.SendTx(
                    chainRpc: rpc,
                    contractAddress: to,
                    encodedData: data,
                    value: value,
                    walletKey: _key,
                    txType: 2,
                    speedup: 1,
                    debug: false,
                    proxy: proxy
                );

                _log?.Send($"Transaction sent: {hash}");
                return hash;
            }
            catch (Exception ex)
            {
                _log?.Send($"Failed to send transaction: {ex.Message}");
                throw;
            }
        }

        public bool IsExpired()
        {
            if (ExpiryTimestamp == 0) return false;
            var expiryDate = DateTimeOffset.FromUnixTimeSeconds(ExpiryTimestamp).DateTime;
            return DateTime.UtcNow > expiryDate;
        }

        public TimeSpan GetTimeUntilExpiry()
        {
            if (ExpiryTimestamp == 0) return TimeSpan.MaxValue;
            var expiryDate = DateTimeOffset.FromUnixTimeSeconds(ExpiryTimestamp).DateTime;
            var timeLeft = expiryDate - DateTime.UtcNow;
            return timeLeft > TimeSpan.Zero ? timeLeft : TimeSpan.Zero;
        }

        public override string ToString()
        {
            return $"WalletConnect [Address={Address}, Topic={Topic}, Version={Version}, Relay={RelayProtocol}]";
        }
    }
}
