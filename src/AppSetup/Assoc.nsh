/****************************

�÷�:
* ���������Ķ�!�����Ҫ���ⶫ���Ļ�:-p

1. ����Ľű���ͷ����ͷ�ļ�
	!include "Assoc.nsh"

	������Ĺ�������,����Ϊ�˱����Ƿ�����Լ����豸�ݵ�����ֵ,���ɿ�!
	!define Project ��������
	��
	!define Project MPlayerc

	�����Ҫ����ÿ�������Ķ���(����DetailPrint��ʾ)��,�ڽű���ͷ����������:
	!define Echo ""

	�����Ҫ����ÿ��ȡ�������Ķ���(����DetailPrint��ʾ)��,�ڽű���ͷ����������:
	!define UnEcho ""

	��ǿ����:��д�ù���������Զ����ȡ�������Ĵ���,�ڽű���ͷ����������:
	!define EchoUnSources ""

2. ��Section����Function������Ҫ�ĵط�����:

	# Ҫ������ͨ�ļ���:
	!macro Assoc �ļ���չ���б� �ļ����� ���� �򿪷�ʽ Ĭ��ͼ�� ��������
	# ����:
	!insertmacro Assoc "jpg" "jpgfile" "JPEGͼ��" "$INSTDIR\ACDSee.exe" "$INSTDIR\Icon.ico"
	!insertmacro Assoc "rar,zip,7z,gz" "rarfile" "WinRAR ѹ���ļ�" "$INSTDIR\WinRAR.exe" "$INSTDIR\Icon.ico"

	# Ҫ����һ��ý���ļ���:
	!macro Assoc_Media �ļ���չ���б� �ļ����� ���� �򿪷�ʽ Ĭ��ͼ�� �������� ����CLSID
	# ����:
	!insertmacro Assoc_Media "rmvb" "rmvbfile" "Real ý���ļ� "$INSTDIR\KMPlayer.exe" "$INSTDIR\Icon.ico" "video/vnd.rn-realmedia" "{CFCDAA03-8BE4-11CF-B84B-0020AFBBCCFA}"
	!insertmacro Assoc_Media "rmvb,rm,ra,rv" "rmvbfile" "Real ý���ļ� "$INSTDIR\KMPlayer.exe" "$INSTDIR\Icon.ico" "video/vnd.rn-realmedia" "{CFCDAA03-8BE4-11CF-B84B-0020AFBBCCFA}"

	ע:�ļ������б��ö���","�ָ�.

	# ȡ�������ļ���:
	!macro UnAssoc �ļ���չ���б�
	# ����:
	!insertmacro UnAssoc "jpg"
	!insertmacro UnAssoc "rmvb,rm,ra,rv"

	ע:�ļ������б��ö���","�ָ�.

3.���й���������Ϻ��һ��ˢ������:
	System::Call 'Shell32::SHChangeNotify(i 0x8000000, i 0, i 0, i 0)'
	��������ʾ����.

4.CheckSection:��鵱ǰ������
�÷�����:
	1.  ������ͷ��û!include "Assoc.nsh",û�оͼ���.

	2.	����Ĵ��뿪ͷ��InstType��һ��:
			InstType ��ǰ����
		�Ա����,Ȼ�����"InstType ��ǰ�������ļ�����"�����ǵڼ���(��1��ʼ��),��
		    InstType �������и�ʽ�ļ�
			InstType ����������Ƶ�ļ�
			InstType ����������Ƶ�ļ�
			InstType ȡ�����������ļ�
			InstType ��ǰ�������ļ�����-----�����ǵ�5��

	3.  ��"InstType ��ǰ����"�ǵ�3��,�ͽ���ͷ�ļ������!macro CheckSection ���IntOp $0 $0 | 16���и�Ϊ"IntOp $0 $0 | 4",���ǽ�$0�Ķ�����ֵ��4�Ķ�����ֵ100,���ǽ�������InstType��1,ѡ����.
		��"InstType ��ǰ����"�ǵ�4��,�ͽ���ͷ�ļ������!macro CheckSection ���IntOp $0 $0 | 16���и�Ϊ"IntOp $0 $0 | 8",8���Ƕ�����1000,����ѡ��1000�ĵ�4��.
		���ǵ�5�о���16(10000),�����о���32(100000)....

		Ȼ���ٸ�"IntOp $0 $0 & 31"����,���"InstType ��ǰ����"�ǵ�3�о͸�Ϊ"IntOp $0 $0 & 3",����$0 &(�߼���ķ���) 011,��������InstType����,��ǰ�����Ǹ���0ȡ��ѡ��
		�ǵ�4�о͸�Ϊ"IntOp $0 $0 & 7"(7����0111);��5�о͸�Ϊ"IntOp $0 $0 & 15"(15����01111);��6�о͸�Ϊ"IntOp $0 $0 & 31"

	4.  ������Ҫ����Section���ڿ�ͷ��"Section "Real ��Ƶ" REAL_V"�ĵڶ�����REAL_V,��Ϊ${REAL_V}��ΪCheckSection��ĵ�һ������

	5.  Ȼ��Ҫ������չ���б���(rmvb,rm,rmx,rm33j,rms,rv,rvx)��Ϊ�ڶ�����,����˳�������������κ�һ����չ���ͻ���Ϊ
		���������Section,�ͻṴ�����Section.

	6.  Ȼ����Function .onInit���μ����������:
	*****************************
	ReadRegStr $0 HKCR "Back.${Project}" ""
	StrCmp $0 "" check_skip
	InstTypeSetText 4 "��ǰ�������ļ�����"
	!insertmacro CheckSection ${REAL_V} rmvb,rm,rmx,rm33j,rms,rv,rvx    ;�����Ǽ���ļ�rmvb,rm,rmx,rm33j,rms,rv,rvx,Ȼ������Ƿ��� "Section "Real ��Ƶ" REAL_V"

	;....����Ÿ����CheckSection

	SetCurInstType 4
	goto init_end
	check_skip:
	InstTypeSetText 4 ""
	SetCurInstType 0
	init_end:
*****************************/

