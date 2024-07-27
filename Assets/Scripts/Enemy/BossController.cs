using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class BossController : NetworkBehaviour
{
	public MovementBehavior movementBehavior;
	public BaseHealthBehavior healthBehavior;
	public Collider collider;

	private SkinnedMeshRenderer skinnedMeshRenderer;

	[SerializeField] private Hub_ObjectInfomation hub_Enemy;
	[SerializeField] private Transform canvasHolder;

	[SerializeField] private ObjectStats bossStats;
	[SerializeField] Animator animator;
	private bool isactivated = false;

	[SerializeField] LayerMask zombielayer;

	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		if (IsServer)
		{
			ServerInitialize();
		}
		else
		{
			ClientInitizlize(-1, 0);
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
		movementBehavior.enabled = true;

		float enemySpeed = Random.Range(bossStats.minMovSpeed, bossStats.maxMovSpeed);

		movementBehavior.Initizlized(enemySpeed, bossStats.damage);
		healthBehavior.InitializeBaseData(bossStats.totalHealth);

		healthBehavior.m_OnZeroHealthEvent += Dead;
	}

	private void ClientInitizlize(int oldvalue, int newvalue)
	{
		healthBehavior.InitilizeClientData(bossStats.flashTime, bossStats.normalMaterial, bossStats.flashMaterial);
		AddObjectUI();

		skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
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
		animator.SetTrigger("dead");
		this.GetComponent<NetworkObject>().Despawn(true);
		GameStateManager.GetInstance().EndGame(true);
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

		GameStateManager.GetInstance().GoToNextMission();

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
		yield return new WaitForSeconds(1.0f);

		Collider[] result = Physics.OverlapSphere(this.transform.position, 100f, zombielayer);

		foreach (Collider i in result)
		{
			i.GetComponent<EnemyController>()?.StartActive();
		}

		yield return new WaitForSeconds(3.0f);

		movementBehavior.enabled = true;
		movementBehavior.EnableMove();

	}

	private void OnMouseEnter()
	{
		skinnedMeshRenderer.material = bossStats.hightlightMaterial;
	}

	void OnMouseExit()
	{
		skinnedMeshRenderer.material = bossStats.normalMaterial;
	}

	private void AddObjectUI()
	{
		Hub_ObjectInfomation hub = Instantiate(hub_Enemy, canvasHolder);
		hub.Initialize(healthBehavior, bossStats.name, bossStats.level);
	}
};