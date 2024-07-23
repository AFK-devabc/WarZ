using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
	[SerializeField] private TMP_Text m_lobbyCodeLabel;

	private LobbyUIMediator m_lobbyUIMediator;

	[SerializeField] private TMP_Text m_readyButtonText;
	[SerializeField] private Button m_readyButton;
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
			m_readyButton.interactable = false;
		}
		else
		{
			m_lobbyUIMediator.ChangeReadyState();
		}
	}

	public void OnSearchForServerFaild()
	{
		m_readyButton.interactable = true;

	}
}
