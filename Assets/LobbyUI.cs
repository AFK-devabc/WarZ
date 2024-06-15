using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour
{
	[SerializeField] private TMP_Text m_lobbyCodeLabel;

	private LobbyUIMediator m_lobbyUIMediator;

	[SerializeField] private TMP_Text m_readyButtonText;

	public void Inittialize(LobbyUIMediator i_lobbyUIMediator)
	{
		m_lobbyUIMediator = i_lobbyUIMediator;
	}

	private void OnEnable()
	{
		if (m_lobbyUIMediator.m_localLobbyUser.IsHost)
		{
			m_readyButtonText.text = "Start";
		}
		else
		{
			m_readyButtonText.text = "Ready";
		}
	}

	public void OnLocalLobbyChanged(LocalLobby i_localLobby)
	{
		m_lobbyCodeLabel.text = i_localLobby.LobbyCode;
	}

	public void OnLeaveLobbyClicked()
	{
		m_lobbyUIMediator.LeaveLobby();
	}

	public void OnStartButtonClicked()
	{
		if (m_lobbyUIMediator.m_localLobbyUser.IsHost)
		{
			m_lobbyUIMediator.StartGame();
		}
		else
		{
			m_lobbyUIMediator.ChangeReadyState();
		}
	}
}
