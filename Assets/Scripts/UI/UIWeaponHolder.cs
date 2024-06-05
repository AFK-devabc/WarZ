using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWeaponHolder : MonoBehaviour
{
	[SerializeField] private Image weaponIcon;
	[SerializeField] private Transform holderTransform;
	[SerializeField] private Transform actualTransform;
	Weapon weapon;
	int weaponIndex;
	private CharacterUIController characterUIController;
	public void Initialize(CharacterUIController i_characterUIController,Weapon i_weapon, int degree, int index)
	{
		characterUIController = i_characterUIController;

		weapon = i_weapon;
		weaponIcon.sprite = weapon.avatar;

		actualTransform.transform.eulerAngles = new Vector3(0, 0, degree);
		holderTransform.transform.eulerAngles = new Vector3(0, 0, -degree);
		weaponIndex = index;
	}

	public void OnWeaponSelected()
	{
		characterUIController.SetWeapon( weaponIndex);
	}
}
