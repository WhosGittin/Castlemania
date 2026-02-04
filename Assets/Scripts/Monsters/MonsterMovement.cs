using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
	//Monster movement speed
	public float moveSpeed;
	//Scale of the monster sprite
	public float monsterScale;

	//For keeping track of the player position
	public Transform playerTransform;

	public void Start()
	{
		//Find the player in the scene by searching for the GameObject with the "Player" tag
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if (player != null)
		{
			playerTransform = player.transform;
		}
	}

	// Update is called once per frame
	void Update()
	{
		//If the player is to the right of the monster
		if (transform.position.x < playerTransform.position.x)
		{
			//Turn the sprite to face right
			transform.localScale = new Vector3(monsterScale, monsterScale, 1);
		}
		else
		{
			//Turn the sprite to face left
			transform.localScale = new Vector3(-monsterScale, monsterScale, 1);
		}

		//Move towards the player
		transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
	}
}
