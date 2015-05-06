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
    Thread worker;
    TreeNode root;
    object[] rootTree;

    //long time1, time2;

    string path;
    string rootName="no-name";

    string iconsPath = "lib/icons/";
    string closeFolder = "dir.png";
    string openFolder = "dir-open.png";
    ImageList icons;

    delegate TreeNode Add(TreeNode root, string text);
    delegate void Set(TreeNode root);

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

    private void buildTree_nonThread(TreeNode root, object[] tree) {
      foreach (object obNode in tree) {
        IDictionary node = (IDictionary) obNode;

        if (node.Contains("children")) {
          TreeNode dirNode = root.Nodes.Add((string) node["text"]);
          object[] dirTree = (object[]) node["children"];
          buildTree_nonThread(dirNode, dirTree);
        }
        else {
          TreeNode fileNode = root.Nodes.Add((string) node["text"]);
          string icon = (string)node["icon"];
          icon = Functions.extractIconName(icon);
          fileNode.ImageKey = fileNode.SelectedImageKey = icon;
        }

      }
    }

    private void buildTree1(TreeView view, TreeNode root, object[] tree) {
      foreach (object obNode in tree) {
        IDictionary node = (IDictionary)obNode;

        if (node.Contains("children")) {
          TreeNode dirNode = (TreeNode)treeView.Invoke(new Add(AddNode), new object[] { root, (string)node["text"] });
          //TreeNode dirNode = (TreeNode)treeView.Invoke(new Add(AddNode), new object[] { root, (string)node["text"] });

          object[] dirTree = (object[])node["children"];
          buildTree1(view, dirNode, dirTree);
        }
        else {
          TreeNode fileNode = (TreeNode)treeView.Invoke(new Add(AddNode), new object[] { root, (string)node["text"] });
          string icon = (string)node["icon"];
          icon = Functions.extractIconName(icon);
          fileNode.ImageKey = fileNode.SelectedImageKey = icon;
        }

      }
    }

    private TreeNode AddNode(TreeNode root, string text) {
      return root.Nodes.Add(text);
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
      long time11, time22;

      time11 = Functions.ms();

        TreeView view = new TreeView();
        TreeNode root = view.Nodes.Add(rootName);
        buildTree(root, rootTree);

        e.Result = root;

        TreeNode node = (TreeNode)root.Clone();
        //treeView.Nodes.Add(node);

        treeView.Invoke(new Set(setTree), new object[]{node});

      time22 = Functions.ms();
      Debug.WriteLine("thread-stop: " + Functions.formatTime((int)(time22 - time11), "{0:f2} s"));
    }

    void setTree(TreeNode root) {
      long time1, time2;

      time1 = Functions.ms();

        treeView.Nodes.Add(root);

      time2 = Functions.ms();
      Debug.WriteLine("tree-set: " + Functions.formatTime((int)(time2 - time1), "{0:f2} s"));
    }

    private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
      //time1 = Functions.ms();
      //  TreeNode root = (TreeNode)e.Result;
      //  TreeNode node = (TreeNode) root.Clone();

      //time2 = Functions.ms();
      //Debug.WriteLine("tree-copy: " + Functions.formatTime((int)(time2 - time1), "{0:f2} s"));

      //  treeView.Nodes.Add(node);

      //prepareIcons();
      //treeView.ImageList = icons;
      //treeView.ImageKey = treeView.SelectedImageKey = closeFolder;
    }

    private void backgroundProcess() {
      TreeView view=new TreeView();
      TreeNode root = view.Nodes.Add(rootName);

      buildTree(root, rootTree);

      //treeView.Nodes.Add(rootName);

      treeView = view;
      //treeView.EndUpdate();
    }

    private void bLoadTree_Click(object sender, EventArgs e) {
      tbOut.Focus();
      treeView.Nodes.Clear();

      string json;
      object tree;

      long time1, time2;

      time1 = Functions.ms();

        path = Functions.formatPath(tbPath.Text);
        if (path.Length == 0) {
          bBrowse.PerformClick();
          return;
        }
        rootName = Functions.getNameFromPath(path);

      time2 = Functions.ms();
      Debug.WriteLine("read-path: " + Functions.formatTime((int)(time2 - time1), "{0:f2} s"));

      time1 = Functions.ms();

        prepareIcons();
        treeView.ImageList = icons;
        treeView.ImageKey = treeView.SelectedImageKey = closeFolder;

      time2 = Functions.ms();
      Debug.WriteLine("icons-load: " + Functions.formatTime((int)(time2 - time1), "{0:f2} s"));

      //treeView.BeginUpdate();
      //root = treeView.Nodes.Add(rootName);

      time1 = Functions.ms();

        json = Functions.readFile(path);
        tree = new JavaScriptSerializer().DeserializeObject(json);
        rootTree = (object[])tree;

      time2 = Functions.ms();
      Debug.WriteLine("read-json: " + Functions.formatTime((int)(time2 - time1), "{0:f2} s"));

      time1 = Functions.ms();
      
        backgroundWorker1.RunWorkerAsync();

      time2 = Functions.ms();
      Debug.WriteLine("thread-starting: " + Functions.formatTime((int)(time2 - time1), "{0:f2} s"));

      //worker = new Thread(backgroundProcess);
      //worker.Start();

      //buildTree(root, rootTree);
      //treeView.EndUpdate();
    }

    private void bLoadTree_nonThread_Click(object sender, EventArgs e) {
      string json;
      object tree;

      TreeView view = treeView;

      prepareIcons();
      treeView.ImageList = icons;
      treeView.ImageKey = view.SelectedImageKey = closeFolder;

      treeView.Nodes.Clear();

      path = Functions.formatPath(tbPath.Text);
      if (path.Length == 0) {
        bBrowse.PerformClick();
        return;
      }
      rootName = Functions.getNameFromPath(path);

      treeView.BeginUpdate();
      root = treeView.Nodes.Add(rootName);

      json = Functions.readFile(path);
      tree = new JavaScriptSerializer().DeserializeObject(json);
      rootTree = (object[])tree;

      worker = new Thread(backgroundProcess);
      worker.Start(treeView);

      //buildTree(root, rootTree);
      //treeView.EndUpdate();
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
      tbPath.Text = @"C:\1-Roman\Documents\3-programming\2-projects\Visual Studio\List Folders\List Folders\bin\Debug\export\tree\json\programming-heavy.json";
    }

  }
}
