using StatSystem;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
	public BaseStats playerStats;

	public PlayerHealth playerHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
		// Default values for player stats.
		// STR DEX MOV
		playerStats = new BaseStats(10, 10, 5, 10);
		// Default max health value for the player.
		playerHealth = GameObject.Find("HeroKnight").GetComponent<PlayerHealth>();
    }

	void Update()
	{
		
	}

	public void attack()
	{
		// Attack logic can be implemented here, using the playerStats to determine damage, attack speed, etc.
		// For example, you could calculate damage based on strength and dexterity:
		int calculatedDamage = damage();
		Debug.Log("Player attacks with " + calculatedDamage + " damage!");
	}

	public int damage()
	{
		return playerStats.strength.value * 2 + playerStats.dexterity.value / 2;
	}
}
