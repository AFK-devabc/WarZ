public class MinigunAttack : IWeaponAttack
{
	private float lastAttackTime;
	private float startAttackTime;
	public override void Init(ProjectileStats i_stats)
	{
		stat = i_stats;
	}

	public override void OnStartAttack(PlayerAttack i_playerAttack)
	{
		startAttackTime = 0;
	}

	public override void OnHoldingAttack(PlayerAttack i_playerAttack, float dt)
	{
		lastAttackTime += dt;
		startAttackTime += dt;
		if (startAttackTime < stat.startTime) return;

		if (lastAttackTime >= stat.delayTime)
		{
			lastAttackTime = 0;
			i_playerAttack.SpawnBullet();
		}

	}

	public override void OnEndAttack(PlayerAttack i_playerAttack)
	{

	}
};
