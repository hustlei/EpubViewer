# **Changelog**

## **Ver 1.1.0.1455** -- 2015.12.22
### New Features
+ EpubViewer添加闪屏图片，Views里也有（用于UI调试）

### Bug Fixes
+ 在MainViewModel构造函数里引用IWindowManager参数时，无法启动，添加ImportingConstructor属性解决。
+ 子窗口Owner属性无法绑定，在WindowManager打开窗口时通过settings参数设置。


## **Ver 1.1.0.1450** -- 2015.12.20
### New Features
+ App类添加了一个纯代码实现的方法，当然默认不使用
+ 软件图标资源改到EpubViewer项目内
+ 异步更新UI改为异步线程内更新ViewModel内属性。（属性会在主线程内调用NotifyProperty通知UI，不在需要两个线程切换）
+ 删除Contact模块

### Bug Fixes
+ 解决把caliburn.micro.dll放大common文件夹，程序不能启动的问题（原因App构造函数使用了caliburnmicro的类，clr在调用App构造函数时加载dll，所以必须在App构造函数调用前设置privatepath，比如在Main函数或App的静态构造函数里）。
+ VS无法调试，应该是个bug，因为sharpdevelop可以调试，因此调试时需要把App.config.debug改名为App.config


## **Ver 1.1.0.1435** -- 2015.12.16
### New Features
+ 采用Caliburn Micro框架


## **Ver 1.1.0.1430** -- 2015.12.15
### New Features
+ 增加设置对话框，可以设置关联文件到本软件及取消关联，关联epub文件图标
+ 把图标单独放在rc4net.dll里
+ 工具栏增加打开文件、关闭文件按钮
+ about按钮调整到工具栏最右方
+ Nsis编写安装文件，默认关联epub文件

### Bug Fixes
+ 解决打开格式不正确的epub文档或非epub文档时程序崩溃的问题。
+ 解决打开失败后tmp缓存文件不会自动删除的问题
+ 修正没有打开文档时点击同步按钮、新标签按钮出错的问题
+ 修正工具栏在xp操作系统里显示溢出工具的问题


## **Ver 1.1.0.1420** -- 2015.12.14
### New Features
+ 增加设置对话框，可以设置关联文件到本软件及取消关联，关联epub文件图标
+ 工具栏增加打开文件、关闭文件按钮
+ about按钮调整到工具栏最右方

## **Ver 1.1.0.1410** -- 2015.12.13
### New Features
+ 异步多线程打开Epub文件更新UI功能由BackGroundWorker更改为Task+CurrentSynchronizationContext

## **Ver 1.1.0.1400** -- 2015.12.12
### Features
+ 版本增加到v1.1
+ 修改About页面版本信息

## **Ver 1.0.0.1240** -- 2015.12.11
### Features
+ 添加拖放打开文件功能

### Bug Fixes
+ @hustlei 

## **Ver 1.0.0.1230** -- 2015.12.10
### Features
+ 重构EpubLib
+ 实现异步打开Epub文件
+ 程序名重EpubHelpViewer修改为EpubViewer

### Bug Fixes
+ @hustlei 修正Close后EpubBookList内未删除文件的问题

## **Ver 1.0.0.100** -- 2015.10.20
### Features
+ 可以同时打开多个Epub文件
+ 可以多标签显示Epub电子书文章
+ 使用Winform WebBrowser控件显示页面
+ 页面内可以搜索文字，并高亮显示
+ 可以根据目录搜索文章
+ 页面内支持放大缩小
+ 支持打印
+ 支持前进后退
+ 可以根据当前页面在目录内选中相应标题

### Info
+ .net framwork版本为4.0
+ 依赖Ionic.Zip库
+ 依赖Microsoft.Windows.Shell库
+ 依赖Xceed.Wpf.Toolkit库