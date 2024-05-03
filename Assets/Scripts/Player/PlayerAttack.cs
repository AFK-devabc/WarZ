using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Weapon weapon;
    [SerializeField] private Transform weaponHolder;
    [SerializeField] private Transform target;
    [SerializeField] private Transform shootPoint;
    private float reloadTime = 1.5f;
    [SerializeField] private Animator animator;
    private ProjectileManager projectileManager;
    private bool canExcuteAttack;
    //[SerializeField] private AudioSource audio;
    private bool isPaused;

    private int currentAmmo;
    private bool isLoadingAmmo = false;
    [SerializeField] private int totalAmmo;
    [SerializeField] private AudioClip reloadAmmoSound;

    void Awake()
    {
        //GameStateManager.Instance.OnGameStateChanged += OnGameStateChanged;
        isPaused = false;
    }

    void OnDestroy()
    {
        //GameStateManager.Instance.OnGameStateChanged -= OnGameStateChanged;
    }
    private void OnGameStateChanged(GameState newGameState)
    {
        //enabled = newGameState == GameState.Gameplay;
    }


    private void Start()
    {
        projectileManager = ProjectileManager.GetInstance();
		weaponHolder.SetParent(RecursiveFindChild(transform, "Hand_R"));
        //OnWeaponEquip();
        canExcuteAttack = true;
        currentAmmo = totalAmmo;		
    }

	private Transform RecursiveFindChild(Transform parent, string childName)
	{
		foreach (Transform child in parent)
		{
			if (child.name == childName)
			{
				return child;
			}
			else
			{
				Transform found = RecursiveFindChild(child, childName);
				if (found != null)
				{
					return found;
				}
			}
		}
		return null;
	}


	public void OnClick(InputValue context)
    {
        if (context.isPressed && canExcuteAttack && enabled&& !isLoadingAmmo)
        {
            if (currentAmmo > 0)
            {
                ProjectileController bullet = projectileManager.GetProjectile(weapon.projectilePrefab);
                //audio.clip = weapon.gunSound;
                //audio.Play();
                bullet.transform.position = shootPoint.position;
                bullet.transform.forward = shootPoint.forward;
                StartCoroutine(AttackCountdown());
                currentAmmo--;
            }
            else
            {
                ReloadAmmo();
            }
        }
    }
    public void ReloadAmmo()
    {
		Debug.Log("Reloading ammo");
        isLoadingAmmo = true;
        StartCoroutine(ReloadAmmoCountdown());
        isLoadingAmmo = false;
    }
    public void AddAmmo(int ammo)
    {
        totalAmmo+= ammo;
    }
    private IEnumerator ReloadAmmoCountdown()
    {
        //GetComponent<AudioSource>().clip = reloadAmmoSound;
        //GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = totalAmmo;
    }

    private IEnumerator AttackCountdown()
    {
        canExcuteAttack = false;
        yield return new WaitForSeconds(weapon.stats[weapon.level].cooldownTime);
        canExcuteAttack = true;
    }

    public void EquipWeapon( Weapon weapon)
    {
        this.weapon = weapon;
    }

    private void OnWeaponEquip()
    {
        for (var i = weaponHolder.childCount - 1; i >= 0; i--)
        {
            // only destroy tagged object
                Destroy(weaponHolder.GetChild(i).gameObject);
        }

        GameObject newWeapon = Instantiate(weapon.weaponPrefab,weaponHolder);
    }


}
