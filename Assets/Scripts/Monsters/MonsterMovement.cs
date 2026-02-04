using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
	//Monster movement speed
	public float moveSpeed;
	//Scale of the monster sprite
	public float monsterScale;

	//For keeping track of the player position
	public Transform playerTransform;

	/*Knockback related variables for when monster hits the player*/
	//The force of the knockback
	public float KBForce;
	//Timer to track knockback duration
	public float KBCounter;
	//Total duration of the knockback
	public float KBTotalTime;
	//Direction of the knockback
	public bool KnockFromRight;

	public Rigidbody2D monsterRb;

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

	}

	//Physics-related things can be handled here if needed (ex. Movement). Everything else in Update()
	void FixedUpdate()
	{
		//Stop movement when the monster is knocked back
		if (KBCounter <= 0)
		{
			//Move towards the player
			transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
		}
		else
		{
			//Check direction of the knockback
			if (KnockFromRight)
			{
				//Apply knockback force to the right and upwards
				monsterRb.linearVelocity = new Vector2(KBForce, KBForce);
			}
			else
			{
				//Apply knockback force to the left and upwards
				monsterRb.linearVelocity = new Vector2(-KBForce, KBForce);
			}
			//Decrease the knockback timer
			KBCounter -= Time.deltaTime;
		}

	}
}
