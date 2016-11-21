Option Explicit
Dim result, counter, sim_vehicle, sim_counter, con, query, kamp, colCount, m, n, status, status_expected, rs, env, released_by

Set con=createobject("adodb.connection")
Set rs=createobject("adodb.recordset")
kamp = Environment("kampagne_name")
env = TestArgs("environment")


'for RUS-2606
If Browser("Remote Vehicle Services").Page("Dynamische Kampagne").WebElement("Kampagne starten").Exist(0) = False Then @@ hightlight id_;_Browser("Remote Vehicle Services").Page("Dynamische Kampagne 8").Link("Vehicle")_;_script infofile_;_ZIP::ssf2.xml_;_
	reporter.ReportEvent micPass, "Test passed for RUS-2606 - The 'Start campaign' button ", " is disabled"
Else
	reporter.ReportEvent micFail, "Test failed for RUS-2606 - The 'Start campaign' button ", " is nicht disabled"
End If
With Browser("Remote Vehicle Services").Page("Dynamische Kampagne")
	.WebElement("Simulation starten").Click @@ hightlight id_;_Browser("Remote Vehicle Services").Page("Dynamische Kampagne 7").WebButton("Simulation starten")_;_script infofile_;_ZIP::ssf3.xml_;_
	.WebElement("Ja, Simulation starten").Click	
End With	
wait(2)
With Browser("Remote Vehicle Services").Page("Dynamische Kampagne")
	.WebElement("Zurück").Click
	'.WebElement("Ja, Vorgang bestätigen.").Click
End With

With Browser("Remote Vehicle Services").Page("connect me")
	.WebEdit("mainForm:daivb_campaign_filter_kampanenname").Set Environment("kampagne_name")
	.WebElement("Filter").Click
	wait(1)

result = .WebElement("camp_status").GetROProperty("innertext")
sim_vehicle = .WebElement("simulated_vehicle_box").GetROProperty("innertext")

'timeout is built so that the check for simulation status will not go undefinately
sim_counter = 0
While result <> "Simulation Finished"
	wait(5)
	.WebElement("Filter").Click
	result = .WebElement("camp_status").GetROProperty("innertext")
	sim_vehicle = .WebElement("simulated_vehicle_box").GetROProperty("innertext")
	If sim_counter = 10 Then
		If sim_vehicle = "Timeout" Then
			reporter.ReportEvent micPass, "Timeout. Test passed for RUS-2620. Status: ", sim_vehicle
			ExitRun	
		ElseIf sim_vehicle > 0 Then 
		Else
			reporter.ReportEvent micFail, "Simulation failed", "sim_vehicle"
			ExitRun			
		End If	
	End If
	sim_counter = sim_counter + 1
Wend

'for RUS-2942

If sim_vehicle = "not simulated" Then
	reporter.ReportEvent micFail, "Test fail for RUS-2942. Simulated status is", sim_vehicle
Else
	reporter.ReportEvent micPass, "Test passed for RUS-2942. Simulated status is", sim_vehicle
	'ExitRun
End If

If result = "Simulation Finished" Then
	.WebElement("Name").Click
	wait(2)
	Browser("Remote Vehicle Services").Page("Dynamische Kampagne").WebElement("Kampagne freigeben").Click
	wait(3)
	Browser("Remote Vehicle Services").Page("Dynamic Campaign_9").WebElement("Yes, set the campaign").Click
	wait(5)
	'for RUS-3171
	released_by = Browser("Remote Vehicle Services").Page("Dynamische Kampagne").WebElement("released by").GetROProperty("innertext")
	If released_by = TestArgs("user") Then
		reporter.ReportEvent micPass, "Test passed: RUS-3171. ", released_by& " is shown in the campaign"
	Else
		reporter.ReportEvent micFail, "Test failed: RUS-3171. ", released_by& " is shown in the campaign"	
	End If
	If TestArgs("story") = "RUS-3109" Then
		If Browser("Remote Vehicle Services").Page("Dynamische Kampagne").WebElement("Kampagne starten").Exist(5) Then
			reporter.ReportEvent micPass, "Test passed: RUS-3109. ", "The campaign starter can start the campaign"
		Else
			reporter.ReportEvent micFail, "Test failed: RUS-3109. ", "The campaign starter cannot start the campaign"	
		End If
	End If
	'bewusst nur mit RUSCS01 und nicht mit RVSADMIN oder RUSADMIN
	If TestArgs("user") = "RUSCS01" Then
		    Browser("Remote Vehicle Services").Page("Dynamische Kampagne").WebElement("Kampagne starten").Click
			Browser("Remote Vehicle Services").Page("Dynamische Kampagne_4").WebElement("Ja, Kampagne wirklich").Click
			
			'get expected status from RVS_UI_CAMPAIGN_PROPERTIES
			status_expected = Environment("resultState")
			wait(20)
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
	End If
	Browser("Remote Vehicle Services").Page("connect me").Link("Logout").Click @@ hightlight id_;_Browser("Remote Vehicle Services").Page("Daimler - Abgemeldet")_;_script infofile_;_ZIP::ssf3.xml_;_
	Browser("Remote Vehicle Services").CloseAllTabs		 
End If
End With
Environment("user") = TestArgs("user2")

'result = Msgbox("Should the test go on?",1,"Check simulation status")

'If result = 1 Then
'	With Browser("Remote Vehicle Services").Page("connect me")
'		.WebEdit("mainForm:daivb_campaign_filter_kampanenname").Set Environment("kampagne_name")
'		.WebElement("Filter").Click
'		wait(1)
'		.WebElement("Name").Click
'	End With
'	wait(2)
'	Browser("Remote Vehicle Services").Page("Dynamische Kampagne").WebElement("Kampagne freigeben").Click
'	Browser("Remote Vehicle Services").Page("Dynamische Kampagne").WebElement("Ja, Kampagne freigeben").Click
'	wait(2)
'	Browser("Remote Vehicle Services").Page("connect me").Link("Logout").Click @@ hightlight id_;_Browser("Remote Vehicle Services").Page("Daimler - Abgemeldet")_;_script infofile_;_ZIP::ssf3.xml_;_
'	Browser("Remote Vehicle Services").CloseAllTabs	
'	counter = counter + 1
'	Environment("counter") = counter 
'Else
'	reporter.ReportEvent micWarning, "Test quit", "by the tester"
'	ExitRun
'End If

	
