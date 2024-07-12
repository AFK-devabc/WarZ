using TMPro;
using UnityEngine;

public class Hub_EnemyController : HealthbarController
{

	[SerializeField] private TMP_Text nameText;
	[SerializeField] private TMP_Text levelText;

	public Transform camera;
	public Transform transform;

	public void Initialize(BaseHealthBehavior healthBehavior, string name, int level)
	{
		base.Initialize(healthBehavior);
		nameText.text = name;
		levelText.text = level.ToString();

	}
	private void FixedUpdate()
	{
		healthAni.fillAmount = Mathf.Lerp(healthAni.fillAmount, healthBar.fillAmount, Time.fixedDeltaTime);

		if (camera == null)
		{
			if (Utils.camera != null)
				camera = Utils.camera;
		}
		else
		{
			transform.forward =  - camera.forward;
		}
	}
}
