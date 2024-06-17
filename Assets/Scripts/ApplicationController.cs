using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Multiplay;
using Unity.Netcode.Transports.UTP;


public class ApplicationController : MonoBehaviour
{
	public LocalLobby m_LocalLobby { private set; get; }
	public LocalLobbyUser m_LocalLobbyUser { private set; get; }
	public ServicesManager m_ServicesManager { private set; get; }
	public UpdateRunner m_UpdateRunner { private set; get; }

	[SerializeField] private NetworkManager m_NetworkManager;

	ServerBrowser m_ServerBrowser;

	private async void Awake()
	{
		m_ServicesManager = new ServicesManager();

		if (UnityServices.State != ServicesInitializationState.Initialized)
		{
			await UnityServices.InitializeAsync();
			//m_ServicesManager.Initialize(m_LocalLobby, m_LocalLobbyUser, m_UpdateRunner);
		}
		m_ServerBrowser = new ServerBrowser();
		m_ServerBrowser.SearchForServer();


	}
	private void Start()
	{
#if !DEDICATED_SERVER

		m_ServicesManager = new ServicesManager();

		Application.wantsToQuit += OnWantToQuit;
		DontDestroyOnLoad(gameObject);
		Application.targetFrameRate = 60;

		m_LocalLobby = new LocalLobby();
		m_LocalLobbyUser = new LocalLobbyUser();
		m_UpdateRunner = new UpdateRunner();

		int randomnumber = UnityEngine.Random.Range(0, 100);
		m_LocalLobbyUser.DisplayName = "Player" + randomnumber.ToString();
		m_LocalLobbyUser.CharacterIndex = 1.ToString();
		m_LocalLobbyUser.WeaponIndex = 1.ToString();

		m_ServicesManager.Initialize(m_LocalLobby, m_LocalLobbyUser, m_UpdateRunner);

		SceneManager.LoadScene("MainMenu");
#endif //!DEDICATED_SERVER

	}

	private bool OnWantToQuit()
	{
#if DEDICATED_SERVER
		return false;
#else

		Application.wantsToQuit -= OnWantToQuit;

		var canQuit = m_LocalLobby != null && string.IsNullOrEmpty(m_LocalLobby.LobbyID);
		if (!canQuit)
		{
			StartCoroutine(LeaveBeforeQuit());
		}
		return canQuit;

#endif
	}


#if !DEDICATED_SERVER
	private IEnumerator LeaveBeforeQuit()
	{
		// We want to quit anyways, so if anything happens while trying to leave the Lobby, log the exception then carry on
		try
		{
			Utils.OnLeaveLobbySuccessEvent?.Invoke();
			m_ServicesManager.m_lobbyServiceFacade.EndTracking();
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message);
		}

		yield return null;
		Application.Quit();
	}
#endif

	public void QuitGame()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
	}

	//private Dictionary<string, string> GetCommandlineArgs()
	//{
	//	Dictionary<string, string> argDictionary = new Dictionary<string, string>();

	//	var args = System.Environment.GetCommandLineArgs();

	//	for (int i = 0; i < args.Length; ++i)
	//	{
	//		var arg = args[i].ToLower();
	//		if (arg.StartsWith("-"))
	//		{
	//			var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
	//			value = (value?.StartsWith("-") ?? false) ? null : value;

	//			argDictionary.Add(arg, value);
	//		}
	//	}
	//	return argDictionary;
	//}

}

