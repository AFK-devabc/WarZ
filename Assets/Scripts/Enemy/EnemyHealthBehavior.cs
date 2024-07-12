using System.Collections;
using UnityEngine;

public class EnemyHealthBehavior : BaseHealthBehavior
{
	EnemyStats enemyStats;

	public static float flashTime = 0.2f;
	SkinnedMeshRenderer skinnedMeshRenderer;
	private bool isNotFlashing = true;

	[SerializeField] private EnemyController enemyController;

	public override void OnHealthChanged(float i_oldValue, float i_newValue)
	{
		if (IsHost || IsServer)
			enemyController.SetTarget(0f, 0f);
		base.OnHealthChanged(i_oldValue, i_newValue);
		if (isNotFlashing)
		{
			StartCoroutine(FlashCoroutine());
		}
	}
	IEnumerator FlashCoroutine()
	{
		isNotFlashing = false;
		if (skinnedMeshRenderer == null)
			skinnedMeshRenderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
		skinnedMeshRenderer.material = enemyStats.flashMaterial;
		yield return new WaitForSeconds(flashTime);
		skinnedMeshRenderer.material = enemyStats.normalMaterial;
		isNotFlashing = true;
	}

	public void InitEnemy(EnemyStats i_enemyStats)
	{
		enemyStats = i_enemyStats;
		base.Init(enemyStats.totalHealth);
	}
}
