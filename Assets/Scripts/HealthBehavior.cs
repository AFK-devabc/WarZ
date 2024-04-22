using System;
using UnityEngine;


public class BaseHealthBehavior
{
	private BaseStat m_total;
	private float m_current;
	public event Action<float,float> m_OnHealthChangedEvent;
	public event Action m_OnZeroHealthEvent;

	public BaseHealthBehavior(float i_total)
	{
		m_total = new BaseStat(StatType.Health,i_total);
	}

	public void ChangeHealth(float i_amout,float i_tempAmount)
	{
		m_current =Mathf.Clamp(0,m_current + i_amout, m_total.m_finalValue);
		m_OnHealthChangedEvent.Invoke(m_current, m_total.m_finalValue);
		if(m_current == 0)
		{
			m_OnZeroHealthEvent.Invoke();
		}
	}

    public void AddModifier(StatModifier i_mod)
    {
		m_total.AddModifier(i_mod);
    }

    public void RemoveModifier(StatModifier i_mod)
    {
		m_total.RemoveModifier(i_mod);
    }
}

//public class HealthBehavior : IHealthBehavior
//{
//}
