using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileManager : MonoBehaviour
{
    private static ProjectileManager Instance = null;

    Dictionary<string, IObjectPool<ProjectileController>> projectilePool;

    private Transform m_transform;

    private void KillObjectInPool(ProjectileController bullet)
    {
        projectilePool[bullet.projectileStats.id].Release(bullet);
    }

    public ProjectileController GetProjectile(ProjectileController projectile)
    {
        if(!projectilePool.ContainsKey(projectile.projectileStats.id))
        {
            projectilePool[projectile.projectileStats.id] = new ObjectPool<ProjectileController>(() =>
            {
                ProjectileController bullet = Instantiate(projectile);
                bullet.InIt(KillObjectInPool);
                return bullet;
            }
        , bullet =>
        {
            bullet.gameObject.SetActive(true);
            bullet.transform.SetParent(null);

        }
        , bullet =>
        {
            bullet.transform.parent = m_transform;
            bullet.gameObject.SetActive(false);

        }
        , bullet => Destroy(bullet.gameObject)
        , false, 20);
        }
        return projectilePool[projectile.projectileStats.id].Get();

    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    private void Start()
    {
        m_transform = this.transform;
        projectilePool = new Dictionary<string, IObjectPool<ProjectileController>>();
        projectilePool.Clear();
        //DontDestroyOnLoad(this);
    }
    public static ProjectileManager GetInstance() { return Instance; }
}
