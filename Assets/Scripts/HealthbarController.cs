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
		OnHealthChanged(1, 1);
		healthBehavior.m_OnHealthChangedEvent += OnHealthChanged;
		if (isAlly)
		{
			healthBar.color = new Color(0.15f, 0.9f, 0.4f,1);
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
