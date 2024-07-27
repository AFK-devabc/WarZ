using UnityEngine;

public class EnemyHealthBehavior : BaseHealthBehavior
{
	[SerializeField] private EnemyController enemyController;
	public override void ChangeHealth(float i_amout)
	{
		base.ChangeHealth(i_amout);
		if (m_nwCurrent.Value != 0)
			enemyController.StartActive(true);
	}

}
