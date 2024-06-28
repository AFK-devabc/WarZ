using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : NetworkBehaviour
{
	public static NetworkVariable<int> CurrentGameState { get; private set; } = new NetworkVariable<int>(0);

	public delegate void GameStateChangeHandler(GameState newGameState);
	public static event GameStateChangeHandler OnGameStateChanged;

	public TopDownCamera m_localCameraController;

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
		if (IsClient || IsHost)
		{
			if (Utils.isClientCharacterSetupDone)
				Initialize(Utils.m_localNetworkPlayer);
			else
				Utils.OnClientCharacterSetupDone += Initialize;
		}
		if (IsHost)
		{
			SceneManager.LoadSceneAsync("InGameUI", LoadSceneMode.Additive);
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
