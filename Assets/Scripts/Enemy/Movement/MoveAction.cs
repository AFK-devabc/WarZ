using UnityEngine;
using UnityEngine.AI;

public abstract class MoveAction : MonoBehaviour
{
    public abstract void Move(NavMeshAgent agent, Transform posi, Transform desPosi);
}
