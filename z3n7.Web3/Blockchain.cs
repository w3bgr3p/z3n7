using Nethereum.ABI;
using Nethereum.ABI.ABIDeserialisation;
using Nethereum.ABI.Decoders;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using NBitcoin;

namespace z3nIO.Web3
{
    #region Nethereum
    public class Blockchain
    {
        public static object SyncObject = new object();

        public string walletKey;
        public int chainId;
        public string jsonRpc;

        public Blockchain(string walletKey, int chainId, string jsonRpc)
        {
            this.walletKey = walletKey;
            this.chainId = chainId;
            this.jsonRpc = jsonRpc;
        }

        public Blockchain(string jsonRpc) : this("", 0, jsonRpc)
        { }

        public Blockchain() { }

       public static string GenerateMnemonic(string wordList = "English", int wordCount = 12)
        {
            Wordlist _wordList;
            WordCount _wordCount;

            switch (wordList)
            {
                case "English":
                    _wordList = Wordlist.English;
                    break;

                case "Japanese":
                    _wordList = Wordlist.Japanese;
                    break;

                case "Chinese Simplified":
                    _wordList = Wordlist.ChineseSimplified;
                    break;

                case "Chinese Traditional":
                    _wordList = Wordlist.ChineseTraditional;
                    break;

                case "Spanish":
                    _wordList = Wordlist.Spanish;
                    break;

                case "French":
                    _wordList = Wordlist.French;
                    break;

                case "Portuguese":
                    _wordList = Wordlist.PortugueseBrazil;
                    break;

                case "Czech":
                    _wordList = Wordlist.Czech;
                    break;

                default:
                    _wordList = Wordlist.English;
                    break;
            }

            switch (wordCount)
            {
                case 12:
                    _wordCount = WordCount.Twelve;
                    break;

                case 15:
                    _wordCount = WordCount.Fifteen;
                    break;

                case 18:
                    _wordCount = WordCount.Eighteen;
                    break;

                case 21:
                    _wordCount = WordCount.TwentyOne;
                    break;

                case 24:
                    _wordCount = WordCount.TwentyFour;
                    break;

                default:
                    _wordCount = WordCount.Twelve;
                    break;
            }

            Mnemonic mnemo = new Mnemonic(_wordList, _wordCount);

            return mnemo.ToString();
        }

        public string GetAddressFromPrivateKey(string privateKey)
        {
            if (!privateKey.StartsWith("0x")) privateKey = "0x" + privateKey;
            var account = new Account(privateKey);
            return account.Address;
        }



        public async Task<string> ReadContract(string contractAddress, string functionName, string abi, params object[] parameters)
        {
            var web3 =  new Nethereum.Web3.Web3(jsonRpc);
            web3.TransactionManager.UseLegacyAsDefault = true;
            var contract = web3.Eth.GetContract(abi, contractAddress);
            var function = contract.GetFunction(functionName);
            var result = await function.CallAsync<object>(parameters);

            if (result is Tuple<BigInteger, BigInteger, BigInteger, BigInteger> structResult)
            {
                return $"0x{structResult.Item1.ToString("X")},{structResult.Item2.ToString("X")},{structResult.Item3.ToString("X")},{structResult.Item4.ToString("X")}";
            }

            if (result is BigInteger bigIntResult) return "0x" + bigIntResult.ToString("X");
            else if (result is bool boolResult) return boolResult.ToString().ToLower();
            else if (result is string stringResult) return stringResult;
            else if (result is byte[] byteArrayResult) return "0x" + BitConverter.ToString(byteArrayResult).Replace("-", "");
            else return result?.ToString() ?? "null";
        }
        
        //btc

        public static Dictionary<string, string> MnemonicToAccountEth(string words, int amount)
        {
            string password = "";
            var accounts = new Dictionary<string, string>();

            var wallet = new Nethereum.HdWallet.Wallet(words, password);

            for (int i = 0; i < amount; i++)
            {
                var recoveredAccount = wallet.GetAccount(i);

                accounts.Add(recoveredAccount.Address, recoveredAccount.PrivateKey);
            }

            return accounts;
        }
        

        public static string GetEthAccountBalance(string address, string jsonRpc)
        {
            var web3 = new Nethereum.Web3.Web3(jsonRpc);

            var balance = web3.Eth.GetBalance.SendRequestAsync(address).Result;
            return balance.Value.ToString();
        }

    }
    public class Function
    {
        public static string[] GetFuncInputTypes(string abi, string functionName)
        {
            var deserialize = new ABIJsonDeserialiser();
            var abiFunctions = deserialize.DeserialiseContract(abi).Functions;
            int paramsAmount = abiFunctions.Where(n => n.Name == functionName).SelectMany(p => p.InputParameters, (n, p) => new { Type = p.Type }).Count();
            var inputTypes = abiFunctions.Where(n => n.Name == functionName).SelectMany(p => p.InputParameters, (n, p) => new { Type = p.Type });
            string[] types = new string[paramsAmount];
            var typesList = new List<string>();
            foreach (var item in inputTypes) typesList.Add(item.Type);
            types = typesList.ToArray();
            return types;
        }

        public static Dictionary<string, string> GetFuncInputParameters(string abi, string functionName)
        {
            var deserialize = new ABIJsonDeserialiser();
            var abiFunctions = deserialize.DeserialiseContract(abi).Functions;
            var parameters = abiFunctions.Where(n => n.Name == functionName).SelectMany(p => p.InputParameters, (n, p) => new { Name = p.Name, Type = p.Type });
            return parameters.ToDictionary(p => p.Name, p => p.Type);
        }

        public static Dictionary<string, string> GetFuncOutputParameters(string abi, string functionName)
        {
            var deserialize = new ABIJsonDeserialiser();
            var abiFunctions = deserialize.DeserialiseContract(abi).Functions;
            var parameters = abiFunctions.Where(n => n.Name == functionName).SelectMany(p => p.OutputParameters, (n, p) => new { Name = p.Name, Type = p.Type });
            return parameters.ToDictionary(p => p.Name, p => p.Type);
        }

        public static string GetFuncAddress(string abi, string functionName)
        {
            var deserialize = new ABIJsonDeserialiser();
            var abiFunctions = deserialize.DeserialiseContract(abi).Functions;
            var address = abiFunctions.Where(n => n.Name == functionName).Select(f => f.Sha3Signature).First();
            return address;
        }

    }
    public class Decoder
    {
        public static Dictionary<string, string> AbiDataDecode(string abi, string functionName, string data)
        {
            var decodedData = new Dictionary<string, string>();
            if (data.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) data = data.Substring(2);
            if (data.Length < 64) data = data.PadLeft(64, '0'); // Если данные короче 64 символов, дополняем их нулями слева
            List<string> dataChunks = SplitChunks(data).ToList();
            Dictionary<string, string> parametersList = Function.GetFuncOutputParameters(abi, functionName);
            for (int i = 0; i < parametersList.Count && i < dataChunks.Count; i++)
            {
                string key = parametersList.Keys.ElementAt(i);
                string type = parametersList.Values.ElementAt(i);
                string value = TypeDecode(type, dataChunks[i]);
                decodedData.Add(key, value);
            }
            return decodedData;
        }

        private static IEnumerable<string> SplitChunks(string data)
        {
            int chunkSize = 64;
            for (int i = 0; i < data.Length; i += chunkSize) yield return i + chunkSize <= data.Length ? data.Substring(i, chunkSize) : data.Substring(i).PadRight(chunkSize, '0');
        }

        private static string TypeDecode(string type, string dataChunk)
        {
            string decoded = string.Empty;

            var decoderAddr = new AddressTypeDecoder();
            var decoderBool = new BoolTypeDecoder();
            var decoderInt = new IntTypeDecoder();

            switch (type)
            {
                case "address":
                    decoded = decoderAddr.Decode<string>(dataChunk);
                    break;
                case "uint256":
                    decoded = decoderInt.DecodeBigInteger(dataChunk).ToString();
                    break;
                case "uint8":
                    decoded = decoderInt.Decode<int>(dataChunk).ToString();
                    break;
                case "bool":
                    decoded = decoderBool.Decode<bool>(dataChunk).ToString();
                    break;
                default: break;
            }
            return decoded;
        }
    }
    public class Encoder
    {
        public static string EncodeTransactionData(string abi, string functionName, string[] types, object[] values)
        {
            string funcAddress = Function.GetFuncAddress(abi, functionName);
            string encodedParams = EncodeParams(types, values);
            string encodedData = "0x" + funcAddress + encodedParams;
            return encodedData;
        }

        public static string EncodeParam(string type, object value)
        {
            var abiEncode = new ABIEncode();
            string result = abiEncode.GetABIEncoded(new ABIValue(type, value)).ToHex();
            return result;
        }

        public static string EncodeParams(string[] types, object[] values)
        {
            var abiEncode = new ABIEncode();
            var parameters = new ABIValue[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                parameters[i] = new ABIValue(types[i], values[i]);
            }
            return abiEncode.GetABIEncoded(parameters).ToHex();
        }

        public static string EncodeParams(Dictionary<string, string> parameters)
        {
            var abiEncode = new ABIEncode();
            string result = string.Empty;
            foreach (var item in parameters) result += abiEncode.GetABIEncoded(new ABIValue(item.Value, item.Key)).ToHex();
            return result;
        }
    }
    public class Converter
    {
        public static object[] ValuesToArray(params dynamic[] inputValues)
        {
            int valuesAmount = inputValues.Length;
            var valuesList = new List<object>();
            foreach (var item in inputValues) valuesList.Add(item);
            object[] values = new object[valuesAmount];
            values = valuesList.ToArray();
            return values;
        }
    }

    #endregion
}
