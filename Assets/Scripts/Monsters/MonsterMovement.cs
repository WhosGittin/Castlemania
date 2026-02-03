using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    public float moveSpeed;

    //For keeping track of the player position
    public Transform playerTransform;

    // Update is called once per frame
    void Update()
    {
        //If the player is to the right of the monster
        if (transform.position.x < playerTransform.position.x)
        {
            //Turn the sprite to face right
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            //Turn the sprite to face left
            transform.localScale = new Vector3(-1, 1, 1);
        }

        //Move towards the player
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
    }
}
