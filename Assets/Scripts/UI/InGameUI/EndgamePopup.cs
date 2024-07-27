using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndgamePopup : MonoBehaviour
{
	[SerializeField] private TMP_Text m_headerText;
	[SerializeField] private TMP_Text m_messageText;
	[SerializeField] private CanvasGroup m_mainCanvas;
	public void ShowPopopWithMessage(string header, string message)
	{
		m_headerText.text = header;
		m_messageText.text = message;
	}

	public void OnButtonClicked()
	{
		ClientGameController.GetInstance().ClientQuitGame();
	}
}
