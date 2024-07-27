using System.Collections;
using System.Linq;
using UnityEngine;
public class MovementBehavior : MonoBehaviour
{
	public WorldGrid worldGrid;
	public Transform agentPosition;
	public float force = 1.0f;

	public DijkstraTile lastValidTile;
	public DijkstraTile currentTile;
	public Vector3 moveDir;

	public static float rotateDegree = 45f, avoidanceDistance = 2.0f;
	public static Vector3 offset = new Vector3(0, 1, 0);

	public float delayCaculateTime;
	public static LayerMask avoidanceLayer;
	public static int numRay = 7;
	public Vector3 actualMoveDirection = Vector3.zero;

	[SerializeField] public Animator animator;

	public float damage = 1.0f;
	public float attackRange = 1.0f;
	public float visionRange = 3.0f;
	public LayerMask playerMask;


	public bool isMoving = false;

	private void Start()
	{
		agentPosition = this.transform;
		worldGrid = Utils.worldGrid;
		delayCaculateTime = 0;
	}

	public void Initizlized(float movSpeed, float damage)
	{
		force = movSpeed;
		this.damage = damage;
	}

	// Update is called once per frame

	public void EnableMove()
	{
		animator.SetFloat("speed", force);
		Debug.Log("Enable enemy move");
		isMoving = true;
	}

	public void DisableMove()
	{
		animator.SetFloat("speed", 0);
		isMoving = false;
	}

	void FixedUpdate()
	{

		if (!isMoving)
			return;

		Collider[] result = Physics.OverlapSphere(transform.position, attackRange, playerMask);
		if (result.Count() > 0)
		{
			foreach (Collider coll in result)
			{
				if (coll.ClosestPoint(transform.position).sqrMagnitude > attackRange * attackRange)
				{
					agentPosition.LookAt(coll.transform.position);
					StartCoroutine(AttackCourtine());
					return;
				}
			}
		}

		//Debug.Log("Number of player : " + Utils.m_otherPlayer.Count());

		currentTile = worldGrid.NodeFromWorldPoint(agentPosition.position);
		if (this.lastValidTile == null)
		{
			this.lastValidTile = currentTile;
		}
		if (currentTile.getFlowFieldVector().Equals(Vector2Int.zero))
		{
			Vector2Int flowVector = this.lastValidTile.getVector2d() - currentTile.getVector2d();
			moveDir = new Vector3(flowVector.x, 0, flowVector.y).normalized;
		}
		else
		{
			this.lastValidTile = currentTile;
			Vector2Int flowVector = currentTile.getFlowFieldVector();
			moveDir = new Vector3(flowVector.x, 0, flowVector.y).normalized;
		}

		actualMoveDirection = Vector3.zero;

		for (int i = 0; i < numRay; i++)
		{

			Vector3 direction = Quaternion.Euler(0, -90 + 30 * i, 0) * moveDir;
			if (!Physics.Raycast(agentPosition.position + offset, direction, avoidanceDistance, avoidanceLayer))
			{
				actualMoveDirection += direction;
			}

			if (actualMoveDirection != Vector3.zero)
			{

				agentPosition.forward = actualMoveDirection;
			}
		}
		agentPosition.position += actualMoveDirection.normalized * force * Time.fixedDeltaTime;
	}

	public virtual IEnumerator AttackCourtine()
	{
		DisableMove();
		animator.SetTrigger("attack");
		Debug.Log("PlayerAttacked");

		yield return new WaitForSeconds(1.5f);

		Collider[] result = Physics.OverlapBox(agentPosition.position + agentPosition.forward * 0.5f, new Vector3(1.0f, 2f, 1.0f), agentPosition.rotation, playerMask);

		foreach (Collider coll in result)
		{
			coll.GetComponent<BaseHealthBehavior>().ChangeHealth(-damage);
		}

		yield return new WaitForSeconds(2.0f);

		EnableMove();
	}


}
