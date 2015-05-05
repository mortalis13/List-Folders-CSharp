using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ListFolders.Includes {
  class Functions {
    
    /*
     * Gets Dictionary of all form fields
     * which is used to serialize them to JSON string
     */
    public static IDictionary getFieldsMap(){
      MainForm form=MainForm.form;
      
      Dictionary<string, object> dict=new Dictionary<string, object>();
        
      string path, filterExt, excludeExt, filterDir, exportName;
      bool doExportText, doExportTree;
      
      path=form.tbPath.Text;
      filterExt=form.tbFilterExt.Text;
      excludeExt=form.tbExcludeExt.Text;
      filterDir=form.tbFilterDir.Text;
      
      doExportText=form.chExportText.Checked;
      doExportTree=form.chExportTree.Checked;
      exportName=form.tbExportName.Text;
      
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
     * Formats path, fixes backslashes, trims
     */
    public static string formatPath(string path) {
      path=path.Replace('/', '\\');
      path=path.Trim();
      return path;
    }

    public static string readFile(string path) {
      string res = null;

      try {
        res = File.ReadAllText(path);
      }
      catch (Exception e) {}

      return res;
    }
    
    public static void writeFile(string path, string text) {
      try {
        File.WriteAllText(path, text);
      }
      catch (Exception e) {}
    }
    
    public static bool matches(string regex, string text) {
      Regex rx = new Regex(regex);
      Match match = rx.Match(text);
      return match.Success;
    }
    
    public static string regexFind(string pattern, string text, int group=1){
      string res="";
      
      Regex rx = new Regex(pattern);
      Match match = rx.Match(text);
      if(match.Success){
        res = match.Groups[group].ToString();
      }
      
      return res;
    }
    
    public static void log(string text){
      MainForm.form.tbOut.Text+=text;
    }

  }
}
