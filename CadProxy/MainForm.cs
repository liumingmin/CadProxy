using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CadProxy
{
    public partial class MainForm : Form
    {
        public string _base64str;
        public MainForm(string base64str)
        {
            _base64str = base64str;
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var task = new Task((() =>
            {
                //TCadProxy.OpenCad(_base64str);
                TCadProxy.Test();
            }));

            task.Start();
        }
    }
}
