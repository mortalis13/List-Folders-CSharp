using ListFolders.Includes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ListFolders
{
    public partial class MainForm : Form
    {
        public static MainForm form;

        public MainForm()
        {
            InitializeComponent();
            tbPath.Text = "C:/1-Roman/Documents/8-test/list-test/en";
        }

        private void bScanDir_Click(object sender, EventArgs e)
        {
            ScanDirectory scandir = new ScanDirectory(form);
            scandir.startScan();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            form = this;
        }
    }
}
