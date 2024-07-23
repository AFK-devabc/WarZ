using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Container/ZombieMeshContainerSO")]

public class ZombieMeshContainerSO : ScriptableObject
{
	[SerializeField] public List<Mesh> zombieMesh;
}
