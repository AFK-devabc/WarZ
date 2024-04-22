using System;
using UnityEngine;
using UnityEngine.AI;
public class MovementBehavior : MonoBehaviour
{
	[SerializeField] protected NavMeshAgent agent;

	[SerializeField] protected MoveAction action;

	[SerializeField] private Transform transform;

	private bool isStopped = false;
	protected Transform target;

	public readonly BaseStat m_speed;

	[SerializeField] private Animator animator;
	private void FixedUpdate()
	{
		if (!isStopped)
		{
			action.Move(agent, transform, target);
			animator.SetFloat("Speed", agent.velocity.sqrMagnitude);
		}
	}

	public void SetCanMove(bool isStop)
	{
		isStopped = isStop;
		if (isStop)
		{
			agent.enabled = false;
		}
		else
		{
			agent.enabled = true;
		}
	}

	public void SetTarget(Transform target)
	{
		this.target = target;
	}
	public void DisableBehavior()
	{
		this.enabled = false;
		agent.enabled = false;
		this.GetComponent<Collider>().enabled = false;
	}
	public void AddModifier(StatModifier i_mod)
	{
		m_speed.AddModifier(i_mod);
	}

	public void RemoveModifier(StatModifier i_mod)
	{
		m_speed.RemoveModifier(i_mod);
	}
}
