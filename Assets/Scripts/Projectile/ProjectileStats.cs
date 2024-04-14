using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ProjectilesStats", menuName = "ScriptableObject/ProjectilesStats", order = 1)]


public class ProjectileStats : ScriptableObject
{
    public string id;
    public float movSpeed;
    public LayerMask hitMask;
    public int damage;
    public float lifeTime;
    public bool shouldPlayHitEffect;
    public float cooldownTime;

    public int priceToUpgrape;
}
