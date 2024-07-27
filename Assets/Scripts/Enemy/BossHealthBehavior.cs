using UnityEngine;

public class BossHealthBehavior : BaseHealthBehavior
{
	[SerializeField] private BossController bossController;
	public override void ChangeHealth(float i_amout)
	{
		base.ChangeHealth(i_amout);
		if (m_nwCurrent.Value != 0)
			bossController.StartActive(true);
	}

}
