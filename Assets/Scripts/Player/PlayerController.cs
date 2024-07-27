using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private NetworkPlayer networkPlayer;

	[Header("----------------INFO------------------")]
	[SerializeField] protected float speed = 8.0f;
	private static float normalSpeed = 8.0f;
	private static float crounchingSpeed = 4.0f;
	[Header("--------------COMPONENT---------------")]
	[SerializeField] protected Rigidbody rb;
	[SerializeField] public Animator ani;

	public Vector3 velocity = Vector3.zero;
	public bool isMove = true;
	public bool isAttack;
	public bool isDead;

	[Header("---------- Movement ----------")]
	private Vector3 pointToLook;
	[SerializeField] private Transform body;
	private bool isLocking = false;

	[SerializeField] private PlayerAttack playerAttack;

	[SerializeField] private LayerMask targetMask;

	[Header("---------- Dash ----------")]
	private bool m_isCrounching = false;

	public void Update()
	{
		if (!isLocking)
			LookAtMouse();

		if (playerAttack.isAttacking)
		{
			ani.SetFloat("speed", 0);
			return;
		}
		ani.SetFloat("speed", velocity.sqrMagnitude);
		if (velocity.sqrMagnitude > 0)
		{

			Vector2 velocityRight = new Vector2(velocity.z, -velocity.x);

			Vector2 velocityForward = new Vector2(velocity.x, velocity.z);

			Vector2 forward = new Vector2(body.forward.x, body.forward.z);

			float forwardAngle = Vector2.Angle(forward, velocityForward);
			float rightAngle = Vector3.Angle(forward, velocityRight);

			ani.SetFloat("forwardAngle", forwardAngle);
			ani.SetFloat("rightAngle", rightAngle);

		}
	}
	private void FixedUpdate()
	{
		if (playerAttack.isAttacking)
		{
			rb.velocity = Vector3.zero;
			return;
		}

		if (!isLocking)
		{
			rb.velocity = velocity;
		}

	}

	public void OnMove(InputValue context)
	{
		Vector2 temp = context.Get<Vector2>() * speed;
		velocity.x = temp.x;
		velocity.z = temp.y;
	}

	public void OnDash(InputValue context)
	{
		//if (canDash)
		//{
		//	dashVelocity = dashSpeed	* new Vector2(body.forward.x, body.forward.z).normalized;
		//	ani.SetTrigger("dash");
		//	StartCoroutine(DashCooldown());
		//}
	}

	public void OnReload(InputValue context)
	{
		playerAttack.ReloadAmmo();
	}

	public void OnCrounch(InputValue context)
	{
		m_isCrounching = !m_isCrounching;
		if (m_isCrounching)
		{
			speed = crounchingSpeed;
			ani.SetLayerWeight(2, 0);
			ani.SetLayerWeight(3, 1f);
		}
		else
		{
			speed = normalSpeed;
			ani.SetLayerWeight(2, 1f);
			ani.SetLayerWeight(3, 0);
		}
		if (velocity != Vector3.zero)
			velocity = velocity.normalized * speed;

	}

	private void LookAtMouse()
	{
		var (success, hitInfo) = GetMousePosition();
		if (success)
		{
			// Calculate the direction
			pointToLook = hitInfo.point /*- body.position*/;
			if (hitInfo.transform.tag == "Zombie")
			{
				playerAttack.target = hitInfo.collider.bounds.center;
			}
			else
				playerAttack.target = hitInfo.point;
			// You might want to delete this line.
			// Ignore the height difference.
			pointToLook.y = body.position.y;
			// Make the transform look in the direction.
			body.LookAt(pointToLook);
		}
	}

	private (bool success, RaycastHit hitInfo) GetMousePosition()
	{
		if (m_localCamera == null)
			return (success: false, RaycastHit: new RaycastHit());

		var ray = m_localCamera.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out var hitinfo, Mathf.Infinity, targetMask))
		{
			// The Raycast hit something, return with the position.
			return (success: true, position: hitinfo);
		}
		// The Raycast did not hit anything.
		return (success: false, position: hitinfo);
	}

	#region Camera

	private TopDownCamera m_localCameraController;
	private Camera m_localCamera;

	public void SetCamera(TopDownCamera i_localCameraController)
	{
		m_localCameraController = i_localCameraController;
		m_localCamera = m_localCameraController.m_localCamera;

	}

	#endregion //Camera
}
