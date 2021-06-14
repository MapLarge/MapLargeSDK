# MapLarge SDK Overview
<img src="http://maplarge.com/sites/default/files/maplargelogo3_0.png" alt="maplarge"/>

[MapLarge Site](http://www.maplarge.com)


[MapLarge JS API Info](http://maplarge.com/developer/js)

[REST / Remote Table API Reference](https://github.com/MapLarge/MapLargeSDK/blob/master/api.md)

## MapLarge API Connector

The MapLarge API Connector (MapLargeConnector) is a convenience class that facilities access of the MapLarge RESTFul API. The class MapLargeConnector has been implemented in 4 languages: Python, Java, C#, and PHP. 

The main class MapLargeConnector is used to establish and manage a session with a MapLarge API server. This class exposes several methods for accessing functionality on the MapLarge server.

### Methods

#### Constructor

The Constructor follows this basic pattern, with slight differences depending on language. It is overloaded to allow for passing of username/password or username/authtoken for authentication.

``` csharp
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
 
MapLargeConnector(string urlApiServer, string username, int token)

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
 
MapLargeConnector(string urlApiServer, string username, string password)

```

#### InvokeAPIRequest
Used to Invoke API requests over HTTP that can be handled sufficiently by GET requests. This would be for relatively short data, such as Account or Table Listings. Files can be uploaded, but only in the limited case of passing files by URL. Uploading of file data directly must use the POST method below.

``` csharp
/**
 * 
 * @param actionname
 *            Name of API action being called.
 * @param paramlist
 *            Array of key value pairs.
 * @return API response, usually a JSON formatted string. Returns "ERROR" on
 *         exception.
 */

string InvokeAPIRequest(string actionname, Dictionary<string, string> paramlist)

```

#### InvokeAPIRequestPost
Performs the same API Requests as the first method, but can handle sending and receiving of large amounts of data as HTTP POST is used. This also allows for full featured Binary file uploads directly from your client application.
```csharp

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
 
string InvokeAPIRequestPost(string actionname, Dictionary<string, string> paramlist) 

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
 
string InvokeAPIRequestPost(string actionname, Dictionary<string, string> paramlist, string[] filepaths)

```

#### NO_WEB_CALLS

A property that when set to true causes the API to "simulate" functionality. HTTP calls will not actually occur, and debug info about the call is returned from methods. Useful for debugging.
``` csharp

MapLargeConnector.NO_WEB_CALLS = true;
string response = mlconnPassword.InvokeAPIRequest("CreateTableSynchronous", paramlist);

//Outputs Debug info
Console.WriteLine(response);

```
#### GetRemoteAuthToken
A convenience method to allow retrieval of the Auth Token -- a common use case among many of users.
```csharp

/**
 * 
 * @param user			
 	Username to create authentication token for
 * @param password	
 	Password for supplied username
 * @param ipAddress		
 	IP address of the user for whom you want to build an authentication token 
 * @return The authentication token in string form.
 */

string GetRemoteAuthToken(string user, string password, string ipAddress)

```

### General Usage

General usage involves instantiation of the MapLargeConnector and subsequent calls to the server via the InvokeAPI methods.

```csharp

//Authentication inforamtion
string server = "http://server.maplarge.com/";
string user = "user@ml.com";
string pass = "pw123456";

//HTTP Parameters
Dictionary<string, string> paramlist = new Dictionary<string, string>();

//CREATE MAPLARGE CONNECTION WITH USER / PASSWORD
MapLargeConnector mlconnPassword = new MapLargeConnector(server, user, pass);

//CREATE TABLE SYNCHRONOUSLY
//Set parameters: account, tablename & file URL
paramlist.Add("account", "test");
paramlist.Add("tablename", "testJavaSdkTable");
paramlist.Add("fileurl", "http://www.domain.com/testfile.csv");

string response = mlconnPassword.InvokeAPIRequest("CreateTableSynchronous", paramlist);

```


###  Language specific Usage

#### C#: 

``` csharp

//DEFAULT CREDENTIALS
string server = "http://server.maplarge.com/";
string user = "user@ml.com";
string pass = "pw123456";
int token = 123456789;

Dictionary<string, string> paramlist = new Dictionary<string, string>();

//CREATE MAPLARGE CONNECTION WITH USER / PASSWORD
MapLargeConnector mlconnPassword = new MapLargeConnector(server, user, pass);

//CREATE MAPLARGE CONNECTION WITH USER / AUTH TOKEN
MapLargeConnector mlconnToken = new MapLargeConnector(server, user, token);

//CREATE TABLE SYNCHRONOUS (NO WEB CALL)
paramlist.Add("account", "test");
paramlist.Add("tablename", "testJavaSdkTable");
paramlist.Add("fileurl", "http://www.domain.com/testfile.csv");
MapLargeConnector.NO_WEB_CALLS = true;
string response = mlconnPassword.InvokeAPIRequest("CreateTableSynchronous", paramlist);
Console.WriteLine(response);
MapLargeConnector.NO_WEB_CALLS = false;

//RETRIEVE REMOTE USER AUTH TOKEN 
response = mlconnPassword.GetRemoteAuthToken(user, pass, "255.255.255.255");
Console.WriteLine(response);

//LIST GROUPS
paramlist.Clear();
paramlist.Add("account", "test");
response = mlconnToken.InvokeAPIRequestPost("ListGroups", paramlist);
Console.WriteLine(response);

//CREATE TABLE WITH FILES SYNCHRONOUS
paramlist.Clear();
paramlist.Add("account", "test");
paramlist.Add("tablename", "PostedTableImport");
response = mlconnToken.InvokeAPIRequestPost("CreateTableWithFilesSynchronous", paramlist,
		new string[] { "C:\\Data\\TestFile.csv" });
Console.WriteLine(response);
			
		
```

#### PHP


``` php


//DEFAULT CREDENTIALS
$server = "http://server.maplarge.com/";
$user = "user@ml.com";
$pass = "pw123456";
$token = 921129417;

//CREATE MAPLARGE CONNECTION WITH USER / PASSWORD
$mlconnPassword = MapLargeConnector::CreateFromPassword($server, $user, $pass);

//CREATE MAPLARGE CONNECTION WITH USER / AUTH TOKEN
$mlconnToken = MapLargeConnector::CreateFromToken($server, $user, $token);

//CREATE TABLE SYNCHRONOUS (NO WEB CALL)
$paramlist = array(
    'account' => 'aidsvualpha',
    'tablename' => 'testPHPSDKTable',
    'fileurl' => 'http://www.domain.com/testfile.csv'
);
MapLargeConnector::$NO_WEB_CALLS = true;
$response = $mlconnPassword->InvokeAPIRequest("CreateTableSynchronous", $paramlist);
echo $response . PHP_EOL;
MapLargeConnector::$NO_WEB_CALLS = false;

//RETRIEVE REMOTE USER AUTH TOKEN
$response = $mlconnPassword->GetRemoteAuthToken($user, $pass, "255.255.255.255");
echo $response . PHP_EOL;

//LIST GROUPS
$paramlist = array(
    'account' => 'test',
);
$response = $mlconnToken->InvokeAPIRequestPost("ListGroups", $paramlist);
echo $response . PHP_EOL;

//CREATE TABLE WITH FILES SYNCHRONOUS
$paramlist = array(
    'account' => 'test',
    'tablename' => 'PostedTableImportPHP',
);
$response = $mlconnToken->InvokeAPIRequestPostWithFiles("CreateTableWithFilesSynchronous", $paramlist, array("N:\\MergedPhoenix.csv"));
echo $response . PHP_EOL;
echo 'DONE' . PHP_EOL;
```

### Java

``` java

//DEFAULT CREDENTIALS
String server = "http://server.maplarge.com/";
String user = "user@ml.com";
String pass = "pw123456";
int token = 123456789;

Map<String, String> params = new HashMap<String, String>();

//CREATE MAPLARGE CONNECTION WITH USER / PASSWORD
MapLargeConnector mlconnPassword = new MapLargeConnector(server, user, pass);

//CREATE MAPLARGE CONNECTION WITH USER / AUTH TOKEN
MapLargeConnector mlconnToken = new MapLargeConnector(server, user, token);

//CREATE TABLE SYNCHRONOUS (NO WEB CALL)
params.put("account", "test");
params.put("tablename", "testJavaSdkTable");
params.put("fileurl", "http://localhost/testfile.csv");
MapLargeConnector.NO_WEB_CALLS = true;
String response = mlconnPassword.InvokeAPIRequest("CreateTableSynchronous", params);
System.out.println(response);
MapLargeConnector.NO_WEB_CALLS = false;

//RETRIEVE REMOTE USER AUTH TOKEN
response = mlconnPassword.GetRemoteAuthToken(user, pass, "255.255.255.255");
System.out.println(response);

//LIST GROUPS
params.clear();
params.put("account", "test");
response = mlconnToken.InvokeAPIRequestPost("ListGroups", params);
System.out.println(response);

//CREATE TABLE WITH FILES SYNCHRONOUS
params.clear();
params.put("account", "test");
params.put("tablename", "PostedTableImport");
response = mlconnToken.InvokeAPIRequestPost("CreateTableWithFilesSynchronous", params,	new String[] { "C:\\temp\\usa.csv" });
System.out.println(response);

```

#### Python

``` python

server = "http://server.maplarge.com/"
user = "user@ml.com"
pw = "pw123456"

#CREATE MAPLARGE CONNECTION WITH USER / PASSWORD
mlconnPassword = MapLargeConnector(server, user, pw)

#CREATE MAPLARGE CONNECTION WITH USER / AUTH TOKEN
mlconnToken = MapLargeConnector(server, user, token)

#CREATE TABLE SYNCHRONOUS (NO WEB CALL)
params = {"account": "aidsvualpha", "tablename": "testPythonSdkTable", "fileurl": "http://localhost/testfile.csv"}
mlconnToken.NO_WEB_CALLS = True
response = mlconnToken.InvokeAPIRequest("CreateTableSynchronous", params)
print response

mlconnPassword.NO_WEB_CALLS = False

#RETRIEVE REMOTE USER AUTH TOKEN
response = mlconnPassword.GetRemoteAuthToken(user, pw, "255.255.255.255")
print response

#List Groups
params = {"account": "aidsvualpha"}
response = mlconnPassword.InvokeAPIRequestPost("ListGroups", params)
print response

#CREATE TABLE WITH FILES SYNCHRONOUS
params = {"account": "aidsvualpha", "tablename": "testTable"}
fileList = ["c:\\temp\\usa.csv"]
print mlconnPassword.InvokeAPIRequestPost("CreateTableWithFilesSynchronous", params, fileList)

```


For more samples:

[www.maplarge.com](http://www.maplarge.com)

[MapLarge JS API Info](http://maplarge.com/api)
