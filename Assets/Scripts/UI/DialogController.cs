using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogController : MonoBehaviour
{
	[Header("----------COMPONENT-------------")]
	[SerializeField] protected TMP_Text dialogText;
	[SerializeField] private Animator animator;

	private static float wordSpeed = 0.02f;
	private List<string> dialogsText;
	private int currentDialogIndex;

	Coroutine current;
	bool isTyping = false;

	public void StartConversation(List<string> dialogs)
	{
		dialogsText = dialogs;
		currentDialogIndex = -1;
		animator.SetBool("IsIn", true);

		OnNextButtonClicked();
	}

	private IEnumerator StartTypeText()
	{
		string txt = dialogsText[currentDialogIndex];
		dialogText.text = "";
		isTyping = true;
		foreach (char letter in txt.ToCharArray())
		{
			dialogText.text += letter;
			yield return new WaitForSeconds(wordSpeed);
		}
		isTyping = false;
	}
	public void OnNextButtonClicked()
	{
        if (isTyping)
        {
			isTyping = false;
			StopCoroutine(current);
			dialogText.text = dialogsText[currentDialogIndex];
			return;
		}
        currentDialogIndex++;
		if(currentDialogIndex >= dialogsText.Count)
		{
			animator.SetBool("IsIn", false);
			return;
		}
		current = StartCoroutine(StartTypeText());
	}

}
