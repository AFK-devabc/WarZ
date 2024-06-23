using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponContainer", menuName = "ScriptableObject/WeaponContainer", order = 1)]

public class WeaponContainerSO : ScriptableObject
{
	[SerializeField] public List<Weapon> weapons;
}
