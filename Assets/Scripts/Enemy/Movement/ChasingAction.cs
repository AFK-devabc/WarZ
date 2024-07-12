using UnityEngine;

public class ChasingAction : MoveAction
{
	public override void Move(Transform posi, Transform desPosi, float speed)
	{
		if (posi == null) return;
		Vector3 direction = desPosi.position - posi.position;
		if (direction.sqrMagnitude < 1) return;
		posi.position += direction.normalized * speed * Time.fixedDeltaTime;
		posi.forward = direction;
	}
}
