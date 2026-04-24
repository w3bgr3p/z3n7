
using System;

using System.Globalization;

using System.Numerics;

using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3nIO.Web3
{
    

    public static partial class Web3Tools 
    {
        
        public static decimal OKXPrice(this IZennoPosterProjectModel project, string tiker)
        {
            tiker = tiker.ToUpper();
            return new Api.OkxApi(project).OKXPrice<decimal>($"{tiker}-USDT");

        }
        public static decimal UsdToToken(this IZennoPosterProjectModel project, decimal usdAmount, string tiker, string apiProvider = "KuCoin")
        {
            decimal price;
            switch (apiProvider)
            {
                case "KuCoin":
                    price = Api.KuCoin.KuPrice(tiker);
                    break;
                case "OKX":
                    price = project.OKXPrice(tiker);
                    break;
                case "CoinGecco":
                    price = Api.CoinGecco.PriceByTiker(tiker);
                    break;
                default:
                    throw new ArgumentException($"unknown method {apiProvider}");
            }
            return usdAmount / price;
        }
        private static decimal ToDecimal(this BigInteger balanceWei, int decimals = 18)
        {
            BigInteger divisor = BigInteger.Pow(10, decimals);
            BigInteger integerPart = balanceWei / divisor;
            BigInteger fractionalPart = balanceWei % divisor;

            decimal result = (decimal)integerPart + ((decimal)fractionalPart / (decimal)divisor);
            return result;
        }
        private static decimal ToDecimal(this string balanceHex, int decimals = 18)
        {
            BigInteger number = BigInteger.Parse("0" + balanceHex, NumberStyles.AllowHexSpecifier);
            return ToDecimal(number, decimals);
        }


    }



    public static partial class StringExtensions
    {
        public static BigInteger WeiToEth(this string wei, int decimals = 18)
        {
            return BigInteger.Parse(wei, NumberStyles.AllowHexSpecifier);
        }
    }



}
