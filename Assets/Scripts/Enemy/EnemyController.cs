using Unity.Netcode;
using UnityEngine;

public class EnemyController : ObjectController
{
	public MovementBehavior movementBehavior;
	public AttackBehavior attackBehavior;
	public EnemyHealthBehavior healthBehavior;

	[SerializeField] private EnemyStats enemyStats;

	private SkinnedMeshRenderer skinnedMeshRenderer;

	[SerializeField] private Hub_EnemyController hub_Enemy;
	[SerializeField] private Transform canvasHolder;
	private void Start()
	{
		healthBehavior.InitEnemy(enemyStats);
		//healthBehavior.m_OnZeroHealthEvent += Dead;
		//healthBehavior.m_OnHealthChangedEvent += SetTarget;
		skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

		if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsHost)
		{
			AddObjectUI();
		}
		if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
		{
			movementBehavior.enabled = true;
			attackBehavior.enabled = true;
			movementBehavior.m_speed = enemyStats.movSpeed;
		}
	}


	private void Dead()
	{
		gameObject.SetActive(false);
	}
	public void SetTarget(float i_amout, float i_tempAmount)
	{
		movementBehavior.SetTarget(GameObject.Find("Player(Clone)").transform);
		attackBehavior.SetTarget(GameObject.Find("Player(Clone)").transform);
	}

	private void OnMouseEnter()
	{
		skinnedMeshRenderer.material = enemyStats.hightlightMaterial;
	}

	void OnMouseExit()
	{
		skinnedMeshRenderer.material = enemyStats.normalMaterial;

	}

	public void AddObjectUI()
	{
		Hub_EnemyController hub = Instantiate(hub_Enemy, canvasHolder);
		hub.Initialize(healthBehavior, enemyStats.name, enemyStats.level);
	}
};


