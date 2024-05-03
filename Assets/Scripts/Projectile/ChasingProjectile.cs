using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingProjectile : ProjectileController
{
    [SerializeField] private float target;

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, projectileStats.movSpeed * Time.fixedDeltaTime, projectileStats.hitMask))
        {
            if (hit.collider != null)
            {
                Debug.Log("hit");
                _killAction(this);
                transform.position = hit.point;
                //if (hitEffect != null)
                //    Instantiate(hitEffect, transform.position, Quaternion.identity);
                DoDamage(hit);
            }
        }
        else
        {
            transform.position += projectileStats.movSpeed * Time.fixedDeltaTime * transform.forward;
        }
    }

}
