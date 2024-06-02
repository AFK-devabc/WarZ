using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public enum TYPE_GUN
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
    public TYPE_GUN type;

    [Header("-----------COMPONENT----------")]
    public Sprite avatar;
    public ProjectileController projectile;
    public GameObject weaponModel;
    public AudioClip gunSound;
    [Header("-----------PROPERTY------------")]
    public int level;
    public List<ProjectileStats> stats;
}
