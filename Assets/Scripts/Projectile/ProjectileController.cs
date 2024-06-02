using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] public ProjectileStats projectileStats;
    //[SerializeField] protected ParticleSystem hitEffect;  
    protected Action<ProjectileController> _killAction;
    protected float lifeTime = 1;

	private TrailRenderer[] trailRenderers = null;

	public void Init(Action<ProjectileController> killAction)
    {
        _killAction = killAction;
		//GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
		trailRenderers = GetComponentsInChildren<TrailRenderer>();
    }
    private void OnGameStateChanged(GameState newGameState)
    {
        //enabled = newGameState == GameState.Gameplay;
    }

	public void ResetState(Transform shootPoint)
	{
		lifeTime = projectileStats.lifeTime;

		transform.position = shootPoint.position;
		transform.forward = shootPoint.forward;
		foreach (var trailRenderer in trailRenderers)
		{
			trailRenderer.Clear();
		}

	}

	private void OnDestroy()
    {
        //GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }
    private void FixedUpdate()
    {
        RaycastHit hit;
       
        if(lifeTime > 0)
        {
            lifeTime -= Time.fixedDeltaTime;
        }
        else
        {
            _killAction?.Invoke(this);
            //if(projectileStats.shouldPlayHitEffect)
            //    Instantiate(hitEffect, transform.position, Quaternion.identity);
            return;
        }

        if (Physics.Raycast(transform.position , transform.forward, out hit, projectileStats.movSpeed * Time.fixedDeltaTime , projectileStats.hitMask)) 
        {
            if (hit.collider != null)
            {
                Debug.Log("hit");
                transform.position = hit.point;
                //if (hitEffect != null)
                //    Instantiate(hitEffect, transform.position, Quaternion.identity);
                //DoDamage(hit);
                _killAction(this);

            }
        }
        else 
        {
            transform.position += projectileStats.movSpeed * Time.fixedDeltaTime * transform.forward;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position+ transform.forward * projectileStats.movSpeed * Time.fixedDeltaTime);
    }
    protected void DoDamage(RaycastHit hit)
    {
		hit.transform.GetComponent<BaseHealthBehavior>()?.ChangeHealth(-projectileStats.damage, 0);
	}
}
