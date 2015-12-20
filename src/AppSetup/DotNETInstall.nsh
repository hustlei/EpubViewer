/*********************************
 *  以下是.NET Framwork安装部分  *
 *********************************/


Function GetNetFrameworkVersion
;获取.Net Framework版本支持
    Push $1
    Push $0
    ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client" "Install"
    ReadRegDWORD $1 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Client" "Version"
    StrCmp $1 "" +1 +2
    StrCpy $1 "4.0.30319"
    StrCmp $0 1 KnowNetFrameworkVersion +1
    ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4.0\Client" "Install"
    ReadRegDWORD $1 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4.0\Client" "Version"
    StrCmp $1 "" +1 +2
    StrCpy $1 "4.0.30319"
    StrCmp $0 1 KnowNetFrameworkVersion +1
    ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" "Install"
    ReadRegDWORD $1 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" "Version"
    StrCmp $1 "" +1 +2
    StrCpy $1 "4.0.30319"
    StrCmp $0 1 KnowNetFrameworkVersion +1
    ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5" "Install"
    ReadRegDWORD $1 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5" "Version"
    StrCmp $1 "" +1 +2
    StrCpy $1 "3.5.30729.4926"
    StrCmp $0 1 KnowNetFrameworkVersion +1
    ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.0\Setup" "InstallSuccess"
    ReadRegDWORD $1 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.0\Setup" "Version"
    StrCmp $1 "" +1 +2
    StrCpy $1 "3.0"
    StrCmp $0 1 KnowNetFrameworkVersion +1
    ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v2.0.50727" "Install"
    ReadRegDWORD $1 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v2.0.50727" "Version"
    StrCmp $1 "" +1 +2
    StrCpy $1 "2.0.50727.832"
    StrCmp $0 1 KnowNetFrameworkVersion +1
    ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v1.1.4322" "Install"
    ReadRegDWORD $1 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v1.1.4322" "Version"
    StrCmp $1 "" +1 +2
    StrCpy $1 "1.1.4322.573"
    StrCmp $0 1 KnowNetFrameworkVersion +1
    ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\.NETFramework\policy\v1.0" "Install"
    ReadRegDWORD $1 HKLM "SOFTWARE\Microsoft\.NETFramework\policy\v1.0" "Version"
    StrCmp $1 "" +1 +2
    StrCpy $1 "1.0.3705.0"
    StrCmp $0 1 KnowNetFrameworkVersion +1
    StrCpy $1 "not .NetFramework"
    KnowNetFrameworkVersion:
    Pop $0
    Exch $1
FunctionEnd

Function InstallFramework4
  ClearErrors
  SetDetailsPrint textonly
  DetailPrint "安装 .NET Framework 4.0"
  SetDetailsPrint listonly
  DetailPrint "正在安装 .NET Framework 4.0 ..."
  DetailPrint "安装时间较长，请耐心等候 ..."
  SetDetailsPrint none
  ExecWait "FX_CLR\dotNetFx40_Full_x86_x64.exe /q /norestart /ChainingPackage ADMINDEPLOYMENT" $R1
  SetDetailsPrint listonly
  DetailPrint ".NET Framework 4.0 安装完成"
  setDetailsPrint both
  # ExecWait如果没有指定输出参数且程序放回一个非零错误码，或者存在错误时，程序会设置错误标记
  # ExecWait如果指定了输出参数，将设置退出代码（没有错误一般为0？）
  ##IntCmp $R1 0 done iseror
  #iseror:
  #  MessageBox MB_OK ".net framework4.0 安装失败，如果程序无法运行请手动安装.net framwork4.0"
  #done:
  IfErrors 0 +2
	MessageBox MB_OK ".NET Framework 4.0安装失败。 \
	$\r$\n此软件运行需要.NET Framework 4.0运行环境。\
  $\r$\n在本软件安装完成后，如果程序不能运行，你可以：\
	$\r$\n1.在安装文件netFX目录下找到.NET Framework 4.0自行安装;\
	$\r$\n2.到微软官方网站上下载.NET Framework 4.0并安装。"
FunctionEnd
