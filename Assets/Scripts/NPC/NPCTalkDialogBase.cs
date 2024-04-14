using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTalkDialogBase : MonoBehaviour
{
    [Header("----------NEXT DIALOG--------------")]
    [SerializeField] NPCTalkDialogBase nextDialog;

    [Header("---------------COMPONENT----------------")]
    [SerializeField] protected GameObject dialogUI;
    [SerializeField] protected GameObject talkUI;

    [Header("------------PROPERTIES-------------")]
    [SerializeField] protected bool PlayerIsInRange = false;
    [SerializeField] protected float wordSpeed;

    public void NextDialog()
    {
        if(nextDialog != null)
        {
            nextDialog.enabled = true;
            nextDialog.startDialog();
            this.enabled = false;
        }
    }

    public virtual void startDialog()
    {
        dialogUI.SetActive(true);
    }

    protected virtual void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            PlayerIsInRange = true;
            //talkUI.SetActive(true);
        }
    }

    protected virtual void OnTriggerExit(Collider collision)
    {
        if (collision.tag == "Player")
        {
            PlayerIsInRange = false;
            //talkUI.SetActive(false);
        }
    }
}
