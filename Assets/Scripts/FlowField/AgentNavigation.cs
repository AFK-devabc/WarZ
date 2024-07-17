using UnityEngine;

public class AgentNavigation : MonoBehaviour
{
	public WorldGrid worldGrid;
	private Transform agentPosition;
	//public Rigidbody rb;
	public float force = 1.0f;

	private DijkstraTile lastValidTile;
	DijkstraTile currentTile;
	Vector3 moveDir;
	private void Start()
	{
		agentPosition = this.transform;
		//rb.constraints = RigidbodyConstraints.FreezeRotation;
		//this.lastValidTile = worldGrid.NodeFromWorldPoint(agentPosition.position);
	}

	// Update is called once per frame
	void FixedUpdate()
	{

		currentTile = worldGrid.NodeFromWorldPoint(agentPosition.position);
		if (this.lastValidTile == null)
		{
			this.lastValidTile = currentTile;
		}
		if (currentTile.getFlowFieldVector().Equals(Vector2Int.zero))
		{
			Vector2Int flowVector = this.lastValidTile.getVector2d() - currentTile.getVector2d();
			moveDir = new Vector3(flowVector.x, 0, flowVector.y).normalized;
			//transform.position += moveDir * Time.deltaTime;
		}
		else
		{
			this.lastValidTile = currentTile;
			Vector2Int flowVector = currentTile.getFlowFieldVector();
			moveDir = new Vector3(flowVector.x, 0, flowVector.y).normalized;
		}
		//rb.velocity = moveDir * force;
		agentPosition.position += moveDir * force * Time.fixedDeltaTime;
	}
}
