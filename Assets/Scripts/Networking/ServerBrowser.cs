using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class ServerBrowser
{
	// Start is called before the first frame update

	static string keyId = "55b20e55-e1a4-4de8-ba9f-8dfc86fb7c6e";
	static string keySecret = "1KJz9epNdbYGp1ptbO5-ofgyZQjccmhj";
	static byte[] keyByteArray = Encoding.UTF8.GetBytes(keyId + ":" + keySecret);
	static string keyBase64 = Convert.ToBase64String(keyByteArray);

	static string projectId = "78720668-dd28-4fc2-b32c-c72fb7040caf";
	static string environmentId = "53a13c23-a716-42a5-a556-e6f3f17810d4";

	public Action<bool, AllocatedServer> OnSearchServerComplete;

	public void SearchForServer()
	{

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
			if (server.status == ServerStatus.AVAILABLE.ToString())
			{
				// Server is Online!
				//Transform serverTransform = Instantiate(serverTemplate, serverContainer);
				//serverTransform.gameObject.SetActive(true);
				//serverTransform.GetComponent<ServerBrowserSingleUI>().SetServer(
				//	server.ip,
				//	(ushort)server.port
				//);
				//OnSearchServerComplete?.Invoke(true, server);
				AllocateServer(server);
				return;
			}
		}
	}


	public void AllocateServer(Server server)
	{
		string url = $"https://services.api.unity.com/auth/v1/token-exchange?projectId={projectId}&environmentId={environmentId}";
		string serverAllocationID = Guid.NewGuid().ToString();
		string jsonRequestBody = JsonUtility.ToJson(new TokenExchangeRequest
		{
			scopes = new[] { "multiplay.allocations.create", "multiplay.allocations.list", "multiplay.allocations.get" },
		});

		WebRequests.PostJson(url,
		(UnityWebRequest unityWebRequest) =>
		{
			unityWebRequest.SetRequestHeader("Authorization", "Basic " + keyBase64);
		},
		jsonRequestBody,
		(string error) =>
		{
			Debug.Log("Error: " + error);
		},
		(string json) =>
		{
			Debug.Log("Success: " + json);
			TokenExchangeResponse tokenExchangeResponse = JsonUtility.FromJson<TokenExchangeResponse>(json);

			string fleetId = server.fleetID;
			string url = $"https://multiplay.services.api.unity.com/v1/allocations/projects/{projectId}/environments/{environmentId}/fleets/{fleetId}/allocations";

			WebRequests.PostJson(url,
			(UnityWebRequest unityWebRequest) =>
			{
				unityWebRequest.SetRequestHeader("Authorization", "Bearer " + tokenExchangeResponse.accessToken);
			},
			JsonUtility.ToJson(new QueueAllocationRequest
			{
				allocationId = serverAllocationID,
				buildConfigurationId = server.buildConfigurationID,
				regionId = "b7130a00-c47c-4127-9a01-ea5f78169d98",
			}),
			(string error) =>
			{
				Debug.Log("Error: " + error);
			},
			(string json) =>
			{
				AllocateServerResponde allocateServerResponde = JsonUtility.FromJson<AllocateServerResponde>(json);
				OnAllocatedServerSuccess(allocateServerResponde, tokenExchangeResponse);
			}
			);
		}
		);
	}

	public void OnAllocatedServerSuccess(AllocateServerResponde allocateServerResponde, TokenExchangeResponse tokenExchangeResponse)
	{
		string url = $"https://multiplay.services.api.unity.com{allocateServerResponde.href}";

		Debug.Log(url);
		WebRequests.Get(url,
			(UnityWebRequest unityWebRequest) =>
				{
					unityWebRequest.SetRequestHeader("Authorization", "Bearer " + tokenExchangeResponse.accessToken);
				},
			(string error) =>
			{
				Debug.Log("Error: " + error);
			},
			(string json) =>
			{
				AllocatedServer allocatedServer = JsonUtility.FromJson<AllocatedServer>(json);
				if (allocatedServer.ipv4 == string.Empty)
					OnAllocatedServerSuccess(allocateServerResponde, tokenExchangeResponse);
				else
				{
					Debug.Log(allocatedServer);
					OnSearchServerComplete(true, allocatedServer);
				}
			}
);

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
public class AllocateServerResponde
{
	public string allocationId;
	public string href;
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

[Serializable]
public class AllocatedServer
{
	public string allocationId;
	public int buildConfigurationID;
	public string created;
	public string fleetID;
	public string fulfilled;
	public int gamePort;
	public string ipv4;
	public string ipv6;
	public int machineID;
	public bool readiness;
	public string ready;
	public string regionId;
	public int requestId;
	public string requested;
	public int serverId;
}
