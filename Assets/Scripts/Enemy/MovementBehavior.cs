using UnityEngine;
public class MovementBehavior : MonoBehaviour
{
	[SerializeField] protected MoveAction action;

	[SerializeField] private Transform transform;

	private bool isStopped = true;
	protected Transform target;

	public float m_speed;

	[SerializeField] private Animator animator;
	private void FixedUpdate()
	{
		if (!isStopped)
		{
			action.Move(transform, target, m_speed);
		}
	}

	public void SetCanMove(bool isStop)
	{
		isStopped = isStop;
		if (isStop)
		{
		}
		else
		{
		}
	}

	public void SetTarget(Transform target)
	{
		this.target = target;
		isStopped = false;
	}
	public void DisableBehavior()
	{
		this.enabled = false;
		this.GetComponent<Collider>().enabled = false;
	}
	//public void AddModifier(StatModifier i_mod)
	//{
	//	m_speed.AddModifier(i_mod);
	//}

	//public void RemoveModifier(StatModifier i_mod)
	//{
	//	m_speed.RemoveModifier(i_mod);
	//}
}
