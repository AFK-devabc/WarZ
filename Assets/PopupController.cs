using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupController : MonoBehaviour
{
	[SerializeField] private TMP_Text m_headerText;
	[SerializeField] private TMP_Text m_messageText;
	[SerializeField] private CanvasGroup m_mainCanvas;
	public void ShowPopopWithMessage(string header, string message)
	{
		m_headerText.text = header;
		m_messageText.text = message;
	}

	public void HidePopup()
	{
		this.gameObject.SetActive(false);
		m_mainCanvas.interactable = true;
	}
}
