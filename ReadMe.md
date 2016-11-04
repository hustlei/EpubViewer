# EpubViewer功能

可以打开Epub文件，并且可以同时打开多个文档，支持多标签阅读，支持搜索，包括章节搜索和文章内文字搜索。支持打印。支持根据当前浏览章节同步找到目录相应标签位置。


# 编译

+ 编译前准备
    - 把libraries目录下4个dll文件复制到程序生成目标目录(bin或bind)的common目录下。
    - 把libraries目录下的文件夹复制到程序生成目标目录(bin或bind)下

    bin对应release，bind对应debug

+ 如果用Visual Studio调试，请把App.config.debug改名为App.config，用sharpdevelop调试则不需要，发布软件也不需要。发布软件请删除app.exe.config文件


# License

GNU GENERAL PUBLIC LICENSE Version 2