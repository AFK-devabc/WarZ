using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponHolder", menuName = "ScriptableObject/WeaponHolder", order = 1)]

public class WeaponHolderSO : ScriptableObject
{
	[SerializeField] public List<Weapon> weapons;
}
