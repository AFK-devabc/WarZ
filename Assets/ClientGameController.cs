using Unity.Netcode;
using UnityEngine;

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

	//[SerializeField] public TopDownCamera m_localCameraController;

	//private void Start()
	//{
	//	//SceneManager.LoadSceneAsync("InGameUI", LoadSceneMode.Additive);
	//	NetworkManager.Singleton.ConnectionApprovalCallback += ConnectionApprovalCallback;

	//}

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		if (IsClient)
		{
			NetworkManager.SceneManager.OnSceneEvent += ServerSceneEventCallback;
		}
	}

	//void ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
	//{
	//	/* you can use this method in your project to customize one of more aspects of the player 
	//	 * (I.E: its start position, its character) and to perform additional validation checks. */
	//	response.Approved = true;
	//	response.CreatePlayerObject = true;
	//	response.Position = GetPlayerSpawnPosition();
	//}


	private static void ServerSceneEventCallback(SceneEvent sceneEvent)
	{
		Debug.Log(sceneEvent);
	}
	//public void OnEscButton(InputValue context)
	//{
	//	if (context.isPressed)
	//	{
	//		if (!SceneManager.GetSceneByName("InGameUI").isLoaded)
	//			SceneManager.LoadSceneAsync("InGameUI", LoadSceneMode.Additive);
	//		else
	//			SceneManager.UnloadSceneAsync("InGameUI");
	//	}
	//}
}
