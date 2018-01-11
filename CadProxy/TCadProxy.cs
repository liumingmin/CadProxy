using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace CadProxy
{
    public class TCadProxy
    {
        public static void OpenCad(string msg)
        {
            var origData = TUtils.DecodeForProcInfo(msg);

            if (string.IsNullOrWhiteSpace(origData))
            {
                MessageBox.Show("数据错误");
                return;
            }

            var proInfo = JsonToProcInfo(origData);

            if (proInfo == null)
            {
                MessageBox.Show("JSON格式错误");
                return;
            }

            var cacheDir = Path.GetTempPath();
            if (!Directory.Exists(cacheDir))
                Directory.CreateDirectory(cacheDir);

            var locFilePath = Path.Combine(cacheDir, proInfo.FileName);

            var isNeedDl = CheckNeedDownloadFile(locFilePath, proInfo.FileMd5);
            var dlResult = true;
            if (isNeedDl)
            {
                dlResult = DownloadFileByUrl(proInfo.FileUrl, locFilePath);
            }

            if (!dlResult)
                return;

            TUtils.RunProcess(@"D:\Program\CAD快速看图\CADReader.exe", locFilePath);

            Thread.Sleep(1000);

            Application.Exit();
        }

        private static bool DownloadFileByUrl(string remoteUrl, string locFilePath)
        {
            var clientDownload = new WebClient();

            //Added the function to support proxy
            clientDownload.Proxy = System.Net.WebProxy.GetDefaultProxy();
            clientDownload.Proxy.Credentials = CredentialCache.DefaultCredentials;
            clientDownload.Credentials = System.Net.CredentialCache.DefaultCredentials;

            try
            {
                clientDownload.DownloadFile(new Uri(remoteUrl), locFilePath);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return false;
        }

        private static bool CheckNeedDownloadFile(string locFilePath, string remoteMd5)
        {
            if (File.Exists(locFilePath))
            {
                var md5 = TUtils.ComputeMd5Hash(locFilePath);
                if (remoteMd5.Equals(md5))
                {
                    return false;
                }
            }

            return true;
        }


        #region testcode

        public static void Test()
        {
            var procInfo = new ProcInfo();
            procInfo.FileMd5 = TUtils.ComputeMd5Hash(@"E:\BimSrv2\gisserver\webapps\gis\1.dwg");
            procInfo.FileName = "1.dwg";
            procInfo.FileUrl = "http://127.0.0.1:8080/gis/1.dwg";

            var jsonstr = JsonConvert.SerializeObject(procInfo);

            var base64str = TUtils.EncodeForProcInfo(jsonstr);
            
            TCadProxy.OpenCad(base64str);
        }

        #endregion

        #region protocinfo

        private static ProcInfo JsonToProcInfo(string origStr)
        {
            if (!string.IsNullOrEmpty(origStr))
            {
                var procInfo = JsonConvert.DeserializeObject<ProcInfo>(origStr);
                if (procInfo == null) return null;

                if (!string.IsNullOrEmpty(procInfo.FileUrl) && !string.IsNullOrEmpty(procInfo.FileMd5)
                    && !string.IsNullOrEmpty(procInfo.FileName))
                    return procInfo;
                else
                    return null;
            }
            return null;
        }

        class ProcInfo
        {
            public string FileUrl { get; set; }
            public string FileName { get; set; }
            public string FileMd5 { get; set; }
        }

        #endregion
    }
}
