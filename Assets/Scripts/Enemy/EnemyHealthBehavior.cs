using System.Collections;
using UnityEngine;

public class EnemyHealthBehavior : BaseHealthBehavior
{
	[SerializeField] private EnemyController enemyController;
	public override void ChangeHealth(float i_amout)
	{
		base.ChangeHealth(i_amout);
		enemyController.StartActive();
	}

}
