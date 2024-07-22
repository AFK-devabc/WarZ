using TMPro;
using UnityEngine;

public class Hub_ObjectInfomation : HealthbarController
{

	[SerializeField] private TMP_Text nameText;
	[SerializeField] private TMP_Text levelText;


	public void Initialize(BaseHealthBehavior healthBehavior, string name, int level)
	{
		base.Initialize(healthBehavior);
		nameText.text = name;
		levelText.text = level.ToString();

	}
	private void FixedUpdate()
	{
		healthAni.fillAmount = Mathf.Lerp(healthAni.fillAmount, healthBar.fillAmount, Time.fixedDeltaTime);
	}
}
