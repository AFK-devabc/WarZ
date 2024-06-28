public class ClickAttack : IWeaponAttack
{
	public override void Init(ProjectileStats i_stats)
	{
		stat = i_stats;	
	}

	public override void OnStartAttack(PlayerAttack i_playerAttack)
	{
		i_playerAttack.SpawnBullet();
	}

	public override void OnHoldingAttack(PlayerAttack i_playerAttack, float dt)
	{

	}

	public override void OnEndAttack(PlayerAttack i_playerAttack)
	{

	}
};
