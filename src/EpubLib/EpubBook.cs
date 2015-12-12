using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using Ionic.Zip;

namespace Lei.Common
{
    public partial class EpubBook
    {
        public TreeNode TreeNode;
        public static ImageList ImageList;
        public bool IsSpine = false;
        public string EpubFilename = "";
        public string EpubTitle = "";//从opf文件读取
        public string EpubInformation = "";//从opf文件读取
        public string StateMsg = null;

        private string Doc_Anchor = "";
        private List<string> Script_Files = new List<string>();
        private List<string> XHTML_Files = new List<string>();
        private List<string> XHTML_Sources = new List<string>();

        private string Cover_Content = "";//从opf文件读取
        private string Cover_URL = "";//从opf文件读取

        private string Encoding_Default = "";
        private string Encoding_URL = "";
        private bool Encoding_Reflesh;

        private bool SelectedNode_Flag;
        private bool Navigated_Flag;

        private string Temp_Path = "";
        public static Encoding defaultCoding;
        public static string defaultCodingString;
        static EpubBook()
        {
            ImageList = new ImageList();
            ImageList.Images.Add(Properties.Resources.Book_angle);
            ImageList.Images.Add(Properties.Resources.Book_open);
            ImageList.Images.Add(Properties.Resources.Page);
            ImageList.Images.Add(Properties.Resources.PageUnselect);

        }
        public EpubBook()
        {
            //tree.ImageList = imagelist;
            try
            {
                defaultCoding = Encoding.Default;
                defaultCodingString = "System Default (" + defaultCoding.WebName.ToUpper() + ")";
            }
            catch
            {
            }
        }

