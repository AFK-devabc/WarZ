using UnityEngine;


[CreateAssetMenu(fileName = "ProjectilesStats", menuName = "ScriptableObject/ProjectilesStats", order = 1)]


public class ProjectileStats : ScriptableObjectWithId
{
    public float movSpeed;
    public LayerMask hitMask;
    public int damage;
    public float lifeTime;
    public bool shouldPlayHitEffect;
    public float cooldownTime;
}



