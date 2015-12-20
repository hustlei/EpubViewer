/*********************************
 *  ������.NET Framwork��װ����  *
 *********************************/


Function GetNetFrameworkVersion
;��ȡ.Net Framework�汾֧��
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
  DetailPrint "��װ .NET Framework 4.0"
  SetDetailsPrint listonly
  DetailPrint "���ڰ�װ .NET Framework 4.0 ..."
  DetailPrint "��װʱ��ϳ��������ĵȺ� ..."
  SetDetailsPrint none
  ExecWait "FX_CLR\dotNetFx40_Full_x86_x64.exe /q /norestart /ChainingPackage ADMINDEPLOYMENT" $R1
  SetDetailsPrint listonly
  DetailPrint ".NET Framework 4.0 ��װ���"
  setDetailsPrint both
  # ExecWait���û��ָ����������ҳ���Ż�һ����������룬���ߴ��ڴ���ʱ����������ô�����
  # ExecWait���ָ��������������������˳����루û�д���һ��Ϊ0����
  ##IntCmp $R1 0 done iseror
  #iseror:
  #  MessageBox MB_OK ".net framework4.0 ��װʧ�ܣ���������޷��������ֶ���װ.net framwork4.0"
  #done:
  IfErrors 0 +2
	MessageBox MB_OK ".NET Framework 4.0��װʧ�ܡ� \
	$\r$\n�����������Ҫ.NET Framework 4.0���л�����\
  $\r$\n�ڱ������װ��ɺ�������������У�����ԣ�\
	$\r$\n1.�ڰ�װ�ļ�netFXĿ¼���ҵ�.NET Framework 4.0���а�װ;\
	$\r$\n2.��΢��ٷ���վ������.NET Framework 4.0����װ��"
FunctionEnd
