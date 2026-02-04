using UnityEngine;

public class MonsterDamage : MonoBehaviour
{
	public int damage;
	/*Allow access to other scripts */
	public PlayerHealth playerHealth;
	public MonsterMovement monsterMovement;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			playerHealth.TakeDamage(damage);
			monsterMovement.KBCounter = monsterMovement.KBTotalTime;
			if (collision.transform.position.x <= transform.position.x)
			{
				monsterMovement.KnockFromRight = true;
			}
			else
			{
				monsterMovement.KnockFromRight = false;
			}
		}
	}
}
