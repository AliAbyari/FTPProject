using FTPProject.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Serilog;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ContractsManagement.Api.Helpers
{
    public static class SecurityHelpers
    {
        private static AppSetting? _appSettings;

        public static void Initialize(IOptions<AppSetting> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create();  //or use SHA1.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }
        public static void InsertToLog(bool insertLog, string logText, string type = "Error", [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
        {
            try
            {
                if (insertLog)
                    switch (type)
                    {
                        case "Error":
                            Log.Error(logText, lineNumber + " at line " + lineNumber + " (" + caller + ")" + "\n");
                            break;
                        case "Info":
                            Log.Information(logText, lineNumber + " at line " + lineNumber + " (" + caller + ")" + "\n");
                            break;
                        default:
                            break;
                    }
            }
            catch (Exception)
            {

            }
        }


    }
}
