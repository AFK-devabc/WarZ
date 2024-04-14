using System;
using UnityEngine;
using UnityEngine.AI;
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] protected NavMeshAgent agent;

    [SerializeField] protected MovementBehavior behavior;

    [SerializeField] private Transform transform;

    private bool isStopped = false;
    protected Transform target;

    [SerializeField] private Animator animator;
    private void FixedUpdate()
    {
        if (!isStopped)
        {
            behavior.Move(agent, transform, target);
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

}
