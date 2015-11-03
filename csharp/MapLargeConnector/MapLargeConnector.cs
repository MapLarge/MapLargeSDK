using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace com.maplarge.api
{
  public class MapLargeConnector
  {
    public const int Version = 2;
    /**
		 * When NO_WEB_CALLS is true all MapLargeConnectors will not make remote
		 * calls. Instead, the response will be the full URL that would have been
		 * invoked.
		 */
    public static bool NO_WEB_CALLS;
    private readonly string _apiserver;
    private readonly string _authstring;
    private readonly string _token;

    private readonly string _user;

    /**********************
		 *    CONSTRUCTORS    *
		 **********************/

    /**
		 * Constructor. Creates a connection to a MapLarge API server with a
		 * username and token as credentials.
		 * 
		 * @param urlApiServer
		 *            URL of API server. Must begin with valid protocol
		 *            (http/https).
		 * @param username
		 *            Username to use for connection credentials.
		 * @param token
		 *            Authentication token to use for connection credentials.
		 */

    public MapLargeConnector(string urlApiServer, string username, int token)
    {
      _apiserver = urlApiServer;
      if (!_apiserver.EndsWith("/"))
        _apiserver += '/';
      _user = username;
      _token = token.ToString();
      _authstring = "mluser=" + _user + "&mltoken=" + _token;
    }

    /**
		 * Constructor. Creates a connection to a MapLarge API server with a
		 * username and token as credentials.
		 * 
		 * @param urlApiServer
		 *            URL of API server. Must begin with valid protocol
		 *            (http/https).
		 * @param username
		 *            Username to use for connection credentials.
		 * @param password
		 *            Authentication token to use for connection credentials.
		 */

    public MapLargeConnector(string urlApiServer, string username, string password)
    {
      _apiserver = urlApiServer;
      if (!_apiserver.EndsWith("/"))
        _apiserver += '/';
      _user = username;
      _token = GetToken(password);
      // if (this._token == "") throw new Exception("Authentication Failed")
      // OR SOMETHING ELSE.
      _authstring = "mluser=" + _user + "&mltoken=" + _token;
    }

    private string GetToken(string pass)
    {
      NO_WEB_CALLS = false;
      var s = InvokeURL("Auth", "Login", "mluser=" + _user + "&mlpass=" + pass);
      s = s.Substring(1, s.Length - 1);
      var vals = s.Split(',');
      var success = false;
      var rettoken = "";
      foreach (var t in vals)
      {
        if (t.StartsWith("\"token\""))
          rettoken = t.Split(':')[1].Replace("\"", "");
        if (t.Equals("\"success\":true"))
          success = true;
      }
      return success ? rettoken : "";
    }

    private string InvokeURL(string controller, string actionname, Dictionary<string, string> paramlist)
    {
      var querystring = _authstring;
      foreach (var kvp in paramlist) querystring += "&" + kvp.Key + "=" + kvp.Value;
      var query = string.Join("&", paramlist.Select(x => string.Format("{0}={1}", x.Key, x.Value)));
      return InvokeURL(controller, actionname, querystring);
    }

    private string InvokeURL(string controller, string actionname, string querystring)
    {
      if (NO_WEB_CALLS)
      {
        return _apiserver + controller + "/" + actionname + "?" + querystring;
      }
      try
      {
        var url = new Uri(_apiserver + controller + "/" + actionname + "?" + querystring);
        var wc = new WebClient();
        var s = wc.DownloadString(url);
        //URLConnection conn = url.openConnection();
        //BufferedReader br = new BufferedReader(new InputStreamReader(conn.getInputStream()));
        //string inputLine;
        //string s = "";
        //while ((inputLine = br.readLine()) != null) s += inputLine;
        //br.close();
        return s;
      }
      catch (UriFormatException ufe)
      {
        Console.WriteLine(ufe.Message);
        Console.WriteLine(ufe.StackTrace);
      }
      catch (IOException ioe)
      {
        Console.WriteLine(ioe.Message);
        Console.WriteLine(ioe.StackTrace);
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        Console.WriteLine(e.StackTrace);
      }

      return "ERROR"; // Temporary Error Code
    }

    private string InvokePostURL(string controller,
      string actionname,
      IDictionary<string, string> paramlist,
      IEnumerable<string> filepaths)
    {
      if (NO_WEB_CALLS) return _apiserver + controller + "/" + actionname;
      var boundary = "===" + DateTime.Now.Ticks.ToString("X") + "===";

      var url = new Uri(_apiserver + controller + "/" + actionname);
      var conn = WebRequest.CreateHttp(url);

      conn.Timeout = int.MaxValue;
      conn.Method = "POST";
      conn.ContentType = "multipart/form-data; boundary=" + boundary;
      conn.UserAgent = "MapLarge SDK C#";


      using (var writer = new StreamWriter(conn.GetRequestStream()))
      {
        // HEADERS
        writer.Write("User-Agent: MapLarge SDK C#");
        writer.Write(Environment.NewLine);

        // FORM FIELDS
        paramlist["mluser"] = _user;
        paramlist["mltoken"] = _token;
        foreach (var kvp in paramlist)
        {
          writer.Write("--" + boundary);
          writer.Write(Environment.NewLine);
          writer.Write("Content-Disposition: form-data; name=\"" + kvp.Key + "\"");
          writer.Write(Environment.NewLine);
          writer.Write(Environment.NewLine);
          writer.Write(kvp.Value);
          writer.Write(Environment.NewLine);
        }

        // FILES
        foreach (var path in filepaths)
        {
          if (!File.Exists(path))
          {
            continue; // MISSING FILE
          }
          var fname = Path.GetFileName(path);
          writer.Write("--" + boundary);
          writer.Write(Environment.NewLine);
          writer.Write("Content-Disposition: form-data; name=\"fileUpload\"; filename=\"" + fname + "\"");
          writer.Write(Environment.NewLine);
          //writer.Write("Content-Type: " +  URLConnection.guessContentTypeFromName(fname));
          //writer.Write(LINE_FEED);
          writer.Write("Content-Transfer-Encoding: binary");
          writer.Write(Environment.NewLine);
          writer.Write(Environment.NewLine);
          writer.Flush();
          using (var inputStream = new FileStream(path, FileMode.Open, FileAccess.Read))
          {
            var buffer = new byte[4096];
            var bytesRead = -1;
            while ((bytesRead = inputStream.Read(buffer, 0, 4096)) > 0)
            {
              writer.BaseStream.Write(buffer, 0, bytesRead);
            }
          }
          writer.Write(Environment.NewLine);
        }
        // FINISH
        writer.Write("--" + boundary + "--");
        writer.Write(Environment.NewLine);
      }
      // READ RESPONSE 
      var s = "";
      using (var response = conn.GetResponse())
      {
        using (var reqstream = response.GetResponseStream())
        using (var r = new StreamReader(reqstream))
        {
          string inputLine;
          while ((inputLine = r.ReadLine()) != null)
          {
            s += inputLine;
          }
        }
      }
      return s;
    }

    /**
		 * 
		 * @param actionname
		 *            Name of API action being called.
		 * @param paramlist
		 *            Array of key value pairs.
		 * @return API response, usually a JSON formatted string. Returns "ERROR" on
		 *         exception.
		 */

    public string InvokeAPIRequest(string actionname, Dictionary<string, string> paramlist)
    {
      return InvokeURL("Remote", actionname, paramlist);
    }

    /**
		 * 
		 * @param actionname
		 *            Name of API action being called.
		 * @param kvp
		 *            Array of key value pairs.
		 * @param filepaths
		 *            Array of files to attach to request. Use full file path.
		 * @return API response, usually a JSON formatted string. Returns "ERROR" on
		 *         exception.
		 */

    public string InvokeAPIRequestPost(string actionname, Dictionary<string, string> paramlist)
    {
      try
      {
        return InvokePostURL("Remote", actionname, paramlist, new string[0]);
      }
      catch (IOException ioe)
      {
        Console.WriteLine(ioe.Message);
        Console.WriteLine(ioe.StackTrace);
        return "ERROR";
      }
    }

    /**
		 * 
		 * @param actionname
		 *            Name of API action being called.
		 * @param kvp
		 *            Array of key value pairs.
		 * @param filepaths
		 *            Array of files to attach to request. Use full file path.
		 * @return API response, usually a JSON formatted string. Returns "ERROR" on
		 *         exception.
		 */

    public string InvokeAPIRequestPost(string actionname, Dictionary<string, string> paramlist,
      IEnumerable<string> filepaths)
    {
      try
      {
        return InvokePostURL("Remote", actionname, paramlist, filepaths);
      }
      catch (IOException ioe)
      {
        Console.WriteLine(ioe.Message);
        Console.WriteLine(ioe.StackTrace);
        return "ERROR";
      }
    }

    /**
		 * 
		 * @param user			Username to create authentication token for
		 * @param password		Password for supplied username
		 * @param ipAddress		IP address of the user for whom you want to build an authentication token 
		 * @return The authentication token in string form.
		 */

    public string GetRemoteAuthToken(string user, string password, string ipAddress)
    {
      // NO_WEB_CALLS = false;
      return InvokeURL("Auth", "RemoteLogin", "mluser=" + user + "&mlpass=" + password + "&remoteIP=" + ipAddress);
    }
  }
}