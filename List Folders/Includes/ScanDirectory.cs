using ListFolders.Includes.Tree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace ListFolders.Includes {
  public class ScanDirectory {
    MainForm form;

    public string path;

    public string text;
    public List<TreeNode> jsonArray;

    const string nl = "\r\n";
    const string pad="    ";
    const string iconsPath="./lib/images/";
    
    public List<string> filterExt;
    public List<string> excludeExt;
    public List<string> filterDir;
    
    bool doExportText;
    bool doExportTree;
    
    string exportName="";
    
    int dirCount = 0;
    int rootDirCount = 0;
    int longestDirName = 0;
    bool scanCanceled = false;
    long prevTime = 0;
    int totalTime = 0;
    
    string[] exts={                                       // sets of extensions for tree view icons (stored in lib/images)
      "chm", "css", "djvu", "dll", "doc", 
      "exe", "html", "iso", "js", "msi", 
      "pdf", "php", "psd", "rar", "txt", 
      "xls", "xml", "xpi", "zip",
    };
    
    string[] imageExts={
      "png", "gif", "jpg", "jpeg", "tiff", "bmp",
    };

    string[] musicExts={
      "mp3", "wav", "ogg", "alac", "flac",
    };

    string[] videoExts={
      "mkv", "flv", "vob", "avi", "wmv",
      "mov", "mp4", "mpg", "mpeg", "3gp",
    };

// --------------------------------------------- Constructor --------------------------------------------- 

    public ScanDirectory(MainForm form) {
      string filterExtText, excludeExtText, filterDirText;
      this.form = MainForm.form;
      
      IDictionary fields=Functions.getFieldsMap();

      path=(string) fields["path"];
      path=Functions.formatPath(path);
      form.tbPath.Text=path;
      
      filterExtText=(string) fields["filterExt"];
      excludeExtText=(string) fields["excludeExt"];
      filterDirText=(string) fields["filterDir"];
      
      doExportText=(bool) fields["doExportText"];
      doExportTree=(bool) fields["doExportTree"];
      exportName=(string) fields["exportName"];
      
      filterExt=getFilters(filterExtText);
      excludeExt=getFilters(excludeExtText);
      filterDir=getFilters(filterDirText);
    }

    public void startScan(){                                         // << Start point >>
      jsonArray=fullScan(path, 0);
      done();
      form.tbOut.Text = text;
    }
    
    private void done(){
      if(doExportTree) exportTree();
    }

    private List<TreeNode> fullScan(string dir, int level) {
      List<TreeNode> json, res;
      TreeNode node;
      DirectoryInfo list;
      string pad;
      
      json = new List<TreeNode>();
      
      list = new DirectoryInfo(dir);
      pad = getPadding(level);

      foreach (DirectoryInfo nextDir in list.GetDirectories()) {
        string name=nextDir.Name;
        if(level==0 && !filterDirectory(name)) continue;
        
        string currentDir = "[" + name + "]";
        text += pad + currentDir + nl;
        
        res=fullScan(nextDir.FullName, level+1);
        
        node = new DirNode(name, res);
        json.Add(node);
      }

      foreach (FileInfo nextFile in list.GetFiles()) {
        string name=nextFile.Name;
        if(!filterFile(name)) continue;
        
        string currentFile = name;
        text += pad + currentFile + nl;
        
        node = new FileNode(name, getIcon(name));
        json.Add(node);
      }
      
      return json;
    }
    
// --------------------------------------------------- helpers ---------------------------------------------------
    
    /*
     * Replaces strings from the tree template (strings format: '_string_') with the 'replacement' text
     */
    private string replaceTemplate(string tmpl, string replacement, string text){
      text=text.Replace(tmpl, replacement);
      return text;
    }
    
    /*
     * Outputs padding spaces for text output depending on nesting level
     */
    private string getPadding(int level) {
      string resPad = "";
      for (int i = 0; i < level; i++) {
        resPad += pad;
      }
      return resPad;
    }
    
    /*
     * Returns icon path for the tree view
     */
    private string getIcon(string file){
      string ext, icon, path, iconExt;
      bool useDefault=true;
      
      ext="";
      icon="jstree-file";
      path=iconsPath;
      iconExt=".png"; 
      
      return icon;
    }
    
// --------------------------------------------------- filters ---------------------------------------------------
    
    /*
     * Gets text for the tree template
     */
    private string getFiltersText() {
      string filterExtText="", excludeExtText="", filterDirText="", filters="";
      
      if(filterExt.Count!=0){
        filterExtText = string.Join(",", filterExt);
      }
      if(excludeExt.Count!=0){
        excludeExtText = string.Join(",", excludeExt);
      }
      if(filterDir.Count!=0){
        filterDirText = string.Join(",", filterDir); 
      }
      
      filters="Files include ["+filterExtText+"]";
      filters+=", Files exclude ["+excludeExtText+"]";
      filters+=", Directories ["+filterDirText+"]";
      
      return filters;
    }
    
    /*
     * Cleans, trims and checks filters for emptiness
     */
    private List<string> getFilters(string filter) {
      List<string> list=new List<string>();
      string[] elements;
      filter=filter.Trim();
      
      if(filter.Length!=0){
        elements=filter.Split('\n');
        foreach(string s in elements){
          list.Add(s.Trim());
        }
      }      
      
      return list;
    }    
  
    /*
     * Filters file extensions and returns true if the file will be included in the output
     * If exclude filter is not empty ignores the include filter
     */
    private bool filterFile(string file) {
      if(excludeExt.Count!=0){
        foreach(string ext in excludeExt){
          if(Functions.matches(@"\."+ext+"$",file))
            return false;
        }
        return true;
      }
      
      if(filterExt.Count==0) return true;
      foreach(string ext in filterExt){
        if(Functions.matches(@"\."+ext+"$",file))
          return true;
      }
      return false;
    }
    
    /*
     * Uses form filter to filter directories from the first scanning level
     */
    private bool filterDirectory(string dir) {
      if(filterDir.Count==0) return true;
      
      foreach(string filter in filterDir){
        if(filter.Equals(dir))
          return true;
      }
      return false;
    }
    
// --------------------------------------------------- exports ---------------------------------------------------
    
    /*
     * Exports .json and .html files to the 'export/tree'
     * The .html file can be used directly to view the tree
     * The jsTree plugin must be in the 'tree/lib'
     *
     * The method gets the .html template from 'templates/tree.html', 
     * replaces template strings with the current data and create new .html in the 'exports/tree'
     * Then creates .json in the 'exports/tree/json' which is read by the script in the exported .html page
     */
    private void exportTree() {
      string tmpl, doc, json, treeName, 
      exportPath, jsonFolder, jsonPath, 
      exportDoc, exportJSON;
      string filters;
      string jsonFile, htmlFile;
      
      json = new JavaScriptSerializer().Serialize(jsonArray);
      
      treeName=getExportName(null);                                         // get name
      
      tmpl="templates/tree.html";
      exportPath="export/tree/";
      jsonFolder="json/";
      jsonPath=exportPath+jsonFolder;
      
      exportDoc=treeName+".html";
      exportJSON=treeName+".json";
      
      doc=Functions.readFile(tmpl);                                               // process template
      doc=replaceTemplate("_jsonPath_", jsonFolder+exportJSON, doc);
      doc=replaceTemplate("_Title_", "Directory: "+treeName, doc);
      doc=replaceTemplate("_FolderPath_", "Directory: "+path, doc);
      
      filters=getFiltersText();
      doc=replaceTemplate("_Filters_", "Filters: "+filters, doc);
      
      htmlFile=exportPath+exportDoc;                                        // get paths
      jsonFile=jsonPath+exportJSON;
        
      Functions.writeFile(htmlFile, doc);                                             // write results
      Functions.writeFile(jsonFile, json);
    }
    
    /*
     * Returns the name that will be used to export 
     * text, markup and tree views of the directory structure
     */
    private string getExportName(string ext){
      bool useCurrentDir=true;
      string exportName, name;
      
      exportName="no-name";
      
      if(this.exportName.Length!=0){
        exportName=this.exportName;
        useCurrentDir=false;
      }
      
      if(useCurrentDir){
        exportName="export";
        
        Regex regex = new Regex(@"/([^/]+)$");
        Match match = regex.Match(path);
        if (match.Success) {
          exportName = match.Groups[1].ToString();
        }
      }
      
      name=exportName;
      if(ext!=null) name+=ext;
      
      return name;
    }
      
  }

}