!ifndef Ass_Str
!define Ass_Str "��(&O)"
!endif
!ifndef Ass_Str_Media
!define Ass_Str_Media "����(&P)"
!endif

!include "logiclib.nsh"


/*********����һ���ļ�*********/
!macro Assoc EXT TYPE DESC OPENEXE ICO
Push $1
Push $2
!ifdef EchoUnSources
DetailPrint '!insertmacro UnAssoc ${ext}'
!endif
;*********����","�Ÿ��ִ��ĺ���*********
Push $R0  ;�����ַ���
Push $R1  ;��ѭ������
Push $R2  ;�ɴ˿�ʼ��ȡ�ַ���
Push $R3  ;�ַ�������
Push $R4  ;�س������ַ�
Push $R5  ;�س��ַ���(���)
StrCpy $R0 ${ext}
StrCpy $R1 1
StrCpy $R2 0
StrLen $R3 $R0
${Do}
    ${Do}
	StrCpy $R4 $R0 1 $R1
	${ifThen} $R1 > $R3 ${|} ${ExitDo} ${|}
	IntOp $R1 $R1 + 1
    ${LoopUntil} $R4 == ','
IntOp $R1 $R1 - 1
IntOp $R6 $R1 - $R2
StrCpy $R5 $R0 $R6 $R2

;�س��ַ������Ϊ$R5�ڴ����
ReadRegStr $1 HKCR "Back.${Project}" ".$R5"
${if} "$1" == ""
	ReadRegStr $2 HKCR ".$R5" ""
	${if} "$2" == ""
	WriteRegStr HKCR "Back.${Project}" ".$R5" "_Blank_"
	${Else}
	WriteRegStr HKCR "Back.${Project}" ".$R5" "$2"   ;���ݸ���չ��
	${EndIf}
${EndIf}
DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.${ext}" ;ɾ���ɹ���
DeleteRegKey HKCR ".$R5" ;ɾ���ɹ���
!ifdef Echo
DetailPrint '���ڹ���*.$R5�ļ�����Ϊ"${desc}"...'
!endif
WriteRegStr HKCR ".$R5" "" "${Project}.${type}"
;�س��ַ������Ϊ$R5�ڴ����-���

IntOp $R1 $R1 + 2
IntOp $R2 $R1 - 1
${LoopUntil} $R1 > $R3

Pop $R5
Pop $R4
Pop $R3
Pop $R2
Pop $R1
Pop $R0
;*********����","�Ÿ��ִ��ĺ������*********

WriteRegStr HKCR ".${ext}" "" "${Project}.${type}"
WriteRegStr HKCR "${Project}.${type}" "" "${desc}"
WriteRegStr HKCR "${Project}.${type}\shell" "" open
WriteRegStr HKCR "${Project}.${type}\shell\open" "" "${Ass_Str}"
WriteRegStr HKCR "${Project}.${type}\shell\open\command" "" '${openexe} "%1"'
${if} "${ico}" == "" ;�����ָ��ͼ����ʹ��Ĭ��ͼ��
	WriteRegStr HKCR "${Project}.${type}\DefaultIcon" "" "${openexe}"
${Else}
	WriteRegStr HKCR "${Project}.${type}\DefaultIcon" "" "${ico}"
${EndIf}
WriteRegStr HKCR "Back.${Project}" "" "1"  ;����й����ļ�
Pop $1
Pop $2
!macroend

