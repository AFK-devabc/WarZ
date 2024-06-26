using TMPro;
using UnityEngine;


public class PlayerHealthbarController : HealthbarController
{
	[SerializeField] protected TMP_Text healthText;

	public override void OnHealthChanged(float i_current, float i_total)
	{
		healthBar.fillAmount = (float)i_current / i_total;
		healthText.text = ((int)100 * i_current / i_total).ToString() + "%";
	}
}
