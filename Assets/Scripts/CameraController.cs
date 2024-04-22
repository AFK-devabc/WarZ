using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private Transform player;

    [SerializeField] private float threshold;

    [SerializeField] private LayerMask mouseCollLayer;
    [SerializeField] private Transform mouse;
    Ray mouseRay;

    //[SerializeField] private float audioVolumn;
    //[SerializeField] private AudioSource backgroundMusic;
    //[SerializeField] private AudioClip backgroundClip;
    private void FixedUpdate()
    {
         mouseRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(mouseRay, out RaycastHit hitInfo, float.MaxValue, mouseCollLayer))
        {
            Vector3 targetDireciton =  - player.position + hitInfo.point;
            mouse.position = hitInfo.point;
            Vector3 direction =( -player.position + hitInfo.point).normalized;

            transform.position =  threshold * direction + player.position;
        }
    }
    private void Start()
    {
        //backgroundMusic.clip = backgroundClip;
        //backgroundMusic.Play();
    }
    void Awake()
    {
        //GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    void OnDestroy()
    {
        //GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }
    private void OnGameStateChanged(GameState newGameState)
    {
        //enabled = newGameState == GameState.Gameplay;
    }

}

public enum CameraDirection
{
	Left,
	Top,
	Right,
	Bottom
}
