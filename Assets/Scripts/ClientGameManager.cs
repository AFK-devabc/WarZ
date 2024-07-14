using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : NetworkBehaviour
{
	#region singleton
	private static GameStateManager m_instance = null;
	public static GameStateManager GetInstance() { return m_instance; }

	private void Awake()
	{
		if (m_instance == null)
		{
			m_instance = this;
		}
		else
		{
			Destroy(this);
			return;
		}
	}
	#endregion
	public static NetworkVariable<int> CurrentGameState { get; private set; } = new NetworkVariable<int>(0);

	public delegate void GameStateChangeHandler(GameState newGameState);
	public static event GameStateChangeHandler OnGameStateChanged;

	public TopDownCamera m_localCameraController;

	[SerializeField] private GameObject m_playerGameobject;

	public void Initialize(NetworkPlayer networkPlayer)
	{
		Instantiate(m_localCameraController);

		CurrentGameState.OnValueChanged += OnGameStateChange;
		Utils.m_localNetworkPlayer.transform.position = GetPlayerSpawnPosition();
		LoadingUIController.GetInstance().HideAfterLoadComplete();
	}

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		if (IsClient)
		{
			if (Utils.isClientCharacterSetupDone)
				Initialize(Utils.m_localNetworkPlayer);
			else
				Utils.OnClientCharacterSetupDone += Initialize;
			SceneManager.LoadSceneAsync("InGameUI", LoadSceneMode.Additive);
		}
		else if (IsServer)
		{
			foreach (var playerdata in ClientGameController.GetInstance().playerDataNetworkList)
			{
				GameObject newPlayer = Instantiate(m_playerGameobject, GetPlayerSpawnPosition(), Quaternion.identity);
				newPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerdata.clientId, true);
			}
		}
	}


	Vector3 GetPlayerSpawnPosition()
	{
		/*
		 * this is just an example, and you change this implementation to make players spawn on specific spawn points
		 * depending on other factors (I.E: player's team)
		 */
		return new Vector3(UnityEngine.Random.Range(-3, 3), 0, UnityEngine.Random.Range(-3, 3));
	}


	public void SetState(int newGameState)
	{
		if (newGameState == CurrentGameState.Value)
			return;

		CurrentGameState.Value = newGameState;
	}

	private void OnGameStateChange(int oldState, int newState)
	{
		OnGameStateChanged?.Invoke((GameState)newState);
	}

	public override void OnNetworkDespawn()
	{
		base.OnNetworkDespawn();
	}

}

public enum GameState
{
	MAINMENU,
	LOADING,
	GAMEPLAY,
	Pausing
}
