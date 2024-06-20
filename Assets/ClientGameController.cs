using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameController :  NetworkBehaviour
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

	private void Start()
    {
		//SceneManager.LoadSceneAsync("InGameUI", LoadSceneMode.Additive);
	}

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		if (IsHost)
		{
			
			var status = NetworkManager.Singleton.SceneManager.LoadScene("DevView", LoadSceneMode.Single);
			if (status != SceneEventProgressStatus.Started)
			{
				Debug.LogWarning($"Failed to load DevView" +
					  $"with a {nameof(SceneEventProgressStatus)}: {status}");
			}
		}
		else if(IsClient)
		{

			NetworkManager.SceneManager.OnSceneEvent += ServerSceneEventCallback;
		}
	}

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
