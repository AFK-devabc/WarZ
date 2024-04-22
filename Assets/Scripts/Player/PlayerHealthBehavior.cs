//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerHealthBehavior : HealthBehavior
//{
//    [SerializeField] private PlayerAttack playerAttack;
//    private int defensePoint;
//    [SerializeField] private int maxDefensePoint;
//    private void Start()
//    {
//        healthPoint = maxHealthPoint;
//        defensePoint = maxDefensePoint;
//    }
//    public override void TakeDamage(int damage)
//    {
//        this.healthPoint -= Mathf.Max(0, damage - defensePoint);
//        defensePoint = Mathf.Max(defensePoint - damage,0);
//        TakeDamageEvent?.Invoke();
        
//        if (this.healthPoint <= 0 && !isDead)
//        {
//            Dead();
//        }
//    }
//    public void AddHP(int point)
//    {
//        healthPoint += point;

//    }

//    public void AddMaxHP(int point)
//    {
//        healthPoint = maxHealthPoint += point;
//    }
//    public void AddSheild(int point)
//    {
//        defensePoint += point;
//    }

//    public void AddMaxSheild(int point)
//    {
//        defensePoint = maxDefensePoint += point;
//    }

//    public override void Dead()
//    {
//        if (!isDead)
//        {
//            DeadEvent?.Invoke();
//            if (deadAni.Length > 0)
//                animator.Play(deadAni[Random.Range(0, deadAni.Length)]);
//            playerAttack.enabled = false;
//            StartCoroutine(DeadCooldown());
//        }
//    }
//    protected override IEnumerator DeadCooldown()
//    {
//        isDead  = true;
//        yield return new WaitForSeconds(deadTime);
//        this.gameObject.SetActive(false);
//    }
//}