        public void OpenEpubFile(string filename)
        {
            CloseEpubFile();
            if (!this.extractEPUBFile(filename))
            {
                MessageBox.Show("Cannot open file \"" + filename + "\".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            this.EpubFilename = filename;
            this.EpubTitle = Path.GetFileNameWithoutExtension(filename);
            this.readEPUBFile();
            if (TreeNode.Nodes.Count == 0)
            {
                MessageBox.Show("No book information in file \"" + filename + "\".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
        public void CloseEpubFile()
        {
            this.EpubFilename = "";
            this.EpubTitle = "";
            this.EpubInformation = "";
            this.Doc_Anchor = "";
            this.Script_Files.Clear();
            this.XHTML_Files.Clear();
            this.XHTML_Sources.Clear();
            this.SelectedNode_Flag = false;
            this.Navigated_Flag = false;
            this.Cover_Content = "";
            this.Cover_URL = "";
            this.Encoding_Default = "";
            this.Encoding_URL = "";
            this.Encoding_Reflesh = false;
            this.TreeNode = null;

            try
            {
                if (this.Temp_Path != "")
                {
                    this.StateMsg = "Closing...";
                    try
                    {
                        if (Directory.Exists(this.Temp_Path))
                        {
                            Directory.Delete(this.Temp_Path, true);
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        this.StateMsg = null;
                    }
                    try
                    {
                        if (Directory.Exists(this.Temp_Path))
                        {
                            Directory.Delete(this.Temp_Path, true);
                        }
                    }
                    catch
                    {
                    }
                    this.Temp_Path = "";
                }
            }
            catch
            {
            }
        }

        protected void readEPUBFile()
        {
            if (this.EpubFilename == "")
            {
                return;
            }
            string text = this.Temp_Path + "\\META-INF\\container.xml";
            if (!File.Exists(text))
            {
                return;
            }
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.XmlResolver = null;
            xmlDocument.Load(text);
            List<string> list = new List<string>();
            XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("rootfile");
            for (int i = 0; i < elementsByTagName.Count; i++)
            {
                XmlNode xmlNode = elementsByTagName[i].Attributes["full-path"];
                if (xmlNode != null && xmlNode.Value != "")
                {
                    string text2 = this.Temp_Path + "\\" + xmlNode.Value.Replace("/", "\\");
                    if (File.Exists(text2))
                    {
                        list.Add(text2);
                    }
                }
            }
            if (list.Count == 0 && File.Exists(this.Temp_Path + "\\content.opf"))
            {
                list.Add(this.Temp_Path + "\\content.opf");
            }
            //tree.BeginUpdate();
            //try
            //{
                foreach (string current in list)
                {
                    this.readContentOPF(current);
                }
            //    if (tree.Nodes.Count > 0)
            //    {
            //        tree.Nodes[0].Expand();
            //        if (tree.Nodes[0].Nodes.Count > 0 && tree.Nodes[0].Nodes[0].Name == tree.Nodes[0].Name)
            //        {
            //            tree.Nodes[0].Name = "";
            //        }
            //        if (tree.Nodes[0].Name != "")
            //        {
            //            tree.SelectedNode = tree.Nodes[0];
            //        }
            //        else
            //        {
            //            if (tree.Nodes[0].Nodes.Count > 0 && tree.Nodes[0].Nodes[0].Name != "")
            //            {
            //                tree.SelectedNode = tree.Nodes[0].Nodes[0];
            //            }
            //        }
            //    }
            //}
            //finally
            //{
            //    tree.EndUpdate();
            //}
        }
        protected void readContentOPF(string content)
        {
            if (!File.Exists(content))
            {
                return;
            }
            string directoryName = Path.GetDirectoryName(content);
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.XmlResolver = null;
                xmlDocument.Load(content);
                XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("dc:title");
                if (elementsByTagName.Count > 0 && elementsByTagName[0].InnerText != "")
                {
                    this.EpubTitle = elementsByTagName[0].InnerText;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            #region 读取opf文件内的metadata(设置EPU_Infromation)
            try
            {
                XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("dc:creator");
                if (elementsByTagName.Count > 0)
                {
                    this.EpubInformation = this.EpubInformation + "Creator: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:subject");
                if (elementsByTagName.Count > 0)
                {
                    this.EpubInformation = this.EpubInformation + "Subject: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:description");
                if (elementsByTagName.Count > 0)
                {
                    this.EpubInformation = this.EpubInformation + "Description: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:publisher");
                if (elementsByTagName.Count > 0)
                {
                    this.EpubInformation = this.EpubInformation + "Publisher: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:contributor");
                if (elementsByTagName.Count > 0)
                {
                    this.EpubInformation = this.EpubInformation + "Contributor: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:date");
                if (elementsByTagName.Count > 0)
                {
                    this.EpubInformation = this.EpubInformation + "Date: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:type");
                if (elementsByTagName.Count > 0)
                {
                    this.EpubInformation = this.EpubInformation + "Type: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:format");
                if (elementsByTagName.Count > 0)
                {
                    this.EpubInformation = this.EpubInformation + "Formate: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:identifier");
                if (elementsByTagName.Count > 0)
                {
                    this.EpubInformation = this.EpubInformation + "Identifiere: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:source");
                if (elementsByTagName.Count > 0)
                {
                    this.EpubInformation = this.EpubInformation + "Source: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:language");
                if (elementsByTagName.Count > 0)
                {
                    this.EpubInformation = this.EpubInformation + "Language: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:relation");
                if (elementsByTagName.Count > 0)
                {
                    this.EpubInformation = this.EpubInformation + "Relation: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:coverage");
                if (elementsByTagName.Count > 0)
                {
                    this.EpubInformation = this.EpubInformation + "Coverage: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:rights");
                if (elementsByTagName.Count > 0)
                {
                    this.EpubInformation = this.EpubInformation + "Rights: " + elementsByTagName[0].InnerText + "\n";
                }
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            #endregion

            #region opf文件内guide节点下所有reference的读取(包含目录、start、cover?)
            string coverUrltext = "";
            List<string> referenceHreflist = new List<string>();
            List<string> referenceTitlelist = new List<string>();
            List<string> referenceTypelist = new List<string>();
            try
            {
                XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("reference");
                for (int i = 0; i < elementsByTagName.Count; i++)
                {
                    string referenceHrefText = "";
                    string referenceTitleText = "";
                    string referenceTypeText = "";
                    if (elementsByTagName[i].Attributes["href"] != null && elementsByTagName[i].Attributes["href"].Value != "")
                    {
                        referenceHrefText = elementsByTagName[i].Attributes["href"].Value;
                    }
                    if (elementsByTagName[i].Attributes["title"] != null && elementsByTagName[i].Attributes["title"].Value != "")
                    {
                        referenceTitleText = elementsByTagName[i].Attributes["title"].Value;
                    }
                    if (elementsByTagName[i].Attributes["type"] != null && elementsByTagName[i].Attributes["type"].Value != "")
                    {
                        referenceTypeText = elementsByTagName[i].Attributes["type"].Value.ToLower();
                    }
                    if (!string.IsNullOrEmpty(referenceHrefText) && !string.IsNullOrEmpty(referenceTitleText) && !string.IsNullOrEmpty(referenceTypeText) && (referenceTypeText == "cover" || referenceTypeText == "title-page" || referenceTypeText == "toc" || referenceTypeText == "index" || referenceTypeText == "copyright" || referenceTypeText == "copyright-page" || referenceTypeText == "text") && File.Exists(directoryName + "\\" + this.getFileName(referenceHrefText)))
                    {
                        referenceHreflist.Add(referenceHrefText);
                        referenceTitlelist.Add(referenceTitleText);
                        referenceTypelist.Add(referenceTypeText);
                    }
                    if (referenceTypeText == "cover" && !string.IsNullOrEmpty(referenceHrefText) && File.Exists(directoryName + "\\" + this.getFileName(referenceHrefText)))
                    {
                        coverUrltext = this.getFileURL(directoryName, referenceHrefText);
                    }
                }
            }
            catch
            {
            }
            #endregion

            #region opf文件manifest节点下所有item的读取（item节点包含epub所有文件集合）
            List<string> manifestIDlist = new List<string>();
            List<string> menifestHerflist = new List<string>();
            try
            {
                XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("item");
                for (int j = 0; j < elementsByTagName.Count; j++)
                {
                    if (elementsByTagName[j].Attributes["id"] != null && elementsByTagName[j].Attributes["href"] != null)
                    {
                        manifestIDlist.Add(elementsByTagName[j].Attributes["id"].Value);
                        menifestHerflist.Add(elementsByTagName[j].Attributes["href"].Value);
                        try
                        {
                            if (string.IsNullOrEmpty(coverUrltext) && elementsByTagName[j].Attributes["id"].Value.ToLower() == "cover" && File.Exists(directoryName + "\\" + elementsByTagName[j].Attributes["href"].Value))
                            {
                                coverUrltext = this.getFileURL(directoryName, elementsByTagName[j].Attributes["href"].Value);
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch (Exception ex3)
            {
                MessageBox.Show(ex3.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            //设置Cover_Content和Cover_URL
            this.formatCoverURL(coverUrltext);
            #endregion

            #region opf中spine节点中所有节点(顺序阅读目录节点)
            List<string> spineToclist = new List<string>();
            try
            {
                XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("itemref");
                for (int k = 0; k < elementsByTagName.Count; k++)
                {
                    if (elementsByTagName[k].Attributes["idref"] != null && elementsByTagName[k].Attributes["idref"].Value != "")
                    {
                        for (int l = 0; l < manifestIDlist.Count; l++)
                        {
                            if (manifestIDlist[l] == elementsByTagName[k].Attributes["idref"].Value)
                            {
                                spineToclist.Add(menifestHerflist[l]);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex4)
            {
                MessageBox.Show(ex4.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            #endregion

            //从opfmanifest中获取toc.ncx文件名
            string toc = directoryName + "\\toc.ncx";
            int num = manifestIDlist.FindIndex((string s) => s.Equals("ncx", StringComparison.Ordinal));
            if (num >= 0 && File.Exists(directoryName + "\\" + menifestHerflist[num]))
            {
                toc = directoryName + "\\" + menifestHerflist[num];
            }
            //读取NCX目录
            TreeNode = new TreeNode(this.EpubTitle, 0, 0);
            this.readTocNCX(TreeNode, directoryName, toc);

            #region TreeNode子节点内插入reference下内容
            try
            {
                if (referenceHreflist.Count > 0)
                {
                    for (int m = 0; m < referenceHreflist.Count; m++)
                    {
                        bool flag = false;
                        string fileURL = this.getFileURL(directoryName, referenceHreflist[m]);
                        for (int n = 0; n < TreeNode.Nodes.Count; n++)
                        {
                            if (this.equalsFileURL(TreeNode.Nodes[n].Name, fileURL))
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag && !string.IsNullOrEmpty(fileURL) && !this.equalsFileURL(fileURL, coverUrltext))
                        {
                            string referenceTitleText = referenceTitlelist[m];
                            if (string.IsNullOrEmpty(referenceTitleText))
                            {
                                referenceTitleText = "[untitled]";
                            }
                            TreeNode.Nodes.Insert(0, fileURL, referenceTitleText, 2, 3);
                        }
                    }
                }
            }
            catch
            {
            }
            #endregion
            #region TreeNode子节点没有封面的话，就把TreeNode节点链接设为封面链接
            try
            {
                if (!string.IsNullOrEmpty(coverUrltext) && !this.equalsFileURL(TreeNode.Name, coverUrltext))
                {
                    bool flag2 = false;
                    for (int num2 = 0; num2 < TreeNode.Nodes.Count; num2++)
                    {
                        if (this.equalsFileURL(TreeNode.Nodes[num2].Name, coverUrltext))
                        {
                            flag2 = true;
                            break;
                        }
                    }
                    if (!flag2)
                    {
                        TreeNode.Name = coverUrltext;
                    }
                }
            }
            catch
            {
            }
            #endregion
            #region 如果没有toc.ncx，将顺序阅读目录加入TreeNode
            if (!File.Exists(toc))
            {
                IsSpine = true;
                for (int num3 = 0; num3 < spineToclist.Count; num3++)
                {
                    bool flag3 = false;
                    string fileURL2 = this.getFileURL(directoryName, spineToclist[num3]);
                    for (int num4 = 0; num4 < TreeNode.Nodes.Count; num4++)
                    {
                        if (this.equalsFileURL(TreeNode.Nodes[num4].Name, fileURL2))
                        {
                            flag3 = true;
                            break;
                        }
                    }
                    if (!flag3 && !string.IsNullOrEmpty(fileURL2) && !this.equalsFileURL(coverUrltext, fileURL2))
                    {
                        string text6 = spineToclist[num3];
                        text6 = Path.GetFileNameWithoutExtension(text6);
                        if (string.IsNullOrEmpty(text6))
                        {
                            text6 = "[untitled]";
                        }
                        TreeNode.Nodes.Add(fileURL2, text6, 2, 3);
                    }
                }
            }
            #endregion
            this.EpubInformation = "Title: " + this.EpubTitle + "\n" + this.EpubInformation;
        }
        protected void readTocNCX(TreeNode content_Node, string contant_Path, string toc)
        {
            if (!File.Exists(toc))
            {
                return;
            }
            XmlDocument xmlDocument = new XmlDocument();
            XmlNodeList elementsByTagName;
            try
            {
                xmlDocument.XmlResolver = null;
                xmlDocument.Load(toc);
                elementsByTagName = xmlDocument.GetElementsByTagName("docTitle");
                if (elementsByTagName.Count > 0)
                {
                    XmlNode xmlNode = elementsByTagName[0]["text"];
                    if (xmlNode != null && xmlNode.InnerText != "" && this.EpubTitle != xmlNode.InnerText)
                    {
                        this.EpubTitle = xmlNode.InnerText;
                        if (TreeNode.Nodes.Count > 0)
                        {
                            content_Node.Text = this.EpubTitle;
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("This book seem be locked by DRM. The file toc.ncx (table of contents navigation and control) cannot be opened.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }


            XmlNode navRootNode = xmlDocument.DocumentElement["navMap"];
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDocument.NameTable);
            nsmgr.AddNamespace("ncx", xmlDocument.DocumentElement.NamespaceURI);

            foreach (XmlNode node in navRootNode.SelectNodes("ncx:navPoint", nsmgr))
            {
                AddNavPoint(node, content_Node, contant_Path, nsmgr);
            }

        }

        /// <summary>
        /// 把node节点内的navpoint按层次加入treenode。ps:树节点图像0:文件夹1:打开的文件夹2:文件
        /// </summary>
        /// <param name="node">navpoint节点</param>
        /// <param name="treeNode">将navpoint节点加入treepoint</param>
        /// <param name="file_Path">contant内链接文件的路径</param>
        private void AddNavPoint(XmlNode node, TreeNode treeNode, string file_Path, XmlNamespaceManager nsmgr)
        {
            XmlNode navText = node["navLabel"]["text"];
            XmlNode navContent = node["content"];
            string text = navText.InnerText;
            string url = getFileURL(file_Path, navContent.Attributes["src"].Value);
            XmlNodeList childNavNodes = node.SelectNodes("ncx:navPoint", nsmgr);
            if (childNavNodes.Count > 0)
            {
                if (navText != null && navContent != null)
                {
                    if (!string.IsNullOrEmpty(url))
                    {
                        TreeNode currentTreeNode=treeNode.Nodes.Add(url, text,0,0);
                        foreach (XmlNode n in childNavNodes)
                            AddNavPoint(n, currentTreeNode, file_Path, nsmgr);
                    }
                }
            }
            else
            {
                if (navText != null && navContent != null)
                {
                    if (!string.IsNullOrEmpty(url))
                    {
                        treeNode.Nodes.Add(url, text, 2, 3);
                    }
                }
            }
        }

        private bool extractEPUBFile(string filename)
        {
            this.StateMsg = "Opening...";
            bool result;
            try
            {
                this.Temp_Path = this.GetTempDirectory();
                ZipFile zipFile = ZipFile.Read(filename);
                try
                {
                    zipFile.ExtractAll(this.Temp_Path);
                    result = true;
                }
                catch
                {
                    result = false;
                }
            }
            catch
            {
                result = false;
            }
            finally
            {
                this.StateMsg = null;
            }
            return result;
        }
        private bool readyEPUBFile()
        {
            if (this.EpubFilename == "")
            {
                MessageBox.Show("Please open a EPUB file.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            return true;
        }

    }
}
