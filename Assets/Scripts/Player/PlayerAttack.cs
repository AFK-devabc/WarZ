using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
	[SerializeField] private NetworkPlayer networkPlayer;
	[SerializeField] public Animator animator;
	public int currentLevel = 0;

	[SerializeField] public Weapon weapon;
	[SerializeField] private Transform weaponHolder;
	public Vector3 target;
	[SerializeField] private Transform shootPoint;
	private IWeaponAttack currentAttackType;
	public bool isAttacking = false;
	private int currentAmmo;
	private bool isLoadingAmmo = false;

	private TopDownCamera camera;

	public Action<int, int, bool, float> OnAmmoChangeEvent;

	public void OnClick(InputValue context)
	{
		if (Utils.IsPointerOverUIObject() || !enabled) return;

		if (currentAmmo <= 0)
		{
			ReloadAmmo();
			return;
		}
		if (context.isPressed)
		{
			isAttacking = true;
			currentAttackType.OnStartAttack(this);
		}

		if (!context.isPressed && isAttacking)
		{
			isAttacking = false;
			currentAttackType.OnEndAttack(this);
		}

	}

	public void ReloadAmmo()
	{
		if (isLoadingAmmo) return;
		animator.SetTrigger("reload");
		currentAmmo = weapon.stats[0].ammo;
		OnAmmoChangeEvent(currentAmmo, weapon.stats[0].ammo, true, weapon.stats[0].reloadTime);
		StartCoroutine(ReloadAmmoCountdown());
	}

	private IEnumerator ReloadAmmoCountdown()
	{
		isLoadingAmmo = true;
		yield return new WaitForSeconds(weapon.stats[currentLevel].reloadTime);
		isLoadingAmmo = false;
	}

	public void EquipWeapon(Weapon weapon)
	{
		this.weapon = weapon;
		currentAttackType = SelectAttack(weapon.type);
		currentAttackType.Init(weapon.stats[0]);
		currentAmmo = weapon.stats[0].ammo;
		OnWeaponEquip();
	}

	private void OnWeaponEquip()
	{
	}

	public void SetCamera(TopDownCamera camera)
	{
		this.camera = camera;
	}

	public void Update()
	{
		if (isAttacking)
		{
			currentAttackType.OnHoldingAttack(this, Time.deltaTime);
		}
	}

	private IWeaponAttack SelectAttack(GUNTYPE i_type)
	{
		switch (i_type)
		{
			case GUNTYPE.RIFLE:
				{
					return new HoldAttack();
				}
			case GUNTYPE.MACHINEGUN:
				{
					return new MinigunAttack();
				}
			case GUNTYPE.SNIPER:
				{
					return new ClickAttack();
				}
			case GUNTYPE.SHOTGUN:
				{
					return new ClickAttack();
				}
			case GUNTYPE.ROCKETLAUNCHER:
				{
					return new AimAttack();
				}
			default: return null;
		}
	}

	public void SpawnBullet()
	{
		if (isLoadingAmmo)
			return;

		networkPlayer.SpawnBullet(shootPoint.position, Quaternion.LookRotation(target - shootPoint.position));
		camera.Shake(weapon.shakeFactor, weapon.shakeDuration);
		currentAmmo--;
		OnAmmoChangeEvent?.Invoke(currentAmmo, weapon.stats[0].ammo, false, weapon.stats[0].reloadTime);
		if (currentAmmo <= 0)
		{
			ReloadAmmo();
		}
	}

}
