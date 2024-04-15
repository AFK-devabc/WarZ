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
    [SerializeField] private AudioSource audio;
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
        OnWeaponEquip();
        canExcuteAttack = true;
        currentAmmo = totalAmmo;
    }


    public void ExcuteAttack(InputAction.CallbackContext context)
    {
        if (context.performed && canExcuteAttack && enabled&& !isLoadingAmmo)
        {
            if (currentAmmo > 0)
            {
                ProjectileController bullet = projectileManager.GetProjectile(weapon.projectilePrefab);
                audio.clip = weapon.gunSound;
                audio.Play();
                bullet.transform.position = shootPoint.position;
                bullet.transform.LookAt(new Vector3(target.position.x, shootPoint.position.y, target.position.z));
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
        audio.clip = reloadAmmoSound;
        audio.Play();
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
