using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace ListFolders.Includes {
  public class Functions {

    static MainForm form;

    delegate void dlgClearLog();

    public Functions() {
      form = MainForm.form;
    }

    /*
     * Gets Dictionary of all form fields
     * which is used to serialize them to JSON string
     */
    public static IDictionary getFieldsMap() {
      // form=MainForm.form;

      Dictionary<string, object> dict = new Dictionary<string, object>();

      string path, filterExt, excludeExt, filterDir, exportName;
      bool doExportText, doExportTree;

      path = form.tbPath.Text;
      filterExt = form.tbFilterExt.Text;
      excludeExt = form.tbExcludeExt.Text;
      filterDir = form.tbFilterDir.Text;

      doExportText = form.chExportText.Checked;
      doExportTree = form.chExportTree.Checked;
      exportName = form.tbExportName.Text;

      dict.Add("path", path);
      dict.Add("filterExt", filterExt);
      dict.Add("excludeExt", excludeExt);
      dict.Add("filterDir", filterDir);
      dict.Add("doExportText", doExportText);
      dict.Add("doExportTree", doExportTree);
      dict.Add("exportName", exportName);

      return dict;
    }

    /*
     * Loads field values from the 'options' table
     * and assigns each value to appropriate field on the form
     */
    public static void loadFields(string fieldsList) {
      Dictionary<string, object> fields;
      if (fieldsList.Length == 0) return;
      fields = (Dictionary<string, object>)decodeJSON(fieldsList);

      int x = 1;

      assignFields(fields);
    }

    /*
     * Loads and assign the last options set
     * saved after previous application session
     * Redirects to more general method loadFields(string)
     */
    public static void loadFields() {
      string last = MainForm.db.loadLastOptions();
      if (last == null) return;
      loadFields(last);
    }

    /*
     * Assigns values from the HashMap to form fields
     */
    private static void assignFields(Dictionary<string, object> fields) {
      // MainForm form=MainForm.form;

      form.tbPath.Text = (string)fields["path"];
      form.tbFilterExt.Text = (string)fields["filterExt"];
      form.tbExcludeExt.Text = (string)fields["excludeExt"];
      form.tbFilterDir.Text = (string)fields["filterDir"];

      form.chExportText.Checked = (bool)fields["doExportText"];
      form.chExportTree.Checked = (bool)fields["doExportTree"];
      form.tbExportName.Text = (string)fields["exportName"];
    }

    /*
     * Formats path, fixes backslashes, trims
     */
    public static string formatPath(string path) {
      path = path.Replace('/', '\\');
      path = path.Trim();
      return path;
    }

    /*
     * Returns file name from the full path of the JSON file
     * the name is assigned to the root directory name
     */
    public static string getNameFromPath(string path) {
      string name = "";
      name = regexFind(@"\\([^\\]+)\.[^.]+$", path, 1);
      return name;
    }

    public static string readFile(string path) {
      string res = null;

      try {
        res = File.ReadAllText(path);
      }
      catch (Exception e) { }

      return res;
    }

    public static void writeFile(string path, string text) {
      try {
        File.WriteAllText(path, text);
      }
      catch (Exception e) { }
    }

    public static bool matches(string regex, string text) {
      Regex rx = new Regex(regex);
      Match match = rx.Match(text);
      return match.Success;
    }

    public static string regexFind(string pattern, string text, int group = 1) {
      string res = null;

      Regex rx = new Regex(pattern);
      Match match = rx.Match(text);
      if (match.Success) {
        res = match.Groups[group].ToString();
      }

      return res;
    }

    public static void log(string text) {
      form.tbOut.Text += text;
    }

    public static void clearLog() {
      RichTextBox tbOut = form.tbOut;

      if (tbOut.InvokeRequired) {
        form.Invoke((dlgClearLog)clearLog);
      }
      else {
        tbOut.Clear();
      }
    }

    public static long ms() {
      long time = (long)DateTime.Now.TimeOfDay.TotalMilliseconds;
      return time;
    }

    /*
     * Formats time value according to the format
     */
    public static string formatTime(int time, string format) {
      string res = String.Format(format, (float)time / 1000);
      return res;
    }

    public static void drawBorder(object sender, PaintEventArgs e) {
      var control = (Control)sender;

      var graphics = e.Graphics;
      var bounds = e.Graphics.ClipBounds;

      Pen pen = new Pen(Color.FromArgb(120, 120, 120));
      pen.DashStyle = DashStyle.Dot;
      graphics.DrawLine(pen, new PointF(0, 0), new PointF(control.Width, 0));
    }

    public static void setProgress(int val) {
      form.progressBar.Value = val;
    }

    public static string extractIconName(string path) {
      string icon = null;
      icon = regexFind(@"/([^/]+\.[^/.]+)$", path);
      if (icon == null) icon = "file.png";
      return icon;
    }

// ---------------------------------------- JSON serialization ----------------------------------------

    public static string encodeJSON(object obj) {
      string json = new JavaScriptSerializer().Serialize(obj);
      return json;
    }

    public static IDictionary decodeJSON(string json) {
      IDictionary dict = new JavaScriptSerializer().Deserialize<IDictionary>(json);
      return dict;
    }

// ------------------------------------------------------------------------------------------------------

    /*
     * Template function to get processing time (not for the direct use, copy and edit it)
     */
    private void calcTime() {
      long time1, time2;

      time1 = Functions.ms();
      object tree = new JavaScriptSerializer().DeserializeObject("");                 // process
      time2 = Functions.ms();
      form.tbOut.Text += Functions.formatTime((int)(time2 - time1), "{0:f2} s\n");
    }

  }
}
