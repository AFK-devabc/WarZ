using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehavior : MonoBehaviour
{
    [SerializeField] private EnemyController controller;

    [SerializeField] protected AttackBase m_CurrentAttack;
    [SerializeField] protected List<AttackBase> m_Attacks = new List<AttackBase>();
    [SerializeField] private Transform transform;
    public bool isAttack;
    public bool isDead;

    protected bool isCombat = false;

    private Transform attackTarget;

    public void Update()
    {
        if (isAttack)
        {
            transform.LookAt(attackTarget.position);
            Attack();
        }

        if (isCombat == true && isAttack == false)
        {
            m_CurrentAttack = GetAttackBase();
            if (m_CurrentAttack != null)
            {
                StartAttack();
            }
        }
    }

    public void StartAttack()
    {
        m_CurrentAttack.StartAttack();
        isAttack = true;
        if(m_CurrentAttack.isStopMove)
        {
            controller.movement.SetCanMove(true);
        }
    }

    public void Attack()
    {
        if (m_CurrentAttack != null)
        {
            m_CurrentAttack.Attack();
        }
    }

    public virtual void StopAttack()
    {
        if (m_CurrentAttack != null)
        {
            m_CurrentAttack.StopAttack();
            isAttack =false;

            if (m_CurrentAttack.isStopMove)
            {
                controller.movement.SetCanMove(false);
            }
            m_CurrentAttack = null;
        }
    }

    public AttackBase GetAttackBase()
    {
        foreach (var attackBase in m_Attacks)
        {
            if (attackBase.isReady())
                return attackBase;
        }
        return null;
    }

    public virtual void AniEventAttack()
    {
        m_CurrentAttack.aniEvent();
    }

    public void SetTarget(Transform target)
    {
        isCombat = true;
        attackTarget = target;

        foreach (var skill in m_Attacks)
        {
            skill.SetTarget(attackTarget);
        }
    }
    public void DisableBehavior()
    {
        this.enabled = false;
    }
}
