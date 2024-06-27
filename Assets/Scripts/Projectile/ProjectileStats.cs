using UnityEngine;


[CreateAssetMenu(fileName = "ProjectilesStats", menuName = "ScriptableObject/ProjectilesStats", order = 1)]


public class ProjectileStats : ScriptableObjectWithId
{
    public float movSpeed;
    public LayerMask hitMask;
    public int damage;
	public int projectilePerShoot;
	public int ammo;
    public float lifeTime;
	public float startTime;
    public float delayTime;
	public float reloadTime;
}



