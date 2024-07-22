using Unity.Netcode;
using UnityEngine;

public class ProjectileController : NetworkBehaviour
{
	public GameObject gameObject;
	public NetworkObject networkObject;
	[SerializeField] public ProjectileStats projectileStats;
	//[SerializeField] protected ParticleSystem hitEffect;  
	protected float lifeTime = 1;

	private TrailRenderer[] trailRenderers = null;

	ObjectPoolingManager poolingManager;

	public EffectObjectPoolController hitGroundVFX;
	public EffectObjectPoolController hitMetalVFX;
	public EffectObjectPoolController hitEnemyVFX;

	private void Awake()
	{
		trailRenderers = GetComponentsInChildren<TrailRenderer>();
	}

	public void ResetState()
	{
		lifeTime = projectileStats.lifeTime;
	}

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

		foreach (var trailRenderer in trailRenderers)
		{
			trailRenderer.Clear();
		}
	}

	private void FixedUpdate()
	{
		if (IsServer || IsHost)
		{
			RaycastHit hit;

			if (lifeTime > 0)
			{
				lifeTime -= Time.fixedDeltaTime;
			}
			else
			{
			}

			if (Physics.Raycast(transform.position, transform.forward, out hit, projectileStats.movSpeed * Time.fixedDeltaTime, projectileStats.hitMask))
			{
				if (hit.collider != null)
				{
					Debug.Log("hit");
					transform.position = hit.point;
					OnHit(hit);
					KillAction();
					//if (hitEffect != null)
					//    Instantiate(hitEffect, transform.position, Quaternion.identity);
					//DoDamage(hit);
				}
			}
			else
			{
				transform.position += projectileStats.movSpeed * Time.fixedDeltaTime * transform.forward;
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawLine(transform.position, transform.position + transform.forward * projectileStats.movSpeed * Time.fixedDeltaTime);
	}

	protected void DoDamage(RaycastHit hit)
	{
		hit.transform.GetComponent<BaseHealthBehavior>()?.ChangeHealth(-projectileStats.damage);
	}

	public void OnHit(RaycastHit hit)
	{
		OnHitClientRPC(hit.transform.tag, hit.point);
		if (hit.transform.tag == "Zombie")
		{
			Debug.Log("Zombie Hitted");
			hit.transform.GetComponent<BaseHealthBehavior>().ChangeHealth(-projectileStats.damage);
		}
	}

	[ClientRpc]
	public void OnHitClientRPC(string i_tag, Vector3 i_position)
	{
		if (poolingManager == null)
			poolingManager = ObjectPoolingManager.GetInstance();

		switch (i_tag)
		{
			case "ground":
				{
					EffectObjectPoolController effect = (EffectObjectPoolController)poolingManager.GetObjectInPool(hitGroundVFX);
					effect.transform.position = i_position;
					effect.PlayEffect();
					break;
				}
			case "vehicles":
				{
					EffectObjectPoolController effect = (EffectObjectPoolController)poolingManager.GetObjectInPool(hitMetalVFX);
					effect.transform.position = i_position;
					effect.PlayEffect();
					break;
				}

			//case "Enemy":
			//	{
			//		EffectObjectPoolController effect = (EffectObjectPoolController)poolingManager.GetObjectInPool(hitEnemyVFX);
			//		effect.transform.position = i_position;
			//		effect.PlayEffect();
			//		break;
			//	}
			default:
				{
					break;
				}
		}
	}

	public void KillAction()
	{
		foreach (var trailRenderer in trailRenderers)
		{
			trailRenderer.Clear();
		}

		networkObject.Despawn();
	}
}
