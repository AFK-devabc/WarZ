using System;
using Unity.Netcode;
using UnityEngine;


public class BaseHealthBehavior : NetworkBehaviour
{
	public BaseStat m_total { private set; get; }
	public NetworkVariable<float> m_nwTotal { private set; get; } = new NetworkVariable<float>();
	public NetworkVariable<float> m_nwCurrent { private set; get; } = new NetworkVariable<float>();

	public event Action<float, float> m_OnHealthChangedEvent;
	public event Action m_OnZeroHealthEvent;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
	}

	public void Init(float i_total)
	{
		m_total = new BaseStat(StatType.Health, i_total);

		m_nwTotal.Value = i_total;
		m_nwCurrent.Value = i_total;
		m_nwCurrent.OnValueChanged += OnHealthChanged;

		m_OnHealthChangedEvent?.Invoke(m_nwCurrent.Value, m_total.m_finalValue);
	}
	public virtual void ChangeHealth(float i_amout)
	{
		m_nwCurrent.Value = Mathf.Clamp(m_nwCurrent.Value + i_amout, 0, m_total.m_finalValue);
		if (m_nwCurrent.Value == 0)
		{
			m_OnZeroHealthEvent?.Invoke();
		}
	}

	public virtual void OnHealthChanged(float i_oldValue, float i_newValue)
	{
		m_OnHealthChangedEvent?.Invoke(i_newValue, m_total.m_finalValue);
	}

	public virtual void AddModifier(StatModifier i_mod)
	{
		m_total.AddModifier(i_mod);
	}

	public virtual void RemoveModifier(StatModifier i_mod)
	{
		m_total.RemoveModifier(i_mod);
	}
}
