using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;


public class BaseHealthBehavior : NetworkBehaviour
{
	public BaseStat m_total { private set; get; }
	public NetworkVariable<float> m_nwTotal { private set; get; } = new NetworkVariable<float>(0);
	public NetworkVariable<float> m_nwCurrent { private set; get; } = new NetworkVariable<float>(0);

	public event Action<float, float> m_OnHealthChangedEvent;
	public event Action m_OnZeroHealthEvent;

	SkinnedMeshRenderer skinnedMeshRenderer;
	float flashTime = 0.2f;
	private bool isNotFlashing = true;
	private Material normalMaterial;
	private Material flashMaterial;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();

		if (IsClient)
		{
			if (m_nwCurrent.Value != 0)
			{
				OnHealthChanged(m_nwTotal.Value, m_nwCurrent.Value);
			}
			m_nwCurrent.OnValueChanged += OnHealthChanged;
		}
	}

	public void InitializeBaseData(float i_total)
	{
		m_total = new BaseStat(StatType.Health, i_total);
		m_nwTotal.Value = m_total.m_finalValue;
		m_nwCurrent.Value = m_total.m_finalValue;
	}

	public void InitilizeClientData(float i_flashTime, Material i_normalMaterial, Material i_flashMaterial)
	{
		m_OnHealthChangedEvent += ClientOnHealthChanged;
		flashTime = i_flashTime;
		normalMaterial = i_normalMaterial;
		flashMaterial = i_flashMaterial;
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
		if (IsServer) Debug.LogWarning("Server receive NetworkVariables.OnHealthChanged");
		m_OnHealthChangedEvent?.Invoke(i_newValue, m_total.m_finalValue);
	}

	private void ClientOnHealthChanged(float i_oldValue, float i_newValue)
	{
		if (isNotFlashing)
			StartCoroutine(FlashCoroutine());
	}

	IEnumerator FlashCoroutine()
	{
		isNotFlashing = false;
		if (skinnedMeshRenderer == null)
			skinnedMeshRenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
		skinnedMeshRenderer.material = flashMaterial;
		yield return new WaitForSeconds(flashTime);
		skinnedMeshRenderer.material = normalMaterial;
		isNotFlashing = true;
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
