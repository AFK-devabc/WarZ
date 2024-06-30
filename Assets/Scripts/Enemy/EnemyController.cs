using UnityEngine;

public class EnemyController : ObjectController
{
	public MovementBehavior movementBehavior;
	public AttackBehavior attackBehavior;
	public EnemyHealthBehavior healthBehavior;

	[SerializeField] private EnemyStats enemyStats;

	private SkinnedMeshRenderer skinnedMeshRenderer;

	private void Start()
	{
		healthBehavior.InitEnemy(enemyStats);
		//healthBehavior.m_OnZeroHealthEvent += Dead;
		//healthBehavior.m_OnHealthChangedEvent += SetTarget;
		skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
	}


	private void Dead()
	{
		gameObject.SetActive(false);
	}
	public void SetTarget(float i_amout, float i_tempAmount)
	{
		movementBehavior.SetTarget(GameObject.Find("Player").transform);
	}

	private void OnMouseEnter()
	{
		skinnedMeshRenderer.material = enemyStats.hightlightMaterial;
	}

	void OnMouseExit()
	{
		skinnedMeshRenderer.material = enemyStats.normalMaterial;

	}

};


