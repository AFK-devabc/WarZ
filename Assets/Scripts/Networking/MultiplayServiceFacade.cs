using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Multiplay;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiplayServiceFacade
{

#if DEDICATED_SERVER
	private float autoAllocateTimer = 9999999f;
	private bool alreadyAutoAllocated;
	private static IServerQueryHandler serverQueryHandler; // static so it doesn't get destroyed when this object is destroyed
#endif



	public async Task Initialize()
	{
#if DEDICATED_SERVER
		if (UnityServices.State != ServicesInitializationState.Initialized)
		{
			InitializationOptions initializationOptions = new InitializationOptions();
			//initializationOptions.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());

			await UnityServices.InitializeAsync(initializationOptions);
			Debug.Log("DEDICATED_SERVER Initialize");


			MultiplayEventCallbacks multiplayEventCallbacks = new MultiplayEventCallbacks();
			multiplayEventCallbacks.Allocate += MultiplayEventCallbacks_Allocate;
			multiplayEventCallbacks.Deallocate += MultiplayEventCallbacks_Deallocate;
			multiplayEventCallbacks.Error += MultiplayEventCallbacks_Error;
			multiplayEventCallbacks.SubscriptionStateChanged += MultiplayEventCallbacks_SubscriptionStateChanged;
			IServerEvents serverEvents = await MultiplayService.Instance.SubscribeToServerEventsAsync(multiplayEventCallbacks);

			Debug.Log("SubscribeToServerEventsAsync complete");

			serverQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(4, "TestServer", "WarZ", "1.0", "Default");

			var serverConfig = MultiplayService.Instance.ServerConfig;
			if (serverConfig.AllocationId != "")
			{
				// Already Allocated
				MultiplayEventCallbacks_Allocate(new MultiplayAllocation("", serverConfig.ServerId, serverConfig.AllocationId));
			}

		}
		else
		{
			// Already Initialized
			Debug.Log("DEDICATED_SERVER LOBBY - ALREADY INIT");

			var serverConfig = MultiplayService.Instance.ServerConfig;
			if (serverConfig.AllocationId != "")
			{
				// Already Allocated
				MultiplayEventCallbacks_Allocate(new MultiplayAllocation("", serverConfig.ServerId, serverConfig.AllocationId));
			}
		}
#endif
	}

#if DEDICATED_SERVER

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
		MultiplayService.Instance.UnreadyServerAsync();
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

		MultiplayService.Instance.ReadyServerForPlayersAsync();

		StartServer();
	}
#endif
	public void StartClient(LocalLobby localLobby)
	{
		NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(
									localLobby.ServerIP,  // The IP address is a string
									(ushort)(Int32.Parse(localLobby.ServerPort)) // The port number is an unsigned short
									, "0.0.0.0"                                          //,localLobby.ServerListenAddress // The server listen address is a string.
									);
		NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
		NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
		NetworkManager.Singleton.StartClient();
	}

	private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
	{
		if (!NetworkManager.Singleton.IsServer && NetworkManager.Singleton.DisconnectReason != string.Empty)
		{
			Debug.Log($"Approval Declined Reason: {NetworkManager.Singleton.DisconnectReason}");
			//OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
		}
	}

	private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
	{
		//SetPlayerNameServerRpc(GetPlayerName());
		//SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
	}

	public void StartServer()
	{
		NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
		NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
		NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
		NetworkManager.Singleton.StartServer();

		Debug.Log("Server Started");
	}

	private async void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
	{
		Debug.Log("Client disconnected");

		for (int i = 0; i < ClientGameController.GetInstance().playerDataNetworkList.Count; i++)
		{
			PlayerData playerData = ClientGameController.GetInstance().playerDataNetworkList[i];
			if (playerData.clientId == clientId)
			{
				// Disconnected!
				ClientGameController.GetInstance().playerDataNetworkList.RemoveAt(i);
			}
		}

#if DEDICATED_SERVER
		Debug.Log("playerDataNetworkList.Count " + ClientGameController.GetInstance().playerDataNetworkList.Count);
		if (ClientGameController.GetInstance().playerDataNetworkList.Count <= 0)
		{
			// All players left the game
			Debug.Log("All players left the game");

			Debug.Log("Shutting Down Network Manager");
			NetworkManager.Singleton.Shutdown();

			Debug.Log("Shutting Down Multiplay Service");
			await MultiplayService.Instance.UnreadyServerAsync();

			Debug.Log("Exit Application!");
			Application.Quit(0);
			Environment.Exit(0);
		}
#endif
	}

	private void NetworkManager_OnClientConnectedCallback(ulong clientId)
	{
		Debug.Log("Client Connected " + " " + clientId);
		ClientGameController.GetInstance().playerDataNetworkList.Add(new PlayerData
		{
			clientId = clientId,
		});
		ClientGameController.GetInstance().CheckIfStartServer();
	}

	private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
	{
		Debug.Log(SceneManager.GetActiveScene().name);
		if (SceneManager.GetActiveScene().name != "MainMenu")
		{
			Debug.Log("Client try to connect but game already started");
			connectionApprovalResponse.Approved = false;
			connectionApprovalResponse.Reason = "Game has already started";
			return;
		}
		connectionApprovalResponse.Approved = true;
	}

	public void Update()
	{
#if DEDICATED_SERVER
		autoAllocateTimer -= Time.deltaTime;
		if (autoAllocateTimer <= 0f)
		{
			autoAllocateTimer = 999f;
			MultiplayEventCallbacks_Allocate(null);
		}

		if (serverQueryHandler != null)
		{
			if (NetworkManager.Singleton.IsServer)
			{
				serverQueryHandler.CurrentPlayers = (ushort)NetworkManager.Singleton.ConnectedClientsIds.Count;
			}
			serverQueryHandler.UpdateServerCheck();
		}
#endif
	}
}
