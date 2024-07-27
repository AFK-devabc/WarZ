using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMovementBehavior : MovementBehavior
{
	public override IEnumerator AttackCourtine()
	{
		DisableMove();
		animator.SetTrigger("attack");
		Debug.Log("PlayerAttacked");

		yield return new WaitForSeconds(1.0f);

		Collider[] result = Physics.OverlapBox(agentPosition.position + agentPosition.forward * 2.0f, new Vector3(1f, 2f, 1.5f), agentPosition.rotation, playerMask);

		foreach (Collider coll in result)
		{
			coll.GetComponent<BaseHealthBehavior>().ChangeHealth(-damage);
		}

		yield return new WaitForSeconds(1.5f);

		EnableMove();
	}
}