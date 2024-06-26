using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD_WeaponUI : MonoBehaviour
{
	[SerializeField] protected TMP_Text bulletText;
	[SerializeField] protected Image weaponImage;
	[SerializeField] protected Image loadingImage;
	protected float reloadTime = 1.0f;
	protected float currentTime = 0.0f;
	public void Initialize(PlayerAttack playerAttack)
	{
		playerAttack.OnAmmoChangeEvent += OnAmmoChanged;
		bulletText.text = playerAttack.weapon.stats[0].ammo + "/" + playerAttack.weapon.stats[0].ammo;
		weaponImage.sprite = playerAttack.weapon.ingameAvatar;
	}
	public void OnAmmoChanged(int i_current, int i_total, bool i_isReload, float i_reloadTime)
	{
		bulletText.text = i_current + "/" + i_total;

		if (i_isReload)
		{
			currentTime = 0;
			reloadTime = i_reloadTime;
		}
	}
	private void FixedUpdate()
	{
		if (currentTime < reloadTime)
		{
			currentTime += Time.fixedDeltaTime;
			loadingImage.fillAmount = 1 - (float)(currentTime / reloadTime);
		}
	}
}
