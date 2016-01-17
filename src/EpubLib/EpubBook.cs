using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Ionic.Zip;

namespace Lei.Common
{
    public partial class EpubBook
    {
        /// <summary>
        /// Epub电子书目录树，根节点为书名（根节点链接为封面），二级节点含目录和各个章节
        /// 每个节点的name存储url，text存储章节/文章名称
        /// 有子节点的节点的Icon都设置为CollepsedIcon（文件夹图标）
        /// 无子节点的节点Icon设置为PageIcon(未选中的文章图标)
        /// </summary>
        public ItemNode TocNode { get; set; }
        /// <summary>
        /// 一般Epub电子书都有树形目录和顺序阅读两种目录，IsSpine=false则TreeNode读取树形目录，否则TreeNode存储顺序阅读目录；
        /// 在没有树形目录时，会自动读取顺序阅读目录。
        /// </summary>
        public bool IsSpine { get; set; }
        /// <summary>
        /// Epub电子书文件名(含完整路径)
        /// </summary>
        public string Filename = "";
        /// <summary>
        /// Epub电子书标题，也是TreeNode根节点内容
        /// opf文件里的dc:title标签内容，如果没有该标签则为文件名
        /// </summary>
        public string Title = "";//从opf文件读取
        /// <summary>
        /// Epub电子书的信息，如Creater、Subject、Publisher
        /// </summary>
        public string Information = "";//从opf文件读取
        /// <summary>
        /// 操作过程信息，如打开时为"opening..."
        /// </summary>
        public string StateMsg { get; set; }


        //private string Doc_Anchor = "";
        //private List<string> Script_Files = new List<string>();
        //private List<string> XHTML_Files = new List<string>();
        //private List<string> XHTML_Sources = new List<string>();
        //private bool SelectedNode_Flag;
        //private bool Navigated_Flag;

        private string tempPath = "";
        private string coverContent = "";//从opf文件读取
        private string coverUrl = "";//从opf文件读取

