using System;
using System.Collections.Generic;
using com.maplarge.api;

namespace ConsoleTest
{
  public static class MapLargeSDKTest
  {
    private static void Main(string[] args)
    {
      var connector = Credentials.MyConnector();
      var paramlist = new Dictionary<string, string>
      {
        {"account", "test"},
        {"tablename", "sdktest1"},
        {"fileurl", "http://maplarge-data.s3.amazonaws.com/TwitterS3.zip"}
      };

      //CREATE TABLE SYNCHRONOUS (NO WEB CALL)
      //MapLargeConnector.NO_WEB_CALLS = true;
      var response = connector.InvokeAPIRequest("CreateTableSynchronous", paramlist);
      Console.WriteLine(response);
      MapLargeConnector.NO_WEB_CALLS = false;

      //RETRIEVE REMOTE USER AUTH TOKEN 
      response = connector.GetRemoteAuthToken(Credentials.UserName, Credentials.Password, "255.255.255.255");
      Console.WriteLine(response);

      //LIST GROUPS
      paramlist.Clear();
      paramlist.Add("account", "test");
      response = connector.InvokeAPIRequestPost("ListGroups", paramlist);
      Console.WriteLine(response);

      //CREATE TABLE WITH FILES SYNCHRONOUS
      paramlist.Clear();
      paramlist.Add("account", "test");
      paramlist.Add("tablename", "sdktest2");
      response = connector.InvokeAPIRequestPost("CreateTableWithFilesSynchronous",
        paramlist,
        new[] {@"C:\MapLarge\Small.csv"});
      Console.WriteLine(response);
    }
  }
}