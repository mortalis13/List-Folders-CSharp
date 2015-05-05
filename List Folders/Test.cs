using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ListFolders {
  public partial class Test : Form {
    public Test() {
      InitializeComponent();
    }

    private void test(){
      tbRes.Text = "Hi";
    }

    private void Test_Load(object sender, EventArgs e) {
      test();
    }
  }
}
