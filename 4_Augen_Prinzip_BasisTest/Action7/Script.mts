Option Explicit
Dim con, con_connection, fieldName, fieldVal, query, query_connection, kamp, colCount, colCount_connection, p, m, n, status, status_expected, rs_connection, rs, env, camp_prop_id
Dim DB_name, host_name, port_id, user, password

Set con_connection=createobject("adodb.connection")
Set rs_connection=createobject("adodb.recordset")
Set con=createobject("adodb.connection")
Set rs=createobject("adodb.recordset")
kamp = Environment("kampagne_name")
env = TestArgs("environment")
camp_prop_id = Environment("camp_prop_id")

With Browser("Remote Vehicle Services")
	.Page("Remote Vehicle Services").Link("Kampagnenübersicht").Click
End With

With Browser("Remote Vehicle Services").Page("connect me")
	.WebEdit("mainForm:daivb_campaign_filter_kampanenname").Set Environment("kampagne_name")
	.WebElement("Filter").Click
	wait(1)
	.WebElement("Name").Click
End With
Browser("Remote Vehicle Services").Page("Dynamische Kampagne").WebElement("Kampagne starten").Click
Browser("Remote Vehicle Services").Page("Dynamische Kampagne_4").WebElement("Ja, Kampagne wirklich").Click

'get expected status from RVS_UI_CAMPAIGN_PROPERTIES
status_expected = Environment("resultState")

wait(20)

'Test for RUS-2677
If TestArgs("story") = "RUS-2677" Then
	If Browser("Remote Vehicle Services").Page("Dynamische Kampagne_7").WebElement("Die Kampagne kann nicht").Exist(0) Then
		reporter.ReportEvent micPass, "Test passed for RUS-2677 - Rule filter check", "done"
	Else
		reporter.ReportEvent micFail, "Test failed for RUS-2677 - Rule filter check", "not done"		
	End If
End If


'get the connection
con_connection.Open "Driver={IBM DB2 ODBC DRIVER};Database=RVSDB01;Hostname=muc-daidb-01.nttdata-emea.com;Port=50000;Protocol=TCPIP;uid=db2;pwd=Sysadm#01;"
query_connection = "select * from RVS_TEST_GLOBAL.RVS_CONFIG_APPL_DB where ENVIRONMENT =" & "'"&env&"'"
rs_connection.open query_connection, con_connection
colCount_connection = rs_connection.fields.count
For p = 0 To colCount_connection-1
	If rs_connection.Fields(p).Name <> "" Then
		fieldName = rs_connection.Fields(p).Name
		fieldVal = rs_connection.Fields(p)
		Select Case fieldName
			Case "DB_NAME": DB_name = fieldVal
			Case "HOST_NAME": host_name = fieldVal
			Case "PORT_ID": port_id = fieldVal
			Case "DB_USER": user = fieldVal
			Case "DB_PASSWORD": password = fieldVal
		End Select
	End If
Next

con.Open "Driver={IBM DB2 ODBC DRIVER};Database="&DB_name&";Hostname="&host_name&";Port="&port_id&";Protocol=TCPIP;uid="&user&";pwd="&password&";"
	query = "SELECT * FROM RUS.R_DYNAMIC_CAMPAIGN WHERE F_CAMPAIGN_NAME = '"&kamp&"'"
	rs.open query, con
	colCount= rs.fields.count
	For m = 0 To colCount - 1
		Select Case m
			Case "DB_NAME": DB_name = fieldVal
			Case "HOST_NAME": host_name = fieldVal
			Case "PORT_ID": port_id = fieldVal
			Case "DB_USER": user = fieldVal
			Case "DB_PASSWORD": password = fieldVal
		End Select		
	Next 
con.Close


If status = status_expected Then
	reporter.ReportEvent micPass, "Campaign started successfully. DB2 status:", status
Else
	reporter.ReportEvent micFail, "Campaign started unsuccessfully. DB2 status:", status
End If


