using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JoinLobbyUI : MonoBehaviour
{
	private LobbyUIMediator m_lobbyUIMediator;

	[SerializeField] private TMP_InputField m_lobbyCode;

	public void Initialized(LobbyUIMediator i_lobbyUIMediator)
	{
		m_lobbyUIMediator  = i_lobbyUIMediator;
	}

	public void OnJoinLobbyClicked()
	{
		m_lobbyUIMediator.JoinLobbyWithCode(m_lobbyCode.text.ToUpperInvariant());
	}
}
