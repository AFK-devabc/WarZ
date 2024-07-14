using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Services.Multiplay;
using UnityEngine;
using System.Threading.Tasks;
using System.Net;
using UnityEngine.SceneManagement;
using System;

public class MultiplayServiceFacade
{

	private float autoAllocateTimer = 9999999f;
	private bool alreadyAutoAllocated;
	private static IServerQueryHandler serverQueryHandler; // static so it doesn't get destroyed when this object is destroyed

	public async Task Initialize()
	{
#if DEDICATED_SERVER
		Debug.Log("DEDICATED_SERVER");

		MultiplayEventCallbacks multiplayEventCallbacks = new MultiplayEventCallbacks();
		multiplayEventCallbacks.Allocate += MultiplayEventCallbacks_Allocate;
		multiplayEventCallbacks.Deallocate += MultiplayEventCallbacks_Deallocate;
		multiplayEventCallbacks.Error += MultiplayEventCallbacks_Error;
		multiplayEventCallbacks.SubscriptionStateChanged += MultiplayEventCallbacks_SubscriptionStateChanged;
		IServerEvents serverEvents = await MultiplayService.Instance.SubscribeToServerEventsAsync(multiplayEventCallbacks);

		serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(4, "MyServerName", "KitchenChaos", "1.0", "Default");

		var serverConfig = MultiplayService.Instance.ServerConfig;
		if (serverConfig.AllocationId != "")
		{
			// Already Allocated
			MultiplayEventCallbacks_Allocate(new MultiplayAllocation("", serverConfig.ServerId, serverConfig.AllocationId));
		}
#endif
	}

	private void MultiplayEventCallbacks_SubscriptionStateChanged(MultiplayServerSubscriptionState obj)
	{
		Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_SubscriptionStateChanged");
		Debug.Log(obj);
	}

	private void MultiplayEventCallbacks_Error(MultiplayError obj)
	{
		Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_Error");
		Debug.Log(obj.Reason);
	}

	private void MultiplayEventCallbacks_Deallocate(MultiplayDeallocation obj)
	{
		Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_Deallocate");
	}

	private void MultiplayEventCallbacks_Allocate(MultiplayAllocation obj)
	{
		Debug.Log("DEDICATED_SERVER MultiplayEventCallbacks_Allocate");

		if (alreadyAutoAllocated)
		{
			Debug.Log("Already auto allocated!");
			return;
		}

		alreadyAutoAllocated = true;

		var serverConfig = MultiplayService.Instance.ServerConfig;
		Debug.Log($"Server ID[{serverConfig.ServerId}]");
		Debug.Log($"AllocationID[{serverConfig.AllocationId}]");
		Debug.Log($"Port[{serverConfig.Port}]");
		Debug.Log($"QueryPort[{serverConfig.QueryPort}]");
		Debug.Log($"LogDirectory[{serverConfig.ServerLogDirectory}]");

		string ipv4Address = "0.0.0.0";
		ushort port = serverConfig.Port;
		NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address, port, "0.0.0.0");

		//KitchenGameMultiplayer.Instance.StartServer();
		//Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
	}

	public void StartClient(LocalLobby localLobby)
	{
		NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
									localLobby.ServerIP,  // The IP address is a string
									(ushort)(Int32.Parse(localLobby.ServerPort)) // The port number is an unsigned short
									//,localLobby.ServerListenAddress // The server listen address is a string.
									);
		NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
		NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
		NetworkManager.Singleton.StartClient();
	}

	public void StartHost()
	{
		NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
							"127.0.0.1",  // The IP address is a string
							(ushort)12345, // The port number is an unsigned short
							"0.0.0.0" // The server listen address is a string.
							);
		NetworkManager.Singleton.StartHost();
		//SceneManager.LoadScene("DevView", LoadSceneMode.Single);
	}
}
