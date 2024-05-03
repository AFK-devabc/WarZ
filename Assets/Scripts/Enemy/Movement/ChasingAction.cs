using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChasingAction : MoveAction
{
	[SerializeField] private List<Transform> walkAroundPosi;
	private int currentPosition = -1;
	public override void Move(NavMeshAgent agent, Transform posi, Transform desPosi)
	{
		if (desPosi == null)
		{
			if (walkAroundPosi.Count > 0)
			{
				if (currentPosition < 0)
				{
					currentPosition = 0;
					agent.SetDestination(walkAroundPosi[currentPosition].position);
				}
				if (agent.remainingDistance <= agent.stoppingDistance)
				{
					currentPosition++;

					if (currentPosition >= walkAroundPosi.Count)
					{
						currentPosition = 0;
					}
					agent.SetDestination(walkAroundPosi[currentPosition].position);

				}
			}
		}

		else
		{
			agent.SetDestination(desPosi.position);
		}
	}
}
