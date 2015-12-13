
# Epub电子书格式

## container
Epub电子书为zip格式，解压后META-INF文件夹下有container.xml文件，该文件根标签为container,container子标签为rootfiles,rootfiles内含有一个或多个rootfile标签，一般情况下只有一个。rootfile标签的full-path属性指向一个xxx.opf文件，opf文件包含了所有epub文档的信息。

## opf文件
opf文件有metadata、manifest、spine、guide几个标签。

### metadata
meatadata标签含有Epub文件的信息，如dc:title,dc:creator等。

### manifest
manifest包含所有文件的类型和路径，每个文件为一个item标签。

### spine
按顺序列出顺序阅读的每个文件，每个文件用itemref标签指出引用的文章id。

### guide
包含封面、目录、前言(开始)等文件链接，每个文件用一个reference标签。
