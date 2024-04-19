using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BaseStats
{
	public float m_baseValue;
	public float m_finalValue;
	private readonly List<StatModifier> statModifiers;
 
	public CharacterStat(float baseValue)
	{
		BaseValue = baseValue;
		statModifiers = new List<StatModifier>();
	}
	public void AddModifier(StatModifier mod)
	{
		statModifiers.Add(mod);
	}

	public bool RemoveModifier(StatModifier mod)
	{
		return statModifiers.Remove(mod);
	}
 
	private float CalculateFinalValue()
	{
		float finalValue = BaseValue;
		float sumPercentAdd = 0; // This will hold the sum of our "PercentAdd" modifiers

		for (int i = 0; i < statModifiers.Count; i++)
		{
			StatModifier mod = statModifiers[i];

			if (mod.Type == StatModType.Flat)
			{
				finalValue += mod.Value;
			}
			else if (mod.Type == StatModType.PercentAdd) // When we encounter a "PercentAdd" modifier
			{
				sumPercentAdd += mod.Value; // Start adding together all modifiers of this type

				// If we're at the end of the list OR the next modifer isn't of this type
				if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != StatModType.PercentAdd)
				{
					finalValue *= 1 + sumPercentAdd; // Multiply the sum with the "finalValue", like we do for "PercentMult" modifiers
					sumPercentAdd = 0; // Reset the sum back to 0
				}
			}
			else if (mod.Type == StatModType.PercentMult) // Percent renamed to PercentMult
			{
				finalValue *= 1 + mod.Value;
			}
		}
		return (float)Math.Round(finalValue, 4);
	}
}

public class StatModifier
{
	public readonly float Value;
	public readonly int Order;
	public readonly StatModType Type;
// Change the existing constructor to look like this
public StatModifier(float value, StatModType type, int order)
{
    Value = value;
    Type = type;
    Order = order;
}
 
	// Add a new constructor that automatically sets a default Order, in case the user doesn't want to manually define it
	public StatModifier(float value, StatModType type) : this(value, type, (int)type) { }

}

public enum StatModType
{
    Flat,
    PercentAdd, // Add this new type.
    PercentMult, // Change our old Percent type to this.
}

