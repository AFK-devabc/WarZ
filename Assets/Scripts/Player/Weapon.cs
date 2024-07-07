using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public enum GUNTYPE
{
	SHOTGUN = 1,
	RIFLE = 2,
	MACHINEGUN = 3,
	ROCKETLAUNCHER = 4,
	SNIPER = 5
}

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObject/Weapon", order = 1)]

public class Weapon : ScriptableObject
{
    public string Name;
    public GUNTYPE type;

    [Header("-----------COMPONENT----------")]
    public Sprite avatar;
	public Sprite ingameAvatar;
    public ProjectileController projectileModel;
    public GameObject weaponModel;
    public List <AudioClip> gunSFXs;
	public ObjectPoolController gunVFX;
	public AnimatorOverrideController animator;
	[Header("-----------PROPERTY------------")]
	public float shakeFactor;
	public float shakeDuration;
    public List<ProjectileStats> stats;
}
