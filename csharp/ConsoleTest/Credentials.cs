using System.Dynamic;
using com.maplarge.api;

namespace ConsoleTest
{
  /// <summary>
  /// Put API login credentials here for use
  /// </summary>
  public static class Credentials
  {
    //DEFAULT CREDENTIALS
    const string server = "http://e.maplarge.com/";
    const string user = "USER";
    const string pass = "PASS";
    //int token = 123456789;

    public static string UserName
    {
      get
      {
        return user;
      }
    }

    public static string Password
    {
      get
      {
        return pass;
      }
    }


    public static MapLargeConnector MyConnector()
    {
      //CREATE MAPLARGE CONNECTION WITH USER / PASSWORD
      return new MapLargeConnector(server, user, pass);
    }
  }
}
