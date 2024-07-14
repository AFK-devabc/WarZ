using Unity.Collections;
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
			playerDataNetworkList = new NetworkList<PlayerData>();
			numberPlayer = new NetworkVariable<int>();
			sceneName = new NetworkVariable<FixedString64Bytes>();
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
	public NetworkVariable<FixedString64Bytes> sceneName;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

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
			LoadGameplayScene(sceneName.Value.ToString());
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
