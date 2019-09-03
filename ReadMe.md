# EpubViewer功能

可以打开Epub文件，并且可以同时打开多个文档，支持多标签阅读，支持搜索，包括章节搜索和文章内文字搜索。支持打印。支持根据当前浏览章节同步找到目录相应标签位置。

+ 可以同时打开多个Epub文件
+ 可以多标签显示Epub电子书文章
+ 使用Winform WebBrowser控件显示页面
+ 页面内可以搜索文字，并高亮显示
+ 可以根据目录搜索文章
+ 页面内支持放大缩小
+ 支持打印
+ 支持前进后退
+ 可以根据当前页面在目录内选中相应标题

# Screenshot

<img src="https://github.com/hustlei/EpubViewer/blob/dev/src/Resources/image/screenshot/EpubViewer_v1.1.png" height=300>

# 编译

+ .net framwork版本为4.0
+ 依赖Ionic.Zip库
+ 依赖Microsoft.Windows.Shell库
+ 依赖Xceed.Wpf.Toolkit库
+ cefx86

# 编译

+ 编译前准备
    - 根据libraries目录下的readme文件说明，下载libraries.zip并解压到`EpubViewer/libraries`目录下。
    - 把libraries目录下的所有文件复制到程序生成目标目录(即`EpubViewer/win/bin或EpubViewer/win/bind)`下。

    > bin对应release，bind对应debug

+ 如果用Visual Studio调试，请把App.config.debug改名为App.config，用sharpdevelop调试则不需要，发布软件也不需要。发布软件请删除app.exe.config文件
+ view，和ViewModel要单独手动编译，因为epubviewer不依赖view和ViewModel。
