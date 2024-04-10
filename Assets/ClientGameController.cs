using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ClientGameController : MonoBehaviour
{
    PlayerInput playerInput;
    [SerializeField]
    private InputActionReference ecsRef;
    private void Awake()
    {
        ecsRef.action.performed += OnEcsbuttonPressed;
    }

    private void Start()
    {
        SceneManager.LoadSceneAsync("InGameUI");
    }
    private void OnEcsbuttonPressed(InputAction.CallbackContext callback)
    {
        if(!SceneManager.GetSceneByName("InGameUI").isLoaded)
            SceneManager.LoadSceneAsync("InGameUI");
        else
            SceneManager.UnloadSceneAsync("InGameUI");
    }



}
