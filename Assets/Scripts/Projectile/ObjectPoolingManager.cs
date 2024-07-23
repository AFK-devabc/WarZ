using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolingManager : MonoBehaviour
{
	private static ObjectPoolingManager Instance = null;

	Dictionary<string, IObjectPool<ObjectPoolController>> localObjectPool = new Dictionary<string, IObjectPool<ObjectPoolController>>();

	private Transform m_transform;

	private void KillObjectInPool(ObjectPoolController pooledObject)
	{
		if (localObjectPool.ContainsKey(pooledObject.id))
			localObjectPool[pooledObject.id].Release(pooledObject);
		else
			Destroy(pooledObject.gameObject);
	}

	public ObjectPoolController GetObjectInPool(ObjectPoolController pooledObject)
	{
		if (!localObjectPool.ContainsKey(pooledObject.id))
		{
			localObjectPool[pooledObject.id] = new ObjectPool<ObjectPoolController>(() =>
			{
				ObjectPoolController bullet = Instantiate(pooledObject);
				bullet.Init(KillObjectInPool);
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
		, false, pooledObject.numberPrewarm);
		}
		return localObjectPool[pooledObject.id].Get();

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
		localObjectPool.Clear();
		//DontDestroyOnLoad(this);
	}
	public static ObjectPoolingManager GetInstance() { return Instance; }
}
