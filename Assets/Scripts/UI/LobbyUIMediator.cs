using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class LobbyUIMediator : MonoBehaviour
{
	ApplicationController applicationController;

	public LocalLobby m_LocalLobby;
	public LocalLobbyUser m_localLobbyUser;
	public ServicesManager m_ServicesManager;

	[SerializeField] private LobbyUI m_lobbyUI;
	[SerializeField] private JoinLobbyUI m_JoinLobbyUI;

	[SerializeField] private VoidEventChannelSO m_BlockUIEvent;
	[SerializeField] private VoidEventChannelSO m_UnblockUIEvent;


	[SerializeField] private MainMenuUIController m_MainMenuUIController;

	private void Start()
	{
		applicationController = GameObject.Find("ApplicationController").GetComponent<ApplicationController>();

		m_ServicesManager = applicationController.m_ServicesManager;
		m_LocalLobby = applicationController.m_LocalLobby;
		m_localLobbyUser = applicationController.m_LocalLobbyUser;

		m_LocalLobby.changed += m_lobbyUI.OnLocalLobbyChanged;

		m_JoinLobbyUI.Initialized(this);
		m_lobbyUI.Inittialize(this);
	}

	public async void CreateLobby()
	{
		m_BlockUIEvent.RaiseEvent();
		bool m_isSignedin = await m_ServicesManager.m_authServiceFacade.EnsurePlayerIsAuthorized();


		if (!m_isSignedin)
		{
			m_UnblockUIEvent.RaiseEvent();
			m_MainMenuUIController.ShowPopupWithMessage("Error", "Please log-in first!");
			return;
		}

		var m_createLobbyResult = await m_ServicesManager.m_lobbyServiceFacade.TryCreateLobbyAsync("Lobby", 4, false);

		if (!m_createLobbyResult.Success)
		{
			m_UnblockUIEvent.RaiseEvent();
			m_MainMenuUIController.ShowPopupWithMessage("Error", "Could not create new lobby, please try again!");

			return;
		}

		m_localLobbyUser.IsHost = true;

		m_ServicesManager.m_lobbyServiceFacade.SetRemoteLobby(m_createLobbyResult.Lobby);
		m_ServicesManager.m_lobbyServiceFacade.BeginTracking();

	 await	m_ServicesManager.m_lobbyServiceFacade.UpdateLobbyDataAndChangeLockStatusAsync(false);
		Debug.Log($"Created lobby with ID: {m_LocalLobby.LobbyID} and code {m_LocalLobby.LobbyCode}");

		m_UnblockUIEvent.RaiseEvent();

	}

	public void LeaveLobby()
	{
		m_ServicesManager.m_lobbyServiceFacade.EndTracking();
		m_LocalLobby.changed -= CheckIfGameStarted;
	}

	public async void JoinLobbyWithCode(string i_lobbyCode)
	{
		m_BlockUIEvent.RaiseEvent();

		bool m_isSignedin = await m_ServicesManager.m_authServiceFacade.EnsurePlayerIsAuthorized();

		if (!m_isSignedin)
		{
			m_UnblockUIEvent.RaiseEvent();
			m_MainMenuUIController.ShowPopupWithMessage("Error", "Please log-in first!");

			return;
		}


		var result = await m_ServicesManager.m_lobbyServiceFacade.TryJoinLobbyAsync(null, i_lobbyCode);

		if (!result.Success)
		{
			m_UnblockUIEvent.RaiseEvent();
			m_MainMenuUIController.ShowPopupWithMessage("Error", "Could not join lobby, please make sure lobby code is correct!");

			return;
		}

		m_ServicesManager.m_lobbyServiceFacade.SetRemoteLobby(result.Lobby);
		m_ServicesManager.m_lobbyServiceFacade.BeginTracking();

		m_UnblockUIEvent.RaiseEvent();
		m_LocalLobby.changed += CheckIfGameStarted;
	}

	public async void ChangeReadyState()
	{
		if (m_localLobbyUser.IsReady == "true")
			m_localLobbyUser.IsReady = "false";
		else
			m_localLobbyUser.IsReady = "true";

		await m_ServicesManager.m_lobbyServiceFacade.UpdatePlayerDataAsync("", "");
	}

	public async void StartGame()
	{
		foreach (var lobbyUser in m_LocalLobby.LobbyUsers)
		{
			if (lobbyUser.Value.IsReady == "false" && lobbyUser.Value.IsHost == false)
			{
				m_MainMenuUIController.ShowPopupWithMessage("Error", "Someone is not ready!");
				return;
			}
		}

		LoadingUIController.GetInstance().LoadTask("Joining game", 0.75f);

		m_ServicesManager.m_multiplayServiceFacade.StartHost();

		m_LocalLobby.IsStarted = "true";
		var data = NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData;
		m_LocalLobby.ServerIP = data.Address;
		m_LocalLobby.ServerPort = data.Port.ToString();
		m_LocalLobby.ServerListenAddress = data.ServerListenAddress;


		await m_ServicesManager.m_lobbyServiceFacade.UpdateLobbyDataAndChangeLockStatusAsync(true);
	}

	public void CheckIfGameStarted(LocalLobby localLobby)
	{
		if (localLobby.IsStarted == "true")
		{
			m_ServicesManager.m_multiplayServiceFacade.StartClient(localLobby);
			LoadingUIController.GetInstance().LoadTask("Joining game", 0.75f);

		}
	}

}
