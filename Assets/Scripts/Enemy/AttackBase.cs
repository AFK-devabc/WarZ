using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBase : MonoBehaviour
{
    [SerializeField] public BaseStat m_damage;
    protected bool isCountDown;
    [SerializeField] public BaseStat m_duration;
    [SerializeField] public BaseStat m_cooldownTime;
    protected float timeCDStart;
    [SerializeField] public bool isRange;
    [SerializeField] public float maxRange;
    [SerializeField] public float minRange;
    [SerializeField] public bool isStopMove;

    protected Transform target;

    protected int aniEventCount;

    protected bool isAttacking = false;

    public virtual void Update()
    {
        if (isCountDown == true)
        {
            if (timeCDStart > m_cooldownTime.m_finalValue)
            {
                isCountDown = false;
                timeCDStart = 0f;
            }
            else timeCDStart += Time.deltaTime;
        }

        // is In range
    }


    public virtual void StopAttack()
    {
        isAttacking = false;
        if (m_cooldownTime.m_finalValue > 0f)
        {
            timeCDStart = Time.time;
            isCountDown = true;
        }
    }
    public virtual void StartAttack()
    {
        isAttacking = true;
    }
    public virtual void Attack() { }
    public virtual void aniEvent() { }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public virtual bool isReady()
    {
        if (!isAttacking && !isCountDown && IsInRange()&& CanAttack())
            return true;
        return false;
    }

    public virtual bool CanAttack() { return true; }

    private bool IsInRange()
    {
        if (target != null)
        {
                float dis = Vector3.Distance(target.position, transform.position);
                if (  dis >= minRange && dis <= maxRange)
                    return true;
        }
        return false;
    }
}
