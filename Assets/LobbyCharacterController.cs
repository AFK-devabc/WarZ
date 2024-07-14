using System.Collections.Generic;
using UnityEngine;

public class LobbyCharacterController : MonoBehaviour
{
	[SerializeField] private List<LobbyCharacter> m_LobbyCharactersHolder;
	[SerializeField] private LobbyCharacter m_MMCharacter;

	private LocalLobby m_LocalLobby;

	private void Start()
	{
#if !DEDICATED_SERVER
		ApplicationController applicationController = GameObject.Find("ApplicationController").GetComponent<ApplicationController>();
		m_LocalLobby = applicationController.m_LocalLobby;
		m_MMCharacter.Initialized(applicationController.m_LocalLobbyUser);

		Utils.OnJoinLobbySuccessEvent+= OnJoinedLobby;
		Utils.OnLeaveLobbySuccessEvent += OnLeavedLobby;
#endif
	}

	private void OnDisable()
	{
#if !DEDICATED_SERVER

		Utils.OnJoinLobbySuccessEvent -= OnJoinedLobby;
		Utils.OnLeaveLobbySuccessEvent -= OnLeavedLobby;

		OnLeavedLobby();
#endif
	}

	public void OnJoinedLobby()
	{
		m_LocalLobby.changed += OnLocalLobbyChanged;
		m_MMCharacter.gameObject.SetActive(false);

		for (int i = 0; i < m_LobbyCharactersHolder.Count; i++)
		{
			m_LobbyCharactersHolder[i].gameObject.SetActive(true);
		}

	}

	public void OnLeavedLobby()
	{
		m_LocalLobby.changed -= OnLocalLobbyChanged;
		m_MMCharacter.gameObject.SetActive(true);

		for (int i = 0; i < m_LobbyCharactersHolder.Count; i++)
		{
			m_LobbyCharactersHolder[i].gameObject.SetActive(false);
		}

	}


	public void OnLocalLobbyChanged(LocalLobby i_localLobby)
	{
		for (int i = 0; i < m_LobbyCharactersHolder.Count; i++)
		{
			m_LobbyCharactersHolder[i].Initialized(new LocalLobbyUser());
		}

		int index = 0;
		foreach (var i in i_localLobby.LobbyUsers)
		{
			m_LobbyCharactersHolder[index].Initialized(i.Value);
			index++;
		}
	}


}
