using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace CadProxy
{
    class Program
    {
        static void Main(string[] args)
        {
            string base64str = string.Empty;
            if (args.Any())
            {
                base64str = args[0].Replace("cadviewer://", string.Empty);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new MainForm(base64str));
        }

        
    }
}
