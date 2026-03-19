using UnityEngine;

public class SpawnManager : MonoBehaviour
{
	//Prefab of the monster to spawn
	public GameObject spawnMonster;
	//Array of spawn locations for the monsters
	public Vector3[] spawnLocations;
	//Time between spawns
	public float spawnInterval;
	//Number of monsters to spawn each time
	public float spawnAmount = 1;

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
		for (int i = 0; i < spawnAmount; i++)
		{
			//Instantiate the monster prefab at the specified location
			GameObject NewEnemy = Instantiate(spawnMonster, spawnLocations[Random.Range(0, spawnLocations.Length)], Quaternion.identity);
		}
	}
}
