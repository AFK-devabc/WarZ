using UnityEngine;
using UnityEngine.UI;

public class HealthbarController : MonoBehaviour
{
	[SerializeField] protected Image healthBar;
	[SerializeField] protected Image healthAni;
	BaseHealthBehavior healthBehavior;
	public void Initialize(BaseHealthBehavior i_healthBehavior, bool isAlly = false)
	{
		healthBehavior = i_healthBehavior;
		OnHealthChanged(healthBehavior.m_nwCurrent.Value, healthBehavior.m_nwTotal.Value);
		healthBehavior.m_OnHealthChangedEvent += OnHealthChanged;
		if (isAlly)
		{
			healthBar.color = new Color(36 / 255, 219 / 255, 63 / 255);
		}
	}

	private void FixedUpdate()
	{
		healthAni.fillAmount = Mathf.Lerp(healthAni.fillAmount, healthBar.fillAmount, Time.fixedDeltaTime);
	}

	public virtual void OnHealthChanged(float i_current, float i_total)
	{
		healthBar.fillAmount = (float)i_current / i_total;

		if (i_current == 0)
		{
			this.enabled = false;
			healthBehavior.m_OnHealthChangedEvent -= OnHealthChanged;
		}
	}
}
