using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    private static PlayerControler instance;

    public static PlayerControler getInstance()
    {
        if(instance == null)
        {
            instance = GameObject.FindObjectOfType<PlayerControler>();
        }

        return instance;
    }

    private void Awake()
    {
        instance = this;
    }


    private void Update()
    {
    }
}
