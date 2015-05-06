using ListFolders.Includes;
using Tree = ListFolders.Includes.Tree;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace ListFolders {
  public partial class TreeViewer : Form {
    object[] rootTree;
    delegate void Set(TreeNode root);
    ImageList icons;

    string path;
    string rootName="no-name";

    string iconsPath = "lib/icons/";
    string closeFolder = "dir.png";
    string openFolder = "dir-open.png";

    public TreeViewer() {
      InitializeComponent();
    }

    private void TreeViewer_KeyDown(object sender, KeyEventArgs e) {
      if (e.KeyCode == Keys.Escape) {
        Close();
      }
    }

    private void prepareIcons() {
      icons = new ImageList();
      icons.ColorDepth = ColorDepth.Depth32Bit;

      DirectoryInfo list = new DirectoryInfo(iconsPath);
      foreach (FileInfo nextFile in list.GetFiles()) {
        string name = nextFile.Name;
        setIcon(icons, name);
      }
    }

    private void setIcon(ImageList icons, string name) {
      if (icons.Images.ContainsKey(name)) return;
      Image img=Image.FromFile(iconsPath + name);
      icons.Images.Add(name, img);
    }

    private void buildTree(TreeNode root, object[] tree) {
      foreach (object obNode in tree) {
        IDictionary node = (IDictionary)obNode;

        if (node.Contains("children")) {
          TreeNode dirNode = root.Nodes.Add((string)node["text"]);
          object[] dirTree = (object[])node["children"];
          buildTree(dirNode, dirTree);
        }
        else {
          TreeNode fileNode = root.Nodes.Add((string)node["text"]);
          string icon = (string)node["icon"];
          icon = Functions.extractIconName(icon);
          fileNode.ImageKey = fileNode.SelectedImageKey = icon;
        }

      }
    }

    private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
      long time1, time2;
      time1 = Functions.ms();

        TreeView view = new TreeView();
        TreeNode root = view.Nodes.Add(rootName);
        buildTree(root, rootTree);

        e.Result = root;

        TreeNode node = (TreeNode)root.Clone();
        treeView.Invoke(new Set(setTree), new object[]{node});

      time2 = Functions.ms();
      Debug.WriteLine("thread-stop: " + Functions.formatTime((int)(time2 - time1), "{0:f2} s"));
    }

    void setTree(TreeNode root) {
      long time1, time2;
      time1 = Functions.ms();

        treeView.BeginUpdate();
        treeView.Nodes.Add(root);
        treeView.EndUpdate();

      time2 = Functions.ms();
      Debug.WriteLine("tree-set: " + Functions.formatTime((int)(time2 - time1), "{0:f2} s"));
    }

    private void bLoadTree_Click(object sender, EventArgs e) {
      string json;
      object tree;

      treeView.Nodes.Clear();

      path = Functions.formatPath(tbPath.Text);
      if (path.Length == 0) {
        bBrowse.PerformClick();
        return;
      }
      rootName = Functions.getNameFromPath(path);

      prepareIcons();
      treeView.ImageList = icons;
      treeView.ImageKey = treeView.SelectedImageKey = closeFolder;

      json = Functions.readFile(path);
      tree = new JavaScriptSerializer().DeserializeObject(json);
      rootTree = (object[])tree;
      
      worker.RunWorkerAsync();
    }

    private void treeView_AfterExpand(object sender, TreeViewEventArgs e) {
      e.Node.ImageKey = openFolder;
      e.Node.SelectedImageKey = openFolder;
    }

    private void treeView_AfterCollapse(object sender, TreeViewEventArgs e) {
      e.Node.ImageKey = closeFolder;
      e.Node.SelectedImageKey = closeFolder;
    }

    private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
      var hitTest = e.Node.TreeView.HitTest(e.Location);
      if (hitTest.Location == TreeViewHitTestLocations.PlusMinus)
        return;

      if (e.Node.IsExpanded)
        e.Node.Collapse();
      else
        e.Node.Expand();
    }

    private void bBrowse_Click(object sender, EventArgs e) {
      OpenFileDialog browseFile;

      browseFile = new OpenFileDialog();
      browseFile.InitialDirectory = Directory.GetCurrentDirectory()+ @"\export\tree\json";
      browseFile.Filter = "JSON files (*.json)|*.json";
      browseFile.RestoreDirectory = true;

      if (browseFile.ShowDialog() == DialogResult.OK) {
        tbPath.Text = browseFile.FileName;
      }
    }

    private void TreeViewer_Load(object sender, EventArgs e) {
      //tbPath.Text = @"C:\1-Roman\Documents\3-programming\2-projects\Visual Studio\List Folders\List Folders\bin\Debug\export\tree\json\programming-heavy.json";
    }

  }
}
