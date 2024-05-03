using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class EnemyController : ObjectController
{
	public MovementBehavior movementBehavior;
	public AttackBehavior attackBehavior;
	public BaseHealthBehavior healthBehavior;
	private void Start()
	{
		healthBehavior.Init(5);
		healthBehavior.m_OnZeroHealthEvent += Dead;
		healthBehavior.m_OnHealthChangedEvent += SetTarget;
	}
	private void Dead()
	{
		gameObject.SetActive(false);
	}
	public void SetTarget(float i_amout, float i_tempAmount)
	{
		movementBehavior.SetTarget(GameObject.Find("Player").transform);
	}	

}


