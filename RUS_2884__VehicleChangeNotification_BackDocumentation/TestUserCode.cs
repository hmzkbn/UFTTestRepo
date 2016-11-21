using System.Globalization;
	namespace Script
{
    using System;
    using System.Xml;
    using System.Xml.Schema;
    using HP.ST.Ext.BasicActivities;
    using HP.ST.Fwk.RunTimeFWK;
    using HP.ST.Fwk.RunTimeFWK.ActivityFWK;
    using HP.ST.Fwk.RunTimeFWK.Utilities;
    using HP.ST.Fwk.RunTimeFWK.CompositeActivities;
	using HP.ST.Ext.CustomDataProviders.Extensions;
	using HP.ST.Ext.CustomDataProviders.ExcelFileArguments;
	using System.Windows.Forms;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Threading.Tasks;
	using System.Data.Odbc;
    using System.Data;
	using System.IO;    
    using System.Collections.Generic;
	using System.IO.Compression;
	using System.Text;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;
	using Microsoft.CSharp;
    using System.Dynamic;
   
    
	
	
	
	
	
    [Serializable()]
    public class TestUserCode : TestEntities
    {
    		
    		
		  		
    	
    	public void CodeActivity17_OnExecuteEvent(object sender, STActivityBaseEventArgs args)
    	{
    		string input = this.CodeActivity17.Input.InputXML;
    		if(!string.IsNullOrWhiteSpace(input))
    			this.CodeActivity17.Output.OutputEscapedXML = System.Security.SecurityElement.Escape(input);
			
        	
       	}
    	
    	public string blobDecompressor (string env, string query_str, string dbConnString)
    	{
    		string environment = env;
			string queryString = query_str;
			OdbcConnection conn = new OdbcConnection();
			conn.ConnectionString = dbConnString;
			if(!string.IsNullOrWhiteSpace(query_str))
			{
				try
				{
				conn.Open();
				  
				    using (OdbcCommand com = new OdbcCommand(queryString, conn))
				    {
				        using (OdbcDataReader reader = com.ExecuteReader())
				        {
				        	// Size of the BLOB buffer.
								int bufferSize = 100;                   
								// The BLOB byte[] buffer to be filled by GetBytes.
								byte[] outByte = new byte[bufferSize];
							while (reader.Read())
							{
								using (var fs2 = new StreamReader(reader.GetStream(0)))
								{
									using (var br = new BinaryReader(fs2.BaseStream))
									{
										var len = (int)br.BaseStream.Length;
										var encrypted = br.ReadBytes(len);
										// decrypt here
										var decrypted = encrypted; // <== new result after decryption
										using (var ms = new MemoryStream(decrypted))
										{
											List<byte> bytesList = new List<byte>();
											using (var decompress = new GZipStream(ms, CompressionMode.Decompress, true))
											{
												int val = decompress.ReadByte();
												while (val > -1)
												{
													bytesList.Add((byte)val);
													val = decompress.ReadByte();
												}  
											}
											var final_result = new String(Encoding.UTF8.GetChars(bytesList.ToArray()));
											return final_result;
											//System.IO.File.WriteAllText(@"C:\vehiclechangenotification_430998.xml", final_result);
										}
									}
								}
				    		}
						}
					}
				}
				catch (Exception ex)
				{
					//throw new Exception("Error with Blob Decompressor! \r\n" + ex.Message);
					MessageBox.Show(ex.Message);
	    			return "";
	    			
				}
				finally
				{
				    conn.Close();
				}
			}
			MessageBox.Show("gggg");
				//throw new Exception("Error with Blob Decompressor! \r\n" + "Query String is empty!");
			return "";
			
    	}
    		
    		public void CodeActivity26_OnExecuteEvent(object sender, STActivityBaseEventArgs args)
    		{
    			//string VCN_PutTimeStamp = this.CodeActivity26.Input.VCN_REQ_PUT_TIMESTAMP.ToString();
    			string VCN_REQ_UUID = this.CodeActivity26.Input.VCN_REQ_UUID;
    			this.CodeActivity26.Output.Execution_Status = 1;
    			string connectionString = "Driver={IBM DB2 ODBC DRIVER};Database=RUSDB;Hostname=rus-test.e.corpintra.net;Port=50000;Protocol=TCPIP;uid=rusappli;pwd=eC5jEtSm;";
    			string query_string = "SELECT F_CONTENT FROM  RUS.R_MESSAGE WHERE  F_MESSAGE_TYPE LIKE 'BACK_DOCUMENTATION_REQUEST' AND F_MESSAGE_ID-1 = (SELECT F_MESSAGE_ID FROM RUS.R_MESSAGE WHERE F_MESSAGE_UUID LIKE '"+ VCN_REQ_UUID +"')";
    			//decompressedResult contains the content of back_doc_request message 
    			string decompressedResult = blobDecompressor("TEST",query_string,connectionString);
    			if(!string.IsNullOrWhiteSpace(decompressedResult))
    			{
	    			XmlDocument doc = new XmlDocument();
	    			doc.LoadXml(decompressedResult);
	    			XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
					nsmgr.AddNamespace("ns4", "http://rus.daimler.com/messages");
	    			string attrVal = doc.SelectSingleNode("/ns4:redocRequest/@userID", nsmgr).Value;
	    			if(attrVal != "")
		    			if(attrVal == "RVSC-OTA")
		    			{
		    				this.CodeActivity26.Output.Execution_Status = 1;
		    				//MessageBox.Show("The userID in the Back_doc_req has been found:" + attrVal);
		    				this.CodeActivity26.Output.Back_Doc_Req = decompressedResult;
		    				this.CodeActivity26.Output.Comment = "The userID in the Back_doc_req has been found:" + attrVal;
		    			}
		    			else
		    			{
		    				this.CodeActivity26.Output.Execution_Status = 2;
		    				this.CodeActivity26.Output.Comment = "Wrong userId was found in Back_doc_req: "+ attrVal;
		    			}
	    			else
	    			{
		    			this.CodeActivity26.Output.Execution_Status = 2;
	    				this.CodeActivity26.Output.Comment = "userId was not found in Back_doc_req!";
	    			}
    			}

    		}

