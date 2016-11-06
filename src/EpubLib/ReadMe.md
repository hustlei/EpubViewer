# EpubLib库简介

EpubLib是一个C#版本的读取Epub文档的类库。基于.NetFramework 4.0，引用了Ionic.Zip库。

它可以读取epub文件，并输出epub文件的文档信息和文档的目录结构和链接，可以用于显示目录和文档。EpubLib读取epub文档后，把epubfile信息在EPUB_Title等属性中，把epubfile存储在treenode树形结构中，treenode可以直接在winform treeview内显示，包括treeview树的图标。treenode每个节点存储了每个页面的标题文本(用于显示目录)和链接(用于显示文件内容)。Epub文档有按目录阅读和按顺序阅读两种模式，在EpubLib读取文件时可以设定读取的是树形目录还是顺序目录，默认读取的是树形目录，当文档内没有树形目录时会自动读取顺序目录。


# EpubLib库结构

EpubLib内仅包含一个类EpubBook，可以读取一个Epub文件，EpubBook类的主要函数和属性有：

+ 方法/函数：
    - OpenFile(string filename)：读取epubfile
	- OpenAsync(string filename)：异步打开epubfile，并且保证TreeNode的操作都是在调用的线程上执行的（只要调用OpenAsync的线程是UI线程，TreeNode就能直接绑定到xaml上）。
    - CloseFile():关闭文档，清空所有属性，删除缓存文档，在程序关闭前必须调用该函数，否则会占用系统临时目录空间。
+ 属性:
    - TreeNode TreeNode:Epub目录树
	    * name属性存储url
		* text属性存储章节/文章名称
        * ImageIndex存储图标序号
    - static ImageList ImageList;按顺序存储文件夹图标、打开的文件夹图标、文件图标
    - bool IsSpine;是否读取顺序阅读目录，默认为否
    - string Filename：文件名称
    - string Title：文件标题
    - string Information：文件信息
	- string StateMsg：过程信息，例如：打开时msg为"Opening..."


# License

GNU GENERAL PUBLIC LICENSE Version 2