
using UnityEngine;

public class ObjectController : MonoBehaviour
{
	public readonly ObjectStat stat;
}

public struct ObjectStat
{
	public BaseStat m_health;
	public BaseStat m_movementSpeed;
	public BaseStat m_attackDamage;
	public BaseStat m_attackSpeed;

	public ObjectStat(float i_health, float i_movementSpeed, float i_damage, float i_attackSpeed)
	{
		m_health = new BaseStat(StatType.Health, i_health);
		m_movementSpeed = new BaseStat(StatType.MovementSpeed, i_movementSpeed); ;
		m_attackDamage = new BaseStat(StatType.AttackDamage, i_damage); ;
		m_attackSpeed = new BaseStat(StatType.AttackSpeed, i_attackSpeed); ;
	}
	public void AddModifier(StatModifier i_mod, StatType i_type)
	{
		switch (i_type)
		{
			case StatType.Health:
				m_health.AddModifier(i_mod);
				break;
			case StatType.MovementSpeed:
				m_movementSpeed.AddModifier(i_mod);
				break;
			case StatType.AttackSpeed:
				m_attackSpeed.AddModifier(i_mod);
				break;
			case StatType.AttackDamage:
				m_attackDamage.AddModifier(i_mod);
				break;
			default: break;
		}
	}

	public void RemoveModifier(StatModifier i_mod,StatType i_type)
	{
		switch (i_type)
		{
			case StatType.Health:
				m_health.RemoveModifier(i_mod);
				break;
			case StatType.MovementSpeed:
				m_movementSpeed.RemoveModifier(i_mod);
				break;
			case StatType.AttackSpeed:
				m_attackSpeed.RemoveModifier(i_mod);
				break;
			case StatType.AttackDamage:
				m_attackDamage.RemoveModifier(i_mod);
				break;
			default: break;
		}
	}
}