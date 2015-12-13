## Changelog

+ **1.0.0.1240 : 2015.12.11**
    - **Features**
	- 打开文件函数改为OpenFile,关闭文件函数改为CloseFile
	- 标题属性改为Title,文档信息属性改为Information
	- 添加公开函数和属性的说明
	- 重构方法和属性名称,名称更加易懂
	- 重构方法和属性名，Public方法和属性全部用Pascal方法命名，非Public方法和属性用Camel法命名

	- **Info**

    - **Bug Fixes**
    - @hustlei 

+ **1.0.0.1230 : 2015.12.10**
    - **Features**
	- 改为只能读取一个Epub文档，如果要读取多个Epub文档，请用多个EpubBook实例
	- TreeNode代替TreeView存储文档目录，更加灵活
	- 把EpubReader类改名为EpubBook类，重构方法和属性名，Public方法和属性全部用Camel法

	- **Info**

    - **Bug Fixes**
    - @hustlei 修正Close后临时文件删除不净的问题

+ **1.0.0.100 : 2015.10.20**
    - **Features**
    - 输出epubfile信息在EPUB_Title等属性中
    - 把epubfile存储在treeview（system.windows.forms名称空间）树形结构中，treeview内的标题文本可以用于显示目录，treeview内的链接可以用于显示文件内容
	- 同一个treeview对象可以打开多个文件，每次打开后文件内容将添加在treeview树内，但是会导致close后临时文件删除不净，不能单独删除某本书。
	- openEPUBFile(string filename)
        * 读取epubfile
	    * 读取的目录结果存在静态属性treeview中
		* 读取的文件标题赋值给EPUB_Title
        * 读取的文件信息赋值给EPUB_Information
    - treeView（静态属性）
        * 按树形结构存储目录标题，每个标题链接，每个标题图标序号
	    * treeview.ImageListIndex默认值:0文件夹，1打开的文件夹,2文件
    - imagelist（静态属性）存放3个图标
	    * 文件夹图标
	    * 打开的文件夹图标
		* 文件图标
    - closeEPUBFile()
        * 清除treeview目录树
	    * 清除EPUB_Title等数据
    - msg
        * 过程信息
	    * 例如：打开时msg为"Opening..."
	    * 没有操作时为null

	- **Info**
	- .net framwork版本为4.0
	- 读取xml文件用的是xmldocument类
	- 依赖Ionic.Zip库

    - **Bug Fixes**
    - @hustlei 