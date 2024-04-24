using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera m_camera;
    [SerializeField] private Transform m_player;

    [SerializeField] private float m_threshold;

    [SerializeField] private LayerMask m_mouseCollLayer;
	// use this to know where camera hitpoint
    [SerializeField] private Transform m_mouse;
    Ray mouseRay;
	private CameraDirection m_currentDir;
    //[SerializeField] private float audioVolumn;
    //[SerializeField] private AudioSource backgroundMusic;
    //[SerializeField] private AudioClip backgroundClip;
    private void FixedUpdate()
    {
		mouseRay = camera.ScreenPointToRay(Mouse.current.position.ReadValue());
		if (Physics.Raycast(mouseRay, out RaycastHit hitInfo, float.MaxValue, mouseCollLayer))
		{
			if(hitInfo.transform.tag == "Enemy")
			{
				
			}
			else
			{
				Vector3 targetDireciton =  - player.position + hitInfo.point;
				mouse.position = hitInfo.point;
				Vector3 direction =( -player.position + hitInfo.point).normalized;
				transform.position =  threshold * direction + player.position;
			}
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
	Bottom,
	None
}


public class IsometricAiming : MonoBehaviour
{
    #region Datamembers

    #region Editor Settings

        [SerializeField] private LayerMask targetMask;

        #endregion
        #region Private Fields

        private Camera mainCamera;

        #endregion

        #endregion


        #region Methods

        #region Unity Callbacks

        private void Start()
        {
            // Cache the camera, Camera.main is an expensive operation.
            mainCamera = Camera.main;
        }

        private void Update()
        {
            Aim();
        }

        #endregion

        private void Aim()
        {
            var (success, position) = GetMousePosition();
            if (success)
            {
                // Calculate the direction
                var direction = position - transform.position;

                // You might want to delete this line.
                // Ignore the height difference.
                direction.y = 0;

                // Make the transform look in the direction.
                transform.forward = direction;
            }
        }

        private (bool success, Vector3 position) GetMousePosition()
        {
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
            {
                // The Raycast hit something, return with the position.
                return (success: true, position: hitInfo.point);
            }
            else
            {
                // The Raycast did not hit anything.
                return (success: false, position: Vector3.zero);
            }
        }

        #endregion
    }