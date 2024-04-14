using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthBehavior : HealthBehavior
{
    [SerializeField] private Player player;
    [SerializeField] private PlayerAttack playerAttack;
    HUDManager hudManager;
    private int defensePoint;
    [SerializeField] private int maxDefensePoint;
    private void Start()
    {
        healthPoint = maxHealthPoint;
        defensePoint = maxDefensePoint;
        hudManager = HUDManager.getInstance();
        hudManager.setHP(healthPoint);
        hudManager.setDefense(maxDefensePoint);
        hudManager.setMaxDefense(defensePoint);
        hudManager.setMaxHP(maxHealthPoint);
    }
    public override void TakeDamage(int damage)
    {
        this.healthPoint -= Mathf.Max(0, damage - defensePoint);
        defensePoint = Mathf.Max(defensePoint - damage,0);
        TakeDamageEvent?.Invoke();
        
        hudManager.setHP(healthPoint);
        hudManager.setDefense(defensePoint);
        if (this.healthPoint <= 0 && !isDead)
        {
            Dead();
        }
    }
    public void AddHP(int point)
    {
        healthPoint += point;
        hudManager.setHP(healthPoint);

    }

    public void AddMaxHP(int point)
    {
        healthPoint = maxHealthPoint += point;
        hudManager.setHP(healthPoint);
        hudManager.setMaxHP(maxHealthPoint);

    }
    public void AddSheild(int point)
    {
        defensePoint += point;
        hudManager.setDefense(maxDefensePoint);

    }

    public void AddMaxSheild(int point)
    {
        defensePoint = maxDefensePoint += point;
        hudManager.setDefense(maxDefensePoint);
        hudManager.setMaxDefense(defensePoint);
    }

    public override void Dead()
    {
        if (!isDead)
        {
            DeadEvent?.Invoke();
            if (deadAni.Length > 0)
                animator.Play(deadAni[Random.Range(0, deadAni.Length)]);
            player.enabled = false;
            playerAttack.enabled = false;
            StartCoroutine(DeadCooldown());
        }
    }
    protected override IEnumerator DeadCooldown()
    {
        isDead  = true;
        yield return new WaitForSeconds(deadTime);
        this.gameObject.SetActive(false);
        GameManager.Instance.LooseGame();
    }
}
