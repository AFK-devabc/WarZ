using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehavior : MonoBehaviour
{
    [SerializeField] private EnemyController controller;

    [SerializeField] protected AttackSkill currentSkill;
    [SerializeField] protected List<AttackSkill> skills = new List<AttackSkill>();
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
            currentSkill = GetAttackSkill();
            if (currentSkill != null)
            {
                StartAttack();
            }
        }
    }

    public void StartAttack()
    {
        currentSkill.StartAttack();
        isAttack = true;
        if(currentSkill.isStopMove)
        {
            controller.movement.SetCanMove(true);
        }
    }

    public void Attack()
    {
        if (currentSkill != null)
        {
            currentSkill.Attack();
        }
    }

    public virtual void StopAttack()
    {
        if (currentSkill != null)
        {
            currentSkill.StopAttack();
            isAttack =false;

            if (currentSkill.isStopMove)
            {
                controller.movement.SetCanMove(false);
            }
            currentSkill = null;
        }
    }

    public AttackSkill GetAttackSkill()
    {
        foreach (var skill in skills)
        {
            if (skill.isReady())
                return skill;
        }
        return null;
    }

    public virtual void AniEventAttack()
    {
        currentSkill.aniEvent();
    }

    public void SetTarget(Transform target)
    {
        isCombat = true;
        attackTarget = target;

        foreach (var skill in skills)
        {
            skill.SetTarget(attackTarget);
        }
    }
    public void DisableBehavior()
    {
        this.enabled = false;
    }
}
