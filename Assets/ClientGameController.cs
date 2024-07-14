using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameController : NetworkBehaviour
{
	#region singleton
	private static ClientGameController m_instance = null;
	public static ClientGameController GetInstance() { return m_instance; }

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
	public NetworkList<PlayerData> playerDataNetworkList;
	public NetworkVariable<int> numberPlayer;
	public NetworkVariable<string> sceneName;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		playerDataNetworkList = new NetworkList<PlayerData>();
		numberPlayer = new NetworkVariable<int>();

		if (IsClient)
		{
			NetworkManager.SceneManager.OnSceneEvent += ServerSceneEventCallback;

			if (ApplicationController.GetInstance().m_LocalLobbyUser.IsHost)
			{
				SetGameDataServerRpc(ApplicationController.GetInstance().m_LocalLobby.PlayerCount,
					ApplicationController.GetInstance().m_LocalLobby.Data.sceneName);
			}

			CheckIfStartServerRpc();
		}
		else if (IsServer)
		{
			numberPlayer.Value = 4;
		}
	}

	[ServerRpc]
	private void CheckIfStartServerRpc()
	{
		CheckIfStartServer();
	}

	public void CheckIfStartServer()
	{
		if (playerDataNetworkList.Count == numberPlayer.Value)
			LoadGameplayScene(sceneName.Value);
	}

	private static void ServerSceneEventCallback(SceneEvent sceneEvent)
	{
		Debug.Log(sceneEvent);
	}

	public void LoadGameplayScene(string sceneName)
	{
		NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
	}

	[ServerRpc]
	public void SetGameDataServerRpc(int number, string sceneName)
	{
		numberPlayer.Value = number;
		this.sceneName.Value = sceneName;
	}

}
