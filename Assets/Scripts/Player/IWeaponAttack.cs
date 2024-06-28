public abstract class IWeaponAttack
{
	public ProjectileStats stat;
	public abstract void Init(ProjectileStats stats);
	public abstract void OnStartAttack(PlayerAttack i_playerAttack);

	public abstract void OnHoldingAttack(PlayerAttack i_playerAttack, float dt);

	public abstract void OnEndAttack(PlayerAttack i_playerAttack);
};
