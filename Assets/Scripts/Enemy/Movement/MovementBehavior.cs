using UnityEngine;
using UnityEngine.AI;

public abstract class MovementBehavior : MonoBehaviour
{
    public abstract void Move(NavMeshAgent agent, Transform posi, Transform desPosi);
}
