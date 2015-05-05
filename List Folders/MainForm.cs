using ListFolders.Includes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ListFolders {
  public partial class MainForm : Form {
    public static MainForm form;
    public static Database db;
    ScanDirectory scandir;
    
    public static bool startScan=true;

    public MainForm() {
      InitializeComponent();
      pStatus.Paint += Functions.drawBorder;

      tbPath.Text = @"C:\1-Roman\Documents\8-test\list-test\en";
    }

    private void bScanDir_Click(object sender, EventArgs e) {
      if (MainForm.startScan) {
        scandir = new ScanDirectory(form);
        scandir.startScan();
      }
      else {
        scandir.stopScan();
      }
    }

    private void MainForm_Load(object sender, EventArgs e) {
      form = this;
      db = new Database();
      new Functions();
      Functions.loadFields();
    }

    private void bBrowse_Click(object sender, EventArgs e) {
      FolderBrowserDialog browseDir;
      DialogResult result;
      string folderName, path;
      
      browseDir = new FolderBrowserDialog();
      path=tbPath.Text;
      if (path.Length != 0)
        browseDir.SelectedPath = Functions.formatPath(path);
      result = browseDir.ShowDialog();
      
      if (result == DialogResult.OK) {
        folderName = browseDir.SelectedPath;
        tbPath.Text = folderName;
      }

      // OpenFileDialog openFileDialog1 = new OpenFileDialog();

      // openFileDialog1.InitialDirectory = "c:\\" ;
      // openFileDialog1.RestoreDirectory = true ;

      // if(openFileDialog1.ShowDialog() == DialogResult.OK) {
      //   tbPath.Text=openFileDialog1.FileName;
      // }
    }

    private void bClearFilterExt_Click(object sender, EventArgs e) {
      tbFilterExt.Clear();
    }

    private void bClearExcludeExt_Click(object sender, EventArgs e) {
      tbExcludeExt.Clear();
    }

    private void bClearFilterDir_Click(object sender, EventArgs e) {
      tbFilterDir.Clear();
    }

    private void bClearOut_Click(object sender, EventArgs e) {
      tbOut.Clear();
    }

    private void MainForm_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Escape) {
        Close();
      }
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
      string value = Functions.encodeJSON(Functions.getFieldsMap());
      db.updateConfig("last", value);
      db.CloseConnection();
    }

    private void bTest_Click(object sender, EventArgs e) {
      //Functions.log(res.ToString());
    }

  }
}
