﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

namespace Test {
  public partial class Test : Form {

    const string nl = "\r\n";

    public Test() {
      InitializeComponent();
    }

    private void tbRes_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Escape) {
        Close();
      }
    }

    private void test1() {
      int[] data = { 4, 5, 6, 7, 2, 4, 634, 123, 56, 45 };
      var json = new JavaScriptSerializer().Serialize(data);
      tbRes.Text = json;
    }

    private void test2() {
      string path = "C:/1-Roman/Documents/8-test/list-test/en";
      string exportName="no";

      Regex regex = new Regex(@"/([^/]+)$");
      Match match = regex.Match(path);
      if (match.Success) {
        exportName = match.Groups[1].ToString();
      }

      tbRes.Text = exportName;
    }

    private void test3() {
      string path = "templates/tree.html";
      string startupPath = Application.StartupPath;
      string res = "no";

      try {
        res = File.ReadAllText(path);
      }
      catch (Exception e) {
        res = "Not Found"+nl;

        res += e.Message+nl;
        res += e.StackTrace + nl;
      }

      tbRes.Text = res;
    }

    private void test4() {
      string path = "out.html";
      string text = "123";
      string res = "Done";

      try {
        File.WriteAllText(path, text);
      }
      catch (Exception e) {
        res = "Not Found" + nl;

        res += e.Message + nl;
        res += e.StackTrace + nl;
      }

      tbRes.Text = res;
    }

    private void test() {
    }

    private void Test_Load(object sender, EventArgs e) {
      test();
    }

  }
}
