using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateLobbyUIController : MonoBehaviour
{
	//[SerializeField] private VoidEventChannelSO m_BtnCreateLobbyClicked;
	//[SerializeField] private VoidEventChannelSO m_BtnJoinLobbyClicked;
	[SerializeField] private VoidEventChannelSO m_BtnBackToMMClicked;

	private void Awake()
	{
		//m_BtnCreateLobbyClicked.OnEventRaised += HideMenuUI;
		//m_BtnJoinLobbyClicked.OnEventRaised += HideMenuUI;
		m_BtnBackToMMClicked.OnEventRaised += HideUI;
	}

	private void HideUI()
	{
		this.gameObject.SetActive(false);
	}

	private void ShowUI()
	{
		this.gameObject.SetActive(true);
	}
}
