using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUIController : MonoBehaviour
{
	[SerializeField] private GameObject m_MenuUI;
	[SerializeField] private GameObject m_CreateLobbyUI;
	[SerializeField] private GameObject m_JoinLobbyUI;
	[SerializeField] private GameObject m_CustomCharacterUI;
	[SerializeField] private GameObject m_LobbyUI;
	[SerializeField] private PopupController m_popup;


	[SerializeField] CanvasGroup m_CanvasGroup;
	[SerializeField] GameObject m_LoadingSpinner;


	[SerializeField] private VoidEventChannelSO m_BlockUIEvent;
	[SerializeField] private VoidEventChannelSO m_UnblockUIEvent;

	private void Start()
	{
		m_BlockUIEvent.OnEventRaised += BlockUIAndShowLoadingInProgress;
		m_UnblockUIEvent.OnEventRaised += UnblockUIAndHideLoadingInProgress;

		ShowMainMenuUI();
	}

	private void OnEnable()
	{
		Utils.OnJoinLobbySuccessEvent += ShowLobbyUI;
		Utils.OnLeaveLobbySuccessEvent += ShowMainMenuUI;
		Utils.OnEnterCustomCharacterEvent += ShowCustomCharacterUI;
		Utils.OnLeaveCustomCharacterEvent += ShowMainMenuUI;
	}

	private void OnDisable()
	{
		Utils.OnJoinLobbySuccessEvent -= ShowLobbyUI;
		Utils.OnLeaveLobbySuccessEvent -= ShowMainMenuUI;
		Utils.OnEnterCustomCharacterEvent -= ShowCustomCharacterUI;
		Utils.OnLeaveCustomCharacterEvent -= ShowMainMenuUI;
	}

	private void OnDestroy()
	{
		m_BlockUIEvent.OnEventRaised -= BlockUIAndShowLoadingInProgress;
		m_UnblockUIEvent.OnEventRaised -= UnblockUIAndHideLoadingInProgress;

	}

	public void ShowMainMenuUI()
	{
		m_MenuUI.SetActive(true);
		m_CreateLobbyUI.SetActive(false);
		m_JoinLobbyUI.SetActive(false);
		m_CustomCharacterUI.SetActive(false);
		m_LobbyUI.SetActive(false);
	}

	public void ShowCreateLobbyUI()
	{
		m_MenuUI.SetActive(false);
		m_CreateLobbyUI.SetActive(true);
		m_JoinLobbyUI.SetActive(false);
		m_CustomCharacterUI.SetActive(false);
		m_LobbyUI.SetActive(false);
	}

	public void ShowJoinLobbyUI()
	{
		m_MenuUI.SetActive(false);
		m_CreateLobbyUI.SetActive(false);
		m_JoinLobbyUI.SetActive(true);
		m_CustomCharacterUI.SetActive(false);
		m_LobbyUI.SetActive(false);
	}

	public void ShowCustomCharacterUI()
	{
		m_MenuUI.SetActive(false);
		m_CreateLobbyUI.SetActive(false);
		m_JoinLobbyUI.SetActive(false);
		m_CustomCharacterUI.SetActive(true);
		m_LobbyUI.SetActive(false);
	}

	public void ShowLobbyUI()
	{
		m_MenuUI.SetActive(false);
		m_CreateLobbyUI.SetActive(false);
		m_JoinLobbyUI.SetActive(false);
		m_CustomCharacterUI.SetActive(false);
		m_LobbyUI.SetActive(true);
	}

	public void UnblockUIAndHideLoadingInProgress()
	{
		//this callback can happen after we've already switched to a different scene
		//in that case the canvas group would be null
		if (m_CanvasGroup != null)
		{
			m_CanvasGroup.interactable = true;
			m_LoadingSpinner.SetActive(false);
		}
	}

	public void ShowPopupWithMessage( string header,string message)
	{
		m_CanvasGroup.interactable = false;
		m_popup.gameObject.SetActive(true);
		m_popup.ShowPopopWithMessage(header, message);
	}

	public void BlockUIAndShowLoadingInProgress()
	{
		m_CanvasGroup.interactable = false;
		m_LoadingSpinner.SetActive(true);
	}

	public void OnExitButtonClicked()
	{
		ApplicationController.GetInstance().QuitGame();
	}
}
