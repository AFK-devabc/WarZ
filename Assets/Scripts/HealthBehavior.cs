using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class BaseHealthBehavior
{
	private float m_total, m_current, m_sheild;
	public event Action<float,float> m_OnHealthChangedEvent;
	public event m_OnZeroHealthEvent;

	BaseHealthBehavior(float i_total)
	{
		m_total =m_current =i_total;
		m_sheild = 0;
	}

	public void ChangeHealth(float i_amout,float i_tempAmount)
	{
		m_current =mathf.clamp(0,m_current + i_damage,m_total);
		m_OnHealthChangedEvents.Invoke(m_current, m_total);
		if(m_current = 0)
		{
			m_OnZeroHealthEvent.Invoke();
		}
	}
}

public class HealthBehavior : IHealthBehavior
{
}
