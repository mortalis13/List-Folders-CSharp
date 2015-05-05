using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListFolders.Includes.Tree {
  public class TreeNode {
    public String text;

    public TreeNode(String text) {
      this.text = text;
    }

    public String toString() {
      return text;
    }
  }
}
