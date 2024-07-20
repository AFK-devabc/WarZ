using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Container/objectStatHolder")]

public class ObjectStatsHolderSO : ScriptableObject
{
	[SerializeField] public List<ObjectStats> objectStats;
}
