using UnityEngine;

[CreateAssetMenu(fileName = "ObjectStats", menuName = "ScriptableObject/ObjectStats", order = 1)]

public class ObjectStats : ScriptableObject
{
	public string Name;
	public int level;
	[Header("-----------COMPONENT----------")]
	public float flashTime = 0.2f;
	public Material normalMaterial;
	public Material hightlightMaterial;
	public Material flashMaterial;
	[Header("-----------PROPERTY------------")]
	public float totalHealth;
	public float damage;
	public float maxMovSpeed;
	public float minMovSpeed;

}

