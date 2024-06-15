using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCharacterController : MonoBehaviour
{
	[SerializeField] private List<LobbyCharacter> m_LobbyCharactersHolder;
	[SerializeField] private LobbyCharacter m_MMCharacter;

	private LocalLobby m_LocalLobby;

	[SerializeField] private VoidEventChannelSO m_JoinLobbyEvent;
	[SerializeField] private VoidEventChannelSO m_LeaveLobbyEvent;

	private void Start()
	{
		ApplicationController applicationController = GameObject.Find("ApplicationController").GetComponent<ApplicationController>();
		m_LocalLobby = applicationController.m_LocalLobby;
		m_MMCharacter.Initialized(applicationController.m_LocalLobbyUser);

		m_JoinLobbyEvent.OnEventRaised += OnJoinedLobby;
		m_LeaveLobbyEvent.OnEventRaised += OnLeavedLobby;
	}

	private void OnDisable()
	{
		m_JoinLobbyEvent.OnEventRaised -= OnJoinedLobby;
		m_LeaveLobbyEvent.OnEventRaised -= OnLeavedLobby;

		OnLeavedLobby();
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
		for(int i = 0;i< m_LobbyCharactersHolder.Count;i++)
		{
			m_LobbyCharactersHolder[i].Initialized(new LocalLobbyUser());
		}

		int index = 0;
		foreach(var i in i_localLobby.LobbyUsers)
		{
			m_LobbyCharactersHolder[index].Initialized(i.Value);
			Debug.Log(i.Value.IsReady);
			index++;
		}
	}

	
}
