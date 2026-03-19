using UnityEngine;

public class SpawnManager : MonoBehaviour
{
	//Prefab of the monster to spawn
	public GameObject spawnMonster;
	//Location where the monster will spawn
	public Vector3 spawnLocation;
	//Time between spawns
	public float spawnInterval;

	//Timer to keep track of time between spawns
	private float timer = 0f;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		//Spawning enemies at intervals defined by spawnInterval
		timer += Time.deltaTime;
		if (timer >= spawnInterval)
		{
			SpawnEnemy();
			timer = 0f;
		}

	}

	//Method for spawning enemies
	void SpawnEnemy()
	{
		//Instantiate the monster prefab at the specified location
		GameObject NewEnemy = Instantiate(spawnMonster, spawnLocation, Quaternion.identity);
	}
}
