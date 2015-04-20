using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.maplarge.api;

namespace ConsoleTest {
	class MapLargeSDKTest {
		static void Main(string[] args) {
			
			//DEFAULT CREDENTIALS
			string server = "http://e.maplarge.com/";
			string user = "USER";
			string pass = "PASS";
			//int token = 123456789;

			Dictionary<string, string> paramlist = new Dictionary<string, string>();

			//CREATE MAPLARGE CONNECTION WITH USER / PASSWORD
			APIConnection mlconnPassword = new APIConnection(server, user, pass);

			//CREATE MAPLARGE CONNECTION WITH USER / AUTH TOKEN
			//MapLargeConnector mlconnToken = new MapLargeConnector(server, user, token);

			//CREATE TABLE SYNCHRONOUS (NO WEB CALL)
			paramlist.Add("account", "test");
			paramlist.Add("tablename", "sdktest1");
			paramlist.Add("fileurl", "http://maplarge-data.s3.amazonaws.com/TwitterS3.zip");
			//MapLargeConnector.NO_WEB_CALLS = true;
			string response = mlconnPassword.InvokeAPIRequest("CreateTableSynchronous", paramlist);
			Console.WriteLine(response);
			APIConnection.NO_WEB_CALLS = false;

			//RETRIEVE REMOTE USER AUTH TOKEN 
			response = mlconnPassword.GetRemoteAuthToken(user, pass, "255.255.255.255");
			Console.WriteLine(response);

			//LIST GROUPS
			paramlist.Clear();
			paramlist.Add("account", "test");
			response = mlconnPassword.InvokeAPIRequestPost("ListGroups", paramlist);
			Console.WriteLine(response);

			//CREATE TABLE WITH FILES SYNCHRONOUS
			paramlist.Clear();
			paramlist.Add("account", "test");
			paramlist.Add("tablename", "sdktest2");
			response = mlconnPassword.InvokeAPIRequestPost("CreateTableWithFilesSynchronous", paramlist,
					new string[] { @"C:\MapLarge\Small.csv" });
			Console.WriteLine(response);
		}
	}
}
