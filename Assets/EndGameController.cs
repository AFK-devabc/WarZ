using System.Collections.Generic;
using UnityEngine;

public class EndGameController : MonoBehaviour
{
	[SerializeField] private List<Transform> spawnPoint;
	[SerializeField] private EnemyController zombie;
	private void OnTriggerEnter(Collider other)
	{
		if(other.tag =="Player")
		{
			SpawnEnemy();
		}
	}
	private void SpawnEnemy()
	{
		int number = 20;

		for(int i = 0; i < number; i++) 
		{
			int randomSpawn = Random.Range(0, spawnPoint.Count - 1);
		EnemyController enemy = Instantiate(zombie, spawnPoint[randomSpawn].position, Quaternion.identity);
		}
	}
}
