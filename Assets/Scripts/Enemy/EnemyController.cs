using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class EnemyController : NetworkBehaviour
{
	public MovementBehavior movementBehavior;
	public EnemyHealthBehavior healthBehavior;
	public Collider collider;

	private SkinnedMeshRenderer skinnedMeshRenderer;

	[SerializeField] private Hub_ObjectInfomation hub_Enemy;
	[SerializeField] private Transform canvasHolder;

	public NetworkVariable<int> enemyStatsIndex = new NetworkVariable<int>(-1);
	[SerializeField] private ObjectStatsHolderSO enemyStats;
	[SerializeField] private ZombieMeshContainerSO zombieMesh;
	[SerializeField] Animator animator;
	private bool isactivated = false;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		if (IsServer)
		{
			ServerInitialize();
		}
		else
		{
			if (enemyStatsIndex.Value != -1)
			{
				ClientInitizlize(-1, enemyStatsIndex.Value);
			}
			enemyStatsIndex.OnValueChanged += ClientInitizlize;
			Utils.AddNewObject(this.transform, ObjectType.NormalEnemy);
		}
	}
	public override void OnNetworkDespawn()
	{
		base.OnNetworkDespawn();
		if (!IsServer)
			Utils.RemoveObject(this.transform);
	}

	private void ServerInitialize()
	{
		enemyStatsIndex.Value = Random.Range(0, enemyStats.objectStats.Count); ;
		movementBehavior.enabled = true;

		float enemySpeed = Random.Range(enemyStats.objectStats[enemyStatsIndex.Value].minMovSpeed, enemyStats.objectStats[enemyStatsIndex.Value].maxMovSpeed);

		movementBehavior.Initizlized(enemySpeed, enemyStats.objectStats[enemyStatsIndex.Value].damage);
		healthBehavior.InitializeBaseData(enemyStats.objectStats[enemyStatsIndex.Value].totalHealth);

		healthBehavior.m_OnZeroHealthEvent += Dead;
	}

	private void ClientInitizlize(int oldvalue, int newvalue)
	{
		healthBehavior.InitilizeClientData(enemyStats.objectStats[newvalue].flashTime, enemyStats.objectStats[newvalue].normalMaterial, enemyStats.objectStats[newvalue].flashMaterial);
		AddObjectUI();

		skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

		skinnedMeshRenderer.sharedMesh = zombieMesh.zombieMesh[Random.Range(0, zombieMesh.zombieMesh.Count)];
	}

	private void Dead()
	{
		movementBehavior.enabled = false;
		healthBehavior.enabled = false;
		collider.enabled = false;

		StartCoroutine(DeadCourtine());
	}

	public IEnumerator DeadCourtine()
	{
		yield return new WaitForSeconds(5.0f);
		this.GetComponent<NetworkObject>().Despawn(true);

	}

	public void SetTarget(float i_amout, float i_tempAmount)
	{
		//attackBehavior.SetTarget(GameObject.Find("Player(Clone)").transform);
	}

	public void StartActive(bool useRoar = false)
	{
		if (isactivated)
			return;
		isactivated = true;
		if (useRoar)
		{
			StartCoroutine(ActiveCoroutine());
		}
		else
		{
			movementBehavior.enabled = true;
			movementBehavior.EnableMove();
		}
	}
	public IEnumerator ActiveCoroutine()
	{
		animator.SetTrigger("roar");
		yield return new WaitForSeconds(1.5f);

		Collider[] result = Physics.OverlapSphere(this.transform.position, 4.0f);

		foreach (Collider i in result)
		{
			i.GetComponent<EnemyController>()?.StartActive();
		}

		movementBehavior.enabled = true;
		movementBehavior.EnableMove();

	}

	private void OnMouseEnter()
	{
		skinnedMeshRenderer.material = enemyStats.objectStats[enemyStatsIndex.Value].hightlightMaterial;
	}

	void OnMouseExit()
	{
		skinnedMeshRenderer.material = enemyStats.objectStats[enemyStatsIndex.Value].normalMaterial;
	}

	private void AddObjectUI()
	{
		Hub_ObjectInfomation hub = Instantiate(hub_Enemy, canvasHolder);
		hub.Initialize(healthBehavior, "", enemyStats.objectStats[enemyStatsIndex.Value].level);
	}
};