    		public void CodeActivity28_OnExecuteEvent(object sender, STActivityBaseEventArgs args)
    		{
    			
    			int VCN_Req_STATUS_CODE = this.CodeActivity28.Input.VCN_REQ_STATUS_CODE;
    			if(VCN_Req_STATUS_CODE == 200)
    			{
	    			string VCN_REQ_UUID = this.CodeActivity28.Input.VCN_REQ_UUID;
	    			this.CodeActivity28.Output.UserId_Is_Included = false;
	    			
	    			string connectionString=connectionString = "Driver={IBM DB2 ODBC DRIVER};Database=RUSDB;Hostname=s415vmmt452.detss.corpintra.net;Port=60030;Protocol=TCPIP;Uid=rusappli;Pwd=ip8KHkns;";
	    			//string connectionString = "Driver={IBM DB2 ODBC DRIVER};Database=RUSDB;Hostname=s415mt306.detss.corpintra.net;Port=50000;Protocol=TCPIP;uid=rusadmin;pwd=MndVnlPg;";
	    			
	    			string query_string = "SELECT F_CONTENT FROM  RUS.R_MESSAGE WHERE  F_MESSAGE_TYPE LIKE 'BACK_DOCUMENTATION_REQUEST' AND F_MESSAGE_ID-1 = (SELECT F_MESSAGE_ID FROM RUS.R_MESSAGE WHERE F_MESSAGE_UUID LIKE '"+ VCN_REQ_UUID +"')";

	    			//decompressedResult contains the content of back_doc_request message
    				string decompressedResult = blobDecompressor("DEV",query_string,connectionString);
	    			
	    			//MessageBox.Show(decompressedResult);
	    			XmlDocument doc = new XmlDocument();
	    			doc.LoadXml(decompressedResult);
	    			//XmlNode root = doc.DocumentElement;
	    			XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
					nsmgr.AddNamespace("ns4", "http://rus.daimler.com/messages");
	    			string attrVal = doc.SelectSingleNode("/ns4:redocRequest/@userID", nsmgr).Value;
	    			if(attrVal != "")
		    			if(attrVal == "RVSC-OTA")
		    			{
		    				this.CodeActivity28.Output.UserId_Is_Included = true;
		    				//MessageBox.Show("The userID in the Back_doc_req has been found:" + attrVal);
		    				this.CodeActivity28.Output.Back_Doc_Req = decompressedResult;
		    				this.CodeActivity28.Output.Comment = "The userID in the Back_doc_req has been found:" + attrVal;
		    			}
		    			else
		    			{
		    				this.CodeActivity28.Output.UserId_Is_Included = false;
		    				this.CodeActivity28.Output.Comment = "Wrong userId was found in Back_doc_req: "+ attrVal;
		    			}
	    			else
	    			{
		    			this.CodeActivity28.Output.UserId_Is_Included = false;
	    				this.CodeActivity28.Output.Comment = "userId was not found in Back_doc_req!";
	    			}
    			}
    			else
    			{
    				this.CodeActivity28.Output.UserId_Is_Included = false;
    				this.CodeActivity28.Output.Comment = "VechicleChangedEvent has not been sent successfully, Hence unable to check the Back_Doc_Message";
    			}
    		}

    		public void CodeActivity29_OnExecuteEvent(object sender, STActivityBaseEventArgs args)
    		{
    			this.CodeActivity29.Output.TrackingID = System.Guid.NewGuid().ToString("D");
    			DateTime now = DateTime.UtcNow;
    			//var culture = new CultureInfo("de-DE");
    			string pattern = "dd.MM.yyyy hh.mm.ss.ffff";
    			//now = DateTime.ParseExact( currentDateTime,pattern,CultureInfo.InvariantCulture,DateTimeStyles.None);
    			this.CodeActivity29.Output.DateTime = now.ToString(pattern);
    			//MessageBox.Show(now.ToString(pattern));
    				
    			
				

    		}
    	public void CodeActivity27_OnExecuteEvent(object sender, STActivityBaseEventArgs args)
    	{
    		//TODO: Add your code here...
    	}

    	/// <summary>
    	/// Handler for the CodeActivity51 Activity’s ExecuteEvent event.
    	/// </summary>
    	/// <param name=\"sender\">The activity object that raised the ExecuteEvent event.</param>
    	/// <param name=\"args\">The event arguments passed to the activity.</param>
    	/// Use this.CodeActivity51 to access the CodeActivity51 Activity's context, including input and output properties.
    	public void CodeActivity51_OnExecuteEvent(object sender, STActivityBaseEventArgs args)
    	{
    		//TODO: Add your code here...
    	}
}
}