/*********����ý���ļ�*********/
!macro Assoc_Media EXT TYPE DESC OPENEXE ICO CONTENTTYPE CLSID
Push $1
Push $2
!ifdef EchoUnSources
DetailPrint '!insertmacro UnAssoc ${ext}'
!endif
;*********����","�Ÿ��ִ��ĺ���*********
Push $R0  ;�����ַ���
Push $R1  ;��ѭ������
Push $R2  ;�ɴ˿�ʼ��ȡ�ַ���
Push $R3  ;�ַ�������
Push $R4  ;�س������ַ�
Push $R5  ;�س��ַ���(���)
StrCpy $R0 ${ext}
StrCpy $R1 1
StrCpy $R2 0
StrLen $R3 $R0
${Do}
    ${Do}
	StrCpy $R4 $R0 1 $R1
	${ifThen} $R1 > $R3 ${|} ${ExitDo} ${|}
	IntOp $R1 $R1 + 1
    ${LoopUntil} $R4 == ','
IntOp $R1 $R1 - 1
IntOp $R6 $R1 - $R2
StrCpy $R5 $R0 $R6 $R2

;�س��ַ������Ϊ$R5�ڴ����
ReadRegStr $1 HKCR "Back.${Project}" ".$R5"
${if} "$1" == ""
	ReadRegStr $2 HKCR ".$R5" ""
	${if} "$2" == ""
	WriteRegStr HKCR "Back.${Project}" ".$R5" "_Blank_"
	${Else}
	WriteRegStr HKCR "Back.${Project}" ".$R5" "$2"   ;���ݸ���չ��
	${EndIf}
${EndIf}
DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.${ext}" ;ɾ���ɹ���
DeleteRegKey HKCR ".$R5" ;ɾ���ɹ���
!ifdef Echo
DetailPrint '���ڹ���*.$R5�ļ�����Ϊ"${desc}"...'
!endif
WriteRegStr HKCR ".$R5" "" "${Project}.${type}"
;�س��ַ������Ϊ$R5�ڴ����-���

IntOp $R1 $R1 + 2
IntOp $R2 $R1 - 1
${LoopUntil} $R1 > $R3

Pop $R5
Pop $R4
Pop $R3
Pop $R2
Pop $R1
Pop $R0
;*********����","�Ÿ��ִ��ĺ������*********

WriteRegStr HKCR "${Project}.${type}" "" "${desc}"
WriteRegStr HKCR "${Project}.${type}\shell" "" open
WriteRegStr HKCR "${Project}.${type}\shell\open" "" "${Ass_Str_Media}"
WriteRegStr HKCR "${Project}.${type}\shell\open\command" "" '${openexe} "%1"'
${if} "${ico}" == "" ;�����ָ��ͼ����ʹ��Ĭ��ͼ��
	WriteRegStr HKCR "${Project}.${type}\DefaultIcon" "" "${openexe}"
	${Else}
	WriteRegStr HKCR "${Project}.${type}\DefaultIcon" "" "${ico}"
${EndIf}

${if} "${ContentType}" != ""
	WriteRegStr HKCR "${Project}.${type}" "Content Type" "${ContentType}"
	WriteRegStr HKCR "MIME\Database\Content Type\${ContentType}" "Extension" ".${ext}"
	WriteRegStr HKCR "MIME\Database\Content Type\${ContentType}" "CLSID" "${CLSID}"
${EndIf}
WriteRegStr HKCR "Back.${Project}" "" "1"  ;����й����ļ�
Pop $1
Pop $2
!macroend

/*********ɾ���ļ�����*********/
!macro UnAssoc EXT

Push $1  ;${Project}.Back
Push $2  ;type
Push $3  ;Content Type
Push $4  ;${Project}���ֳ���
Push $5  ;type��$4�غ�

StrLen $4 ${Project}
;*********����","�Ÿ��ִ��ĺ���*********
Push $R0  ;�����ַ���
Push $R1  ;��ѭ������
Push $R2  ;�ɴ˿�ʼ��ȡ�ַ���
Push $R3  ;�ַ�������
Push $R4  ;�س������ַ�
Push $R5  ;�س��ַ���(���)
StrCpy $R0 ${ext}
StrCpy $R1 1
StrCpy $R2 0
StrLen $R3 $R0
${Do}
    ${Do}
	StrCpy $R4 $R0 1 $R1
	${ifThen} $R1 > $R3 ${|} ${ExitDo} ${|}
	IntOp $R1 $R1 + 1
    ${LoopUntil} $R4 == ','
IntOp $R1 $R1 - 1
IntOp $R6 $R1 - $R2
StrCpy $R5 $R0 $R6 $R2

;**********�޸����´���Ҫ����!**********
;�س��ַ������Ϊ$R5�ڴ����
ReadRegStr $1 HKCR "Back.${Project}" ".$R5"  ;������
ReadRegStr $2 HKCR ".$R5" ""  ;�����ڵ�
StrCpy $5 $2 $4
${if} "$1" == ""
	!ifdef UnEcho
	DetailPrint '�˳���û�й���*.$R5,����Ҫȡ��...'
	!endif
${Else}
	!ifdef UnEcho
	DetailPrint '����ȡ���ļ�����*.$R5�Ĺ���...'
	!endif
	ReadRegStr $3 HKCR "$2" "Content Type"  ;����û��������
	${if} "$5" == ${Project}
		;���${if}��ⲻ��ȱ��,��һ���Ҿ���û�м���,���Խ�������չ����ע����ɾ����,Ҫ��װ����,��!������װ����-_-!
		DeleteRegValue HKCR ".$R5" "" ;ɾ����չ������(�޸Ĵ˴�Ҫ����!)
		DeleteRegKey /ifempty HKCR ".$R5" ;ɾ����չ������(�޸Ĵ˴�Ҫ����!)
		DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.$R5" ;ɾ������(�޸Ĵ˴�Ҫ����!)
		DeleteRegKey HKCR "$2" ;ɾ����չ���͹���(�޸Ĵ˴�Ҫ����!)
		DeleteRegKey HKCR "MIME\Database\Content Type\$3" ;ɾ��MIME����(�޸Ĵ˴�Ҫ����!)
	${EndIf}
	!ifdef Backup
		${if} "$1" == "_Blank_"  ;���������"_Blank_",֤���Ѿ�����,��û�о�����.
			WriteRegStr HKCR ".$R5" "" ""
		${Else}
			WriteRegStr HKCR ".$R5" "" "$1"
		${EndIf}
	!endif
	DeleteRegValue HKCR "Back.${Project}" ".$R5"



${EndIf}
;**********�޸����ϴ���Ҫ����!**********

;�س��ַ������Ϊ$R5�ڴ����-���

IntOp $R1 $R1 + 2
IntOp $R2 $R1 - 1
${LoopUntil} $R1 > $R3

Pop $R5
Pop $R4
Pop $R3
Pop $R2
Pop $R1
Pop $R0
;*********����","�Ÿ��ִ��ĺ������*********
Pop $5
Pop $4
Pop $3
Pop $2
Pop $1

!macroend

!macro CheckSection SECTION_NAME EXT
Push $1  ;${Project}���ֳ���
Push $2  ;��ȷ�����Ĵ���
StrLen $1 ${Project}
StrCpy $2 0
;*********����","�Ÿ��ִ��ĺ���*********
Push $R0  ;�����ַ���
Push $R1  ;��ѭ������
Push $R2  ;�ɴ˿�ʼ��ȡ�ַ���
Push $R3  ;�ַ�������
Push $R4  ;�س������ַ�
Push $R5  ;�س��ַ���(���)
StrCpy $R0 ${ext}
StrCpy $R1 1
StrCpy $R2 0
StrLen $R3 $R0
${Do}
    ${Do}
	StrCpy $R4 $R0 1 $R1
	${ifThen} $R1 > $R3 ${|} ${ExitDo} ${|}
	IntOp $R1 $R1 + 1
    ${LoopUntil} $R4 == ','
IntOp $R1 $R1 - 1
IntOp $R6 $R1 - $R2
StrCpy $R5 $R0 $R6 $R2
;**********�޸����´���Ҫ����!**********
;�س��ַ������Ϊ$R5�ڴ����
Push $0
ReadRegStr $0 HKCR ".$R5" ""
StrCpy $0 $0 $1    ;-----------����.${EXT}��ȡ${Project}��������˼.
StrCmp $0 ${Project} +1 +2
IntOp $2 $2 + 1
;**********�޸����ϴ���Ҫ����!**********

;�س��ַ������Ϊ$R5�ڴ����-���

IntOp $R1 $R1 + 2
IntOp $R2 $R1 - 1
${LoopUntil} $R1 > $R3
Pop $R5
Pop $R4
Pop $R3
Pop $R2
Pop $R1
Pop $R0
;*********����","�Ÿ��ִ��ĺ������*********
StrCmp $2 0 +5
SectionGetInstTypes "${SECTION_NAME}" $0
IntOp $0 $0 | 16    	 ;-------------------------Ҫ������,���忴˵��
SectionSetInstTypes "${SECTION_NAME}" $0
Goto +4
SectionGetInstTypes "${SECTION_NAME}" $0
IntOp $0 $0 & 31    	 ;-------------------------Ҫ������,���忴˵��
SectionSetInstTypes "${SECTION_NAME}" $0
Pop $2
Pop $1
Pop $0
!macroend

