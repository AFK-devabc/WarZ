using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCTalkDialogText : NPCTalkDialogBase
{
    [Header("---------------COMPONENT----------------")]
    [SerializeField] protected Text dialogText;
    [SerializeField] protected string[] dialogTexts;

    [Header("------------PROPERTIES---------------")]
    protected int index = 0;
    Coroutine TypingCoroutine;
    bool CoroutineIsRunning;

    protected void Update()
    {
        if (PlayerIsInRange && Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (index != 0)
            {
                NextLine();
            }
            else
            {
                //talkUI.SetActive(false);
                dialogUI.SetActive(true);

                dialogText.text = "";
                TypingCoroutine = StartCoroutine(Typing());
                index += 1;
            }
        }
    }

    public override void startDialog()
    {
        if (index != 0)
        {
            NextLine();
        }
        else
        {
            //talkUI.SetActive(false);
            dialogUI.SetActive(true);

            dialogText.text = "";
            TypingCoroutine = StartCoroutine(Typing());
            index += 1;
        }
    }

    public void NextLine()
    {
        if (CoroutineIsRunning == false)
        {
            if (index < dialogTexts.Length)
            {
                dialogText.text = "";
                StopCoroutine(TypingCoroutine);
                TypingCoroutine = StartCoroutine(Typing());
                index += 1;
            }
            else ZeroText();
        }
        else
        {
            dialogText.text = dialogTexts[index - 1];
            StopCoroutine(TypingCoroutine);
            CoroutineIsRunning = false;
        }
    }

    public void EndLine()
    {

    }

    public void ZeroText()
    {
        dialogText.text = "";
        index = 0;

        dialogUI.SetActive(false);
        NextDialog();
    }

    IEnumerator Typing()
    {
        CoroutineIsRunning = true;
        foreach (char letter in dialogTexts[index].ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
        CoroutineIsRunning = false;
    }

    protected override void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            PlayerIsInRange = true;
            //talkUI.SetActive(true);
        }
    }

    protected override void OnTriggerExit(Collider collision)
    {
        if (collision.tag == "Player")
        {
            PlayerIsInRange = false;
            //talkUI.SetActive(false);
            ZeroText();
        }
    }
}
