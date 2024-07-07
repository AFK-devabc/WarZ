using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NPCTalkDialogSelect : NPCTalkDialogBase
{
	[Header("----------COMPONENT-------------")]
	[SerializeField] protected Text dialogText;
	[SerializeField] protected Image ItemImage;
	[SerializeField] protected GameObject[] Btns;

	protected void Update()
	{
		if (PlayerIsInRange && Input.GetKeyDown(KeyCode.UpArrow) && dialogUI.activeInHierarchy == false)
		{
			dialogUI.SetActive(true);
			StartCoroutine(LoadNPCSelect());
		}
	}

	public override void startDialog()
	{
		base.startDialog();
		StartCoroutine(LoadNPCSelect());
	}

	IEnumerator LoadNPCSelect()
	{
		string txt = dialogText.text;
		dialogText.text = "";
		foreach (char letter in txt.ToCharArray())
		{
			dialogText.text += letter;
			yield return new WaitForSeconds(wordSpeed);
		}

		ItemImage.gameObject.SetActive(true);
		yield return new WaitForSeconds(wordSpeed);

		foreach (GameObject btn in Btns)
		{
			btn.SetActive(true);
			yield return new WaitForSeconds(wordSpeed);
		}
	}

	// event click
	public void OnYesBtn()
	{
		dialogUI.SetActive(false);
		NextDialog();
	}

	public void OnNoBtn()
	{
		dialogUI.SetActive(false);
		NextDialog();
	}
}
