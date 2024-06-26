using UnityEngine;
using UnityEngine.UI;

public class HealthbarController : MonoBehaviour
{
	[SerializeField] protected Image healthBar;
	[SerializeField] protected Image healthAni;

	public void Initialize(BaseHealthBehavior healthBehavior)
	{
		OnHealthChanged(healthBehavior.m_nwCurrent.Value, healthBehavior.m_nwTotal.Value);
		healthBehavior.m_OnHealthChangedEvent += OnHealthChanged;
	}

	private void FixedUpdate()
	{
		healthAni.fillAmount = Mathf.Lerp(healthAni.fillAmount, healthBar.fillAmount, Time.fixedDeltaTime);
	}

	public virtual void OnHealthChanged(float i_current, float i_total)
	{
		healthBar.fillAmount = (float)i_current / i_total;
	}
}
