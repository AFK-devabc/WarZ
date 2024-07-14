using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "ScriptableObject/EnemyStats", order = 1)]

public class EnemyStats : ScriptableObject
{
	public string Name;
	public int level;
	[Header("-----------COMPONENT----------")]
	public Material normalMaterial;
	public Material hightlightMaterial;
	public Material flashMaterial;
	[Header("-----------PROPERTY------------")]
	public float totalHealth;
	public float damage;
	public float movSpeed;
}

