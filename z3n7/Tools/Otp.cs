using System;
using System.Threading;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace z3n7.Tools
{
    /// <summary>One-time codes.</summary>
    public static class Otp
    {
        /// <summary>
        /// Computes the current TOTP code from a Base32 secret. When the code expires within
        /// <c>waitIfTimeLess</c> seconds, waits for the next one.
        /// </summary>
        /// <param name="keyString">Base32 secret.</param>
        /// <param name="waitIfTimeLess">Seconds of validity below which the next code is awaited.</param>
        /// <returns>The code. Throws for an empty secret.</returns>
        public static string Offline(string keyString, int waitIfTimeLess = 5)
        {
            if (string.IsNullOrEmpty(keyString))
                throw new Exception($"invalid input:[{keyString}]");
            
            var key = OtpNet.Base32Encoding.ToBytes(keyString.Trim());
            var otp = new OtpNet.Totp(key);
            string code = otp.ComputeTotp();
            int remainingSeconds = otp.RemainingSeconds();

            if (remainingSeconds <= waitIfTimeLess)
            {
                Thread.Sleep(remainingSeconds * 1000 + 1);
                code = otp.ComputeTotp();
            }

            return code;
        }
        /// <summary>
        /// Code from the latest FirstMail message sent to <c>email</c> (see <c>z3n7.FirstMail.GetOTP</c>).
        /// </summary>
        /// <param name="email">Original recipient the message was sent to.</param>
        public static string FirstMail(IZennoPosterProjectModel project, string email )
        {
            if (string.IsNullOrEmpty(email))
                throw new Exception($"invalid input:[{email}]");
            return new FirstMail(project).GetOTP(email);
        }
    }
}




