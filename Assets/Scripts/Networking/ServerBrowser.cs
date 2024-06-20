using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ServerBrowser
{
	// Start is called before the first frame update
	public void SearchForServer()
	{
		string keyId = "55b20e55-e1a4-4de8-ba9f-8dfc86fb7c6e";
		string keySecret = "1KJz9epNdbYGp1ptbO5-ofgyZQjccmhj";
		byte[] keyByteArray = Encoding.UTF8.GetBytes(keyId + ":" + keySecret);
		string keyBase64 = Convert.ToBase64String(keyByteArray);

		string projectId = "78720668-dd28-4fc2-b32c-c72fb7040caf";
		string environmentId = "53a13c23-a716-42a5-a556-e6f3f17810d4";
		string url = $"https://services.api.unity.com/multiplay/servers/v1/projects/{projectId}/environments/{environmentId}/servers";

		WebRequests.Get(url,
			(UnityWebRequest unityWebRequest) =>
			{
				unityWebRequest.SetRequestHeader("Authorization", "Basic " + keyBase64);
			},
		LogServerMessage, OnSearchServerSuccess);

	}

	private void LogServerMessage(string i_message)
	{
		Debug.Log(i_message);
	}

	private void OnSearchServerSuccess(string i_result)
	{
		LogServerMessage("Success: " + i_result);
			ListServers listServers = JsonUtility.FromJson<ListServers>("{\"serverList\":" + i_result + "}");
			foreach (Server server in listServers.serverList)
			{
			Debug.Log(server.ip + " : " + server.port + " " + server.deleted + " " + server.status);
			if (server.status == ServerStatus.ONLINE.ToString() || server.status == ServerStatus.ALLOCATED.ToString())
				{
					//// Server is Online!
					//Transform serverTransform = Instantiate(serverTemplate, serverContainer);
					//serverTransform.gameObject.SetActive(true);
					//serverTransform.GetComponent<ServerBrowserSingleUI>().SetServer(
					//	server.ip,
					//	(ushort)server.port
					//);
				}
			}
	}

}

public class TokenExchangeResponse
{
	public string accessToken;
}


[Serializable]
public class TokenExchangeRequest
{
	public string[] scopes;
}

public class QueueAllocationRequest
{
	public string allocationId;
	public int buildConfigurationId;
	public string payload;
	public string regionId;
	public bool restart;
}


enum ServerStatus
{
	AVAILABLE,
	ONLINE,
	ALLOCATED
}

[Serializable]
public class ListServers
{
	public Server[] serverList;
}

[Serializable]
public class Server
{
	public int buildConfigurationID;
	public string buildConfigurationName;
	public string buildName;
	public bool deleted;
	public string fleetID;
	public string fleetName;
	public string hardwareType;
	public int id;
	public string ip;
	public int locationID;
	public string locationName;
	public int machineID;
	public int port;
	public string status;
}

