using Unity.Netcode;
using UnityEngine;

public class EnemyController : NetworkBehaviour
{
	public MovementBehavior movementBehavior;
	public AttackBehavior attackBehavior;
	public EnemyHealthBehavior healthBehavior;

	private SkinnedMeshRenderer skinnedMeshRenderer;

	[SerializeField] private Hub_ObjectInfomation hub_Enemy;
	[SerializeField] private Transform canvasHolder;

	public NetworkVariable<int> enemyStatsIndex = new NetworkVariable<int>(-1);
	[SerializeField] private ObjectStatsHolderSO enemyStats;

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
		enemyStatsIndex.Value = 0;
		movementBehavior.enabled = true;
		attackBehavior.enabled = true;
		healthBehavior.InitializeBaseData(enemyStats.objectStats[enemyStatsIndex.Value].totalHealth);
	}

	private void ClientInitizlize(int oldvalue, int newvalue)
	{
		healthBehavior.InitilizeClientData(enemyStats.objectStats[newvalue].flashTime, enemyStats.objectStats[newvalue].normalMaterial, enemyStats.objectStats[newvalue].flashMaterial);
		AddObjectUI();

		skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
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


