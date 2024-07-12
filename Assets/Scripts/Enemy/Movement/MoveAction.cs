using UnityEngine;
using UnityEngine.AI;

public abstract class MoveAction : MonoBehaviour
{
	public abstract void Move(Transform posi, Transform desPosi, float speed);
}
