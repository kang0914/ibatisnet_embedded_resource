/*
 배포 전 스크립트 템플릿							
--------------------------------------------------------------------------------------
 이 파일에는 빌드 스크립트 이전에 실행될 SQL 문이 들어 있습니다.	
 SQLCMD 구문을 사용하여 파일을 배포 전 스크립트에 포함합니다.			
 예:      :r .\myfile.sql								
 SQLCMD 구문을 사용하여 배포 전 스크립트의 변수를 참조합니다.		
 예:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
DELETE MM_CODE
INSERT INTO MM_CODE(GROUP_CODE, GROUP_NAME, CODE, NAME, REV_NO, DATA1, DATA2, DATA3, IUSER, IDATE, DUSER, DDATE) VALUES(N'G1', N'쥐1', N'C1', N'코1', 1, NULL, NULL, NULL, N'admin', GETDATE(), NULL, NULL);