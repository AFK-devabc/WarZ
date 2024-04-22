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
}


