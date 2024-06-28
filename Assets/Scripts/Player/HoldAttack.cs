public class HoldAttack : IWeaponAttack
{
	private float lastAttackTime;
	public override void Init(ProjectileStats i_stats)
	{
		stat = i_stats;
	}

	public override void OnStartAttack(PlayerAttack i_playerAttack)
	{
		i_playerAttack.SpawnBullet();
		lastAttackTime = 0;
	}

	public override void OnHoldingAttack(PlayerAttack i_playerAttack, float dt)
	{
		lastAttackTime += dt;

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
