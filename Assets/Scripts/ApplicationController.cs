using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ApplicationController : MonoBehaviour
{
	public LocalLobby m_LocalLobby { private set; get; }
	public LocalLobbyUser m_LocalLobbyUser { private set; get; }
	public ServicesManager m_ServicesManager { private set; get; }
	public UpdateRunner m_UpdateRunner { private set; get; }

	[SerializeField] private NetworkManager m_NetworkManager;

	#region singleton
	private static ApplicationController m_instance = null;
	public static ApplicationController GetInstance() { return m_instance; }

	private void Awake()
	{
		if (m_instance == null)
		{
			m_instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(this);
			return;
		}
	}
	#endregion //singleton

	private void Start()
	{
		Application.targetFrameRate = 60;

#if !DEDICATED_SERVER
		Application.wantsToQuit += OnWantToQuit;
		DontDestroyOnLoad(gameObject);

		m_LocalLobby = new LocalLobby();
		m_LocalLobbyUser = new LocalLobbyUser();
		m_UpdateRunner = gameObject.AddComponent<UpdateRunner>();

		int randomnumber = UnityEngine.Random.Range(0, 100);
		m_LocalLobbyUser.DisplayName = "Player" + randomnumber.ToString();
		m_LocalLobbyUser.CharacterIndex = 1.ToString();
		m_LocalLobbyUser.WeaponIndex = 1.ToString();

		AsyncOperation loadingMMAsync = SceneManager.LoadSceneAsync("MainMenu");

		LoadingUIController.GetInstance().LoadTask("loading", loadingMMAsync);
#endif
		m_ServicesManager = new ServicesManager();
		m_ServicesManager.Initialize(m_LocalLobby, m_LocalLobbyUser, m_UpdateRunner);
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
}

