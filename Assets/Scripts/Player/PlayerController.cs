using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
	[Header("----------------INFO------------------")]
	[SerializeField] protected float speed;

	[Header("--------------COMPONENT---------------")]
	[SerializeField] protected Rigidbody rb;
	[SerializeField] public Animator ani;

	public Vector3 velocity = Vector3.zero;
	public bool isMove = true;
	public bool isAttack;
	public bool isDead;

	//[Header("---------- Movement ----------")]
	//[SerializeField] private PlayerInput playerInput;
	private Vector3 pointToLook;


	[Header("---------- Dash ----------")]

	[SerializeField] private float dashSpeed;
	[SerializeField] private float dashTime;
	[SerializeField] private float dashCD;
	[SerializeField] private LayerMask hitMask;
	[SerializeField] private Transform body;
	private Vector2 dashVelocity, forward = Vector2.zero, right = Vector2.zero;
	private bool isDashing = false;
	private bool canDash = true;

	private Vector2 directionWithLook = Vector2.zero;

	[SerializeField] private PlayerAttack playerAttack;

	[SerializeField] private Camera mainCamera;
	[SerializeField] private LayerMask targetMask;


	void Awake()
	{
	}

	void OnDestroy()
	{
	}
	private void Start()
	{
	}
	public void Update()
	{
		ani.SetFloat("speed", velocity.sqrMagnitude);
		if (velocity.sqrMagnitude > 0)
		{

			directionWithLook.x = pointToLook.x - transform.position.x;
			directionWithLook.y = pointToLook.z - transform.position.z;

			Vector2 velocityRight = new Vector2(velocity.z, -velocity.x);

			Vector2 velocityForward = new Vector2(velocity.x, velocity.z);

			float forwardAngle = Vector3.Angle(directionWithLook, velocityForward);
			float rightAngle = Vector3.Angle(directionWithLook, velocityRight);

			ani.SetFloat("forwardAngle", forwardAngle);
			ani.SetFloat("rightAngle", rightAngle);

		}
		LookAtMouse();
	}
	private void FixedUpdate()
	{
		if (isDashing)
		{
			rb.velocity = new Vector3(dashVelocity.x, rb.velocity.y, dashVelocity.y);
			RaycastHit hit;
			if (Physics.Raycast(transform.position, rb.velocity.normalized, out hit, rb.velocity.magnitude * Time.fixedDeltaTime, hitMask)) ;
			{
				if (hit.collider)
				{
					transform.position = hit.point;
					dashVelocity = Vector2.zero;
				}
			}

		}
		else
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

	public void Dash(InputAction.CallbackContext context)
	{
		if (canDash)
		{
			dashVelocity = dashSpeed * (new Vector2(velocity.x, velocity.z) != Vector2.zero ? new Vector2(velocity.x, velocity.z).normalized
				: new Vector2(transform.forward.x, transform.forward.z).normalized);
			StartCoroutine(DashCooldown());
		}
	}
	private void LookAtMouse()
	{
		var (success, position) = GetMousePosition();
		if (success)
		{
			// Calculate the direction
			pointToLook = position - transform.position;

			// You might want to delete this line.
			// Ignore the height difference.
			pointToLook.y = 0;

			// Make the transform look in the direction.
			body.forward = pointToLook;
		}
	}
	private IEnumerator DashCooldown()
	{
		canDash = false;
		isDashing = true;
		yield return new WaitForSeconds(dashTime);
		isDashing = false;
		yield return new WaitForSeconds(dashCD);
		canDash = true;
	}

	private (bool success, Vector3 position) GetMousePosition()
	{
		var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, targetMask))
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


}
