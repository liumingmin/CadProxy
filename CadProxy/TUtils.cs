using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CadProxy
{
    public class TUtils
    {

        private const string KEY_FOR_SINGLE = "#SSO$App";
        private const string IV_FOR_SINGLE = "#SSO$App";

        public static string EncodeForProcInfo(string data)
        {
            byte[] byKey = System.Text.Encoding.ASCII.GetBytes(KEY_FOR_SINGLE);
            byte[] byIV = System.Text.Encoding.ASCII.GetBytes(IV_FOR_SINGLE);
            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();

            using (MemoryStream ms = new MemoryStream())
            {
                CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV),
                    CryptoStreamMode.Write);

                StreamWriter sw = new StreamWriter(cst);

                sw.Write(data);
                sw.Flush();
                cst.FlushFinalBlock();
                sw.Flush();

                string result = Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);

                sw.Close();
                cst.Close();

                return result;
            }
        }

        public static string DecodeForProcInfo(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return string.Empty;

            byte[] byKey = System.Text.Encoding.ASCII.GetBytes(KEY_FOR_SINGLE);
            byte[] byIV = System.Text.Encoding.ASCII.GetBytes(IV_FOR_SINGLE);
            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(data);

                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();

                using (MemoryStream ms = new MemoryStream(byEnc))
                {
                    using (
                        CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV),
                            CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cst))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
            catch
            {
                return string.Empty;
            }

        }

        public static string ComputeMd5Hash(string filePath)
        {
            string result = string.Empty;
            try
            {
                using (var md5 = MD5.Create())
                {
                    var byteHashValue = md5.ComputeHash(File.ReadAllBytes(filePath));
                    result = BitConverter.ToString(byteHashValue).Replace("-", "");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
        }


        public static void RunProcess(string fileName, string arguments, bool createNoWindow = true)
        {
            try
            {
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = fileName;
                startInfo.Arguments = arguments;
                startInfo.CreateNoWindow = createNoWindow;
                startInfo.WorkingDirectory = Path.GetDirectoryName(fileName);
                if (createNoWindow)
                {
                    startInfo.UseShellExecute = false;
                    //startInfo.RedirectStandardInput = true; //重定向输入
                    //startInfo.RedirectStandardOutput = true; //重定向输出
                    //startInfo.RedirectStandardError = true; //重定向输出错误
                }
                var process = Process.Start(startInfo);
                //process.WaitForExit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
