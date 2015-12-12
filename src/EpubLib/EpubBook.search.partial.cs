using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
//using System.Text;
namespace Lei.Common
{
    partial class EpubBook
    {
        public static TreeNode SearchNodeName(TreeNode treeNode, string text)
        {
            if (treeNode.Name == text)
                return treeNode;
            if (treeNode.Nodes.Count > 0)
            {
                foreach (TreeNode n in treeNode.Nodes)
                {
                    TreeNode t = SearchNodeName(n, text);
                    if (t != null)
                        return t;
                }
            }
            return null;
        }
        public static void SearchTopic(TreeNode treeNode, string topic, List<TreeNode> list)
        {
            //if (Regex.IsMatch(treeNode.Text,text)
            if (treeNode.Text.Contains(topic))
                list.Add(treeNode);
            if (treeNode.Nodes.Count > 0)
            {
                foreach (TreeNode n in treeNode.Nodes)
                {
                    SearchTopic(n, topic, list);
                }
            }
        }
    }
}
