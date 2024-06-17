using UnityEngine;

public class CameraInMM : MonoBehaviour
{
	private CameraState m_state;

	[SerializeField] private Transform m_transform;
	[SerializeField] private Transform m_mmPosition;
	[SerializeField] private Transform m_lobbyPosition;
	[SerializeField] private Transform m_customCharacterPosition;

	private void Start()
	{
		m_state = CameraState.MAINMENU;
		Utils.OnJoinLobbySuccessEvent+= OnJoinLobbyEvent;
		Utils.OnLeaveLobbySuccessEvent += OnLeaveLobbyEvent;
		Utils.OnEnterCustomCharacterEvent += OnEnterCustomCharacterEvent;
		Utils.OnLeaveCustomCharacterEvent += OnLeaveCustomCharacterEvent;
	}

	private void FixedUpdate()
	{
		switch(m_state)
		{
			case CameraState.MAINMENU:
				m_transform.position = Vector3.Lerp(m_transform.position, m_mmPosition.position, Time.fixedDeltaTime);
				m_transform.rotation = Quaternion.Lerp(m_transform.rotation, m_mmPosition.rotation, Time.fixedDeltaTime);
				break;
			case CameraState.LOBBY:
				m_transform.position = Vector3.Lerp(m_transform.position, m_lobbyPosition.position, Time.fixedDeltaTime);
				m_transform.rotation = Quaternion.Lerp(m_transform.rotation, m_lobbyPosition.rotation, Time.fixedDeltaTime);
				break;
			case CameraState.CUSTOMCHARACTER:
				m_transform.position = Vector3.Lerp(m_transform.position, m_customCharacterPosition.position, Time.fixedDeltaTime);
				m_transform.rotation = Quaternion.Lerp(m_transform.rotation, m_customCharacterPosition.rotation, Time.fixedDeltaTime);
				break;

			default:
				break;
		}
	}

	private void OnDisable()
	{
		Utils.OnJoinLobbySuccessEvent -= OnJoinLobbyEvent;
		Utils.OnLeaveLobbySuccessEvent -= OnLeaveLobbyEvent;
		Utils.OnEnterCustomCharacterEvent -= OnEnterCustomCharacterEvent;
		Utils.OnLeaveCustomCharacterEvent -= OnLeaveCustomCharacterEvent;
	}

	private void OnJoinLobbyEvent()
	{
		m_state = CameraState.LOBBY;
	}

	private void OnLeaveLobbyEvent()
	{
		m_state = CameraState.MAINMENU;
	}
	private void OnEnterCustomCharacterEvent()
	{
		m_state = CameraState.CUSTOMCHARACTER;
	}

	private void OnLeaveCustomCharacterEvent()
	{
		m_state = CameraState.MAINMENU;
	}



	public enum CameraState
	{
			LOBBY,
			MAINMENU,
			CUSTOMCHARACTER
	}
}

