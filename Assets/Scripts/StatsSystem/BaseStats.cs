using System;
using System.Collections.Generic;

public class BaseStat
{
	public readonly StatType m_type;
	public readonly float m_baseValue;
	public readonly float m_finalValue;
	private List<StatModifier> m_statModifiers;

	public BaseStat(StatType  i_type,float i_baseValue)
	{
		m_baseValue = m_finalValue = i_baseValue;
		m_statModifiers = new List<StatModifier>();
	}
	public virtual void AddModifier(StatModifier i_mod)
	{
        m_statModifiers.Add(i_mod);
		CalculatefinalValue();
	}

	public virtual void RemoveModifier(StatModifier i_mod)
	{
        m_statModifiers.Remove(i_mod);
        CalculatefinalValue();
    }
 
	private void CalculatefinalValue()
	{
		float m_finalValue = m_baseValue;
		float sumPercentAdd = 0; // This will hold the sum of our "PercentAdd" modifiers

		for (int i = 0; i < m_statModifiers.Count; i++)
		{
			StatModifier mod = m_statModifiers[i];

			if (mod.m_type == StatModType.Flat)
			{
				m_finalValue += mod.m_value;
			}
			else if (mod.m_type == StatModType.PercentAdd) // When we encounter a "PercentAdd" modifier
			{
				sumPercentAdd += mod.m_value; // Start adding together all modifiers of this type

				// If we're at the end of the list OR the next modifer isn't of this type
				if (i + 1 >= m_statModifiers.Count || m_statModifiers[i + 1].m_type!= StatModType.PercentAdd)
				{
					m_finalValue *= 1 + sumPercentAdd; // Multiply the sum with the "m_finalValue", like we do for "PercentMult" modifiers
					sumPercentAdd = 0; // Reset the sum back to 0
				}
			}
			else if (mod.m_type == StatModType.PercentMult) // Percent renamed to PercentMult
			{
				m_finalValue *= 1 + mod.m_value;
			}
		}
		m_finalValue= (float)Math.Round(m_finalValue,4);
	}
}

public class StatModifier
{
	public readonly float m_value;
	public readonly int m_order;
	public readonly StatModType m_type;
	// Change the existing constructor to look like this
	public StatModifier(float i_value, StatModType i_type, int i_order)
	{
		m_value = i_value;
        m_type = i_type;
        m_order = i_order;
	}
	// Add a new constructor that automatically sets a default Order, in case the user doesn't want to manually define it
	public StatModifier(float i_value, StatModType i_type) : this(i_value, i_type, (int)i_type) { }
}

public enum StatModType
{
    Flat,
    PercentAdd, // Add this new type.
    PercentMult, // Change our old Percent type to this.
}

public enum StatType
{
	Health,
	MovementSpeed,
	AttackSpeed,
	AttackDamage
}
