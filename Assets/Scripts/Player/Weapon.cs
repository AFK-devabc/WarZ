using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

enum TYPE_GUN
{
    AR = 1,
    SR = 2,
    DMR = 3,
    SMR = 4,
    SHOTGUN = 5,
    PISTOL = 6  
}

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObject/Weapon", order = 1)]

public class Weapon : ScriptableObject
{
    public int ID;
    public string Name;
    public int type;
    public int price;

    [Header("-----------COMPONENT----------")]
    public Sprite avatar;
    public ProjectileController projectilePrefab;
    public GameObject weaponPrefab;
    public AudioClip gunSound;
    [Header("-----------PROPERTY------------")]
    public int level;
    public List<ProjectileStats> stats;
    [Header("-----------OWNED------------")]
    public bool owned;
}
