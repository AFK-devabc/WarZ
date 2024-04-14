using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HealthBehavior : MonoBehaviour
{
    [SerializeField] protected int maxHealthPoint;
    [SerializeField] protected int healthPoint;

    [SerializeField] protected string[] deadAni;
    [HideInInspector] public UnityEvent DeadEvent;
    [SerializeField] protected float deadTime;
    [SerializeField] protected Animator animator;

    [HideInInspector] public UnityEvent TakeDamageEvent;
    protected bool isDead =false;
    protected HealthbarBehavior healthbarBehavior;
    private void Start()
    {
        healthbarBehavior = GetComponentInChildren<HealthbarBehavior>(true);
        healthbarBehavior.SetHealth(healthPoint, maxHealthPoint);
    }
    public virtual void TakeDamage(int damage)
    {
        TakeDamageEvent?.Invoke();
        this.healthPoint -= damage;
        healthbarBehavior.SetHealth(healthPoint, maxHealthPoint);
        if (this.healthPoint <= 0 && !isDead)
        {
            Dead();
        }
    }

    public virtual void Dead()
    {
        isDead = true;
        DeadEvent?.Invoke();
        if (deadAni.Length > 0)
            animator.Play(deadAni[Random.Range(0, deadAni.Length)]);
        StartCoroutine(DeadCooldown());
    }

    protected virtual IEnumerator DeadCooldown()
    {
        yield return new WaitForSeconds(deadTime);
        Destroy(this.gameObject);
    }


    public bool IsDead()
    {
        return isDead;
    }

}