        /// <summary>
        /// 静态构造函数，在构造函数之前，设定静态属性ImageList
        /// </summary>
        static EpubBook()
        {
        }
        public EpubBook()
        {
            IsSpine = false;
            StateMsg = "";
        }
        /// <summary>
        /// 打开一个Epub电子书
        /// </summary>
        /// <param name="filename">Epub文档完整路径及文件名</param>
        public bool Open(string filename)
        {
            Close();
            if (!this.extractEpubFile(filename))
            {
                MessageBox.Show("Cannot open file \"" + filename + "\".", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Hand);
                return false;
            }
            this.Filename = filename;
            this.Title = Path.GetFileNameWithoutExtension(filename);
            this.readEPUBFile();
            if (TocNode == null)
            {
                MessageBox.Show("\"" + filename + "\"is not a epub file.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Hand);
                return false;
            }
            if (TocNode.Nodes.Count == 0)
            {
                MessageBox.Show("No book information in file \"" + filename + "\".", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }
            return true;
        }
        /// <summary>
        /// 异步打开一个Epub电子书
        /// 只有在UI线程调用才能直接绑定TocNode，否则建议复制TocNode后绑定，因为被绑定属性必须在UI线程创建
        /// </summary>
        /// <param name="filename">Epub文档完整路径及文件名</param>
        public Task<bool> OpenAsync(string filename)
        {
            Close();
            var t = Task.Factory.StartNew(() =>
            {
                if (!this.extractEpubFile(filename))
                {
                    MessageBox.Show("Cannot open file \"" + filename + "\".", "Error", MessageBoxButtons.OK,
                        MessageBoxIcon.Hand);
                    return false;
                }
                return true;
            });
            return t.ContinueWith(x =>
            {
                if (x.Result)
                {
                    this.Filename = filename;
                    this.Title = Path.GetFileNameWithoutExtension(filename);
                    this.readEPUBFile();
                    if (TocNode == null)
                    {
                        MessageBox.Show("\"" + filename + "\"is not a epub file.", "Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Hand);
                        Close();
                        return false;
                    }
                    if (TocNode.Nodes.Count == 0)
                    {
                        MessageBox.Show("No book information in file \"" + filename + "\".", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        Close();
                        return false;
                    }
                    return true;
                }
                return false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        public void Close()
        {
            this.Filename = "";
            this.Title = "";
            this.Information = "";
            //this.Doc_Anchor = "";
            //this.Script_Files.Clear();
            //this.XHTML_Files.Clear();
            //this.XHTML_Sources.Clear();
            //this.SelectedNode_Flag = false;
            //this.Navigated_Flag = false;
            this.coverContent = "";
            this.coverUrl = "";
            //this.Encoding_Default = "";
            //this.Encoding_URL = "";
            //this.Encoding_Reflesh = false;
            this.TocNode = null;

            try
            {
                if (this.tempPath != "")
                {
                    this.StateMsg = "Closing...";
                    try
                    {
                        if (Directory.Exists(this.tempPath))
                        {
                            Directory.Delete(this.tempPath, true);
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
                        if (Directory.Exists(this.tempPath))
                        {
                            Directory.Delete(this.tempPath, true);
                        }
                    }
                    catch
                    {
                    }
                    this.tempPath = "";
                }
            }
            catch
            {
            }
        }

        private void readEPUBFile()
        {
            if (this.Filename == "")
            {
                return;
            }
            string containerFile = this.tempPath + "\\META-INF\\container.xml";
            if (!File.Exists(containerFile))
            {
                return;
            }
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.XmlResolver = null;
            xmlDocument.Load(containerFile);
            List<string> opfFileList = new List<string>();
            XmlNodeList rootfileElements = xmlDocument.GetElementsByTagName("rootfile");
            for (int i = 0; i < rootfileElements.Count; i++)
            {
                XmlNode xmlNode = rootfileElements[i].Attributes["full-path"];
                if (xmlNode != null && xmlNode.Value != "")
                {
                    string opfFilename = this.tempPath + "\\" + xmlNode.Value.Replace("/", "\\");
                    if (File.Exists(opfFilename))
                    {
                        opfFileList.Add(opfFilename);
                    }
                }
            }
            if (opfFileList.Count == 0 && File.Exists(this.tempPath + "\\content.opf"))
            {
                opfFileList.Add(this.tempPath + "\\content.opf");
            }
            //tree.BeginUpdate();
            //try
            //{
            foreach (string current in opfFileList)
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
            string contentPath = Path.GetDirectoryName(content);//opf,toc等文件目录
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.XmlResolver = null;
                xmlDocument.Load(content);
                XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("dc:title");
                if (elementsByTagName.Count > 0 && elementsByTagName[0].InnerText != "")
                {
                    this.Title = elementsByTagName[0].InnerText;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            #region 读取opf文件内的metadata(设置Infromation)
            try
            {
                XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("dc:creator");
                if (elementsByTagName.Count > 0)
                {
                    this.Information = this.Information + "Creator: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:subject");
                if (elementsByTagName.Count > 0)
                {
                    this.Information = this.Information + "Subject: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:description");
                if (elementsByTagName.Count > 0)
                {
                    this.Information = this.Information + "Description: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:publisher");
                if (elementsByTagName.Count > 0)
                {
                    this.Information = this.Information + "Publisher: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:contributor");
                if (elementsByTagName.Count > 0)
                {
                    this.Information = this.Information + "Contributor: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:date");
                if (elementsByTagName.Count > 0)
                {
                    this.Information = this.Information + "Date: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:type");
                if (elementsByTagName.Count > 0)
                {
                    this.Information = this.Information + "Type: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:format");
                if (elementsByTagName.Count > 0)
                {
                    this.Information = this.Information + "Formate: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:identifier");
                if (elementsByTagName.Count > 0)
                {
                    this.Information = this.Information + "Identifiere: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:source");
                if (elementsByTagName.Count > 0)
                {
                    this.Information = this.Information + "Source: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:language");
                if (elementsByTagName.Count > 0)
                {
                    this.Information = this.Information + "Language: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:relation");
                if (elementsByTagName.Count > 0)
                {
                    this.Information = this.Information + "Relation: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:coverage");
                if (elementsByTagName.Count > 0)
                {
                    this.Information = this.Information + "Coverage: " + elementsByTagName[0].InnerText + "\n";
                }
                elementsByTagName = xmlDocument.GetElementsByTagName("dc:rights");
                if (elementsByTagName.Count > 0)
                {
                    this.Information = this.Information + "Rights: " + elementsByTagName[0].InnerText + "\n";
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
                    if (!string.IsNullOrEmpty(referenceHrefText) && !string.IsNullOrEmpty(referenceTitleText) && !string.IsNullOrEmpty(referenceTypeText) && (referenceTypeText == "cover" || referenceTypeText == "title-page" || referenceTypeText == "toc" || referenceTypeText == "index" || referenceTypeText == "copyright" || referenceTypeText == "copyright-page" || referenceTypeText == "text") && File.Exists(contentPath + "\\" + this.getFileName(referenceHrefText)))
                    {
                        referenceHreflist.Add(referenceHrefText);
                        referenceTitlelist.Add(referenceTitleText);
                        referenceTypelist.Add(referenceTypeText);
                    }
                    if (referenceTypeText == "cover" && !string.IsNullOrEmpty(referenceHrefText) && File.Exists(contentPath + "\\" + this.getFileName(referenceHrefText)))
                    {
                        coverUrltext = this.getFileURL(contentPath, referenceHrefText);
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
                            if (string.IsNullOrEmpty(coverUrltext) && elementsByTagName[j].Attributes["id"].Value.ToLower() == "cover" && File.Exists(contentPath + "\\" + elementsByTagName[j].Attributes["href"].Value))
                            {
                                coverUrltext = this.getFileURL(contentPath, elementsByTagName[j].Attributes["href"].Value);
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
            string toc = contentPath + "\\toc.ncx";
            int num = manifestIDlist.FindIndex((string s) => s.Equals("ncx", StringComparison.Ordinal));
            if (num >= 0 && File.Exists(contentPath + "\\" + menifestHerflist[num]))
            {
                toc = contentPath + "\\" + menifestHerflist[num];
            }
            //读取NCX目录
            TocNode = new ItemNode(this.Title, "", ItemNode.CollepsedIcon);
            if (!IsSpine)
                this.readTocNCX(TocNode, toc);

            #region TreeNode子节点内插入reference下内容
            try
            {
                if (referenceHreflist.Count > 0)
                {
                    for (int m = 0; m < referenceHreflist.Count; m++)
                    {
                        bool flag = false;
                        string fileURL = this.getFileURL(contentPath, referenceHreflist[m]);
                        for (int n = 0; n < TocNode.Nodes.Count; n++)
                        {
                            if (this.equalsFileURL(TocNode.Nodes[n].Name, fileURL))
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
                            TocNode.Nodes.Insert(0, new ItemNode(referenceTitleText, fileURL, ItemNode.PageIcon));
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
                if (!string.IsNullOrEmpty(coverUrltext) && !this.equalsFileURL(TocNode.Name, coverUrltext))
                {
                    bool flag2 = false;
                    for (int num2 = 0; num2 < TocNode.Nodes.Count; num2++)
                    {
                        if (this.equalsFileURL(TocNode.Nodes[num2].Name, coverUrltext))
                        {
                            flag2 = true;
                            break;
                        }
                    }
                    if (!flag2)
                    {
                        TocNode.Name = coverUrltext;
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
                    string fileURL2 = this.getFileURL(contentPath, spineToclist[num3]);
                    for (int num4 = 0; num4 < TocNode.Nodes.Count; num4++)
                    {
                        if (this.equalsFileURL(TocNode.Nodes[num4].Name, fileURL2))
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
                        TocNode.Nodes.Add(new ItemNode(text6, fileURL2, ItemNode.PageIcon));
                    }
                }
            }
            #endregion
            this.Information = "Title: " + this.Title + "\n" + this.Information;
        }
        private void readTocNCX(ItemNode rootNode, string toc)
        {
            if (!File.Exists(toc))
            {
                return;
            }
            XmlDocument ncxXmlDocument = new XmlDocument();
            XmlNodeList docTitleElements;
            try
            {
                ncxXmlDocument.XmlResolver = null;
                ncxXmlDocument.Load(toc);
                docTitleElements = ncxXmlDocument.GetElementsByTagName("docTitle");
                if (docTitleElements.Count > 0)
                {
                    XmlNode xmlNode = docTitleElements[0]["text"];
                    if (xmlNode != null && xmlNode.InnerText != "" && this.Title != xmlNode.InnerText)
                    {
                        this.Title = xmlNode.InnerText;
                        if (TocNode.Nodes.Count > 0)
                        {
                            rootNode.Text = this.Title;
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("This book seem be locked by DRM. The file toc.ncx (table of contents navigation and control) cannot be opened.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            XmlNode navRootNode = ncxXmlDocument.DocumentElement["navMap"];
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(ncxXmlDocument.NameTable);
            nsmgr.AddNamespace("ncx", ncxXmlDocument.DocumentElement.NamespaceURI);

            string contentPath = Path.GetDirectoryName(toc);
            foreach (XmlNode node in navRootNode.SelectNodes("ncx:navPoint", nsmgr))
            {
                AddNavPoint(node, rootNode, contentPath, nsmgr);
            }

        }

        /// <summary>
        /// 把node节点内的navpoint按层次加入treenode。ps:树节点图像0:文件夹1:打开的文件夹2:文件
        /// </summary>
        /// <param name="node">navpoint节点</param>
        /// <param name="treeNode">将navpoint节点加入treepoint</param>
        /// <param name="filePath">contant内链接文件的路径</param>
        private void AddNavPoint(XmlNode node, ItemNode treeNode, string filePath, XmlNamespaceManager nsmgr)
        {
            XmlNode navText = node["navLabel"]["text"];
            XmlNode navContent = node["content"];
            string text = navText.InnerText;
            string url = getFileURL(filePath, navContent.Attributes["src"].Value);
            XmlNodeList childNavNodes = node.SelectNodes("ncx:navPoint", nsmgr);
            if (childNavNodes.Count > 0)
            {
                if (navText != null && navContent != null)
                {
                    if (!string.IsNullOrEmpty(url))
                    {
                        ItemNode currentTreeNode = new ItemNode(text, url, ItemNode.CollepsedIcon);
                        treeNode.Nodes.Add(currentTreeNode);
                        foreach (XmlNode n in childNavNodes)
                            AddNavPoint(n, currentTreeNode, filePath, nsmgr);
                    }
                }
            }
            else
            {
                if (navText != null && navContent != null)
                {
                    if (!string.IsNullOrEmpty(url))
                    {
                        treeNode.Nodes.Add(new ItemNode(text, url, ItemNode.PageIcon));
                    }
                }
            }
        }

        private bool extractEpubFile(string filename)
        {
            this.StateMsg = "Opening...";
            bool result;
            try
            {
                this.tempPath = this.getTempDirectory();
                ZipFile zipFile = ZipFile.Read(filename);
                try
                {
                    zipFile.ExtractAll(this.tempPath);
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
                this.StateMsg = "";
            }
            return result;
        }
        //private bool readyEPUBFile()
        //{
        //    if (this.Filename == "")
        //    {
        //        MessageBox.Show("Please open a EPUB file.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        //        return false;
        //    }
        //    return true;
        //}

    }
}
