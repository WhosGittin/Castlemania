using UnityEngine;
using System.Collections;
using TMPro;

public class HeroKnight : MonoBehaviour
{

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip swordSwingSound;
    public AudioClip deathSound;
    public AudioClip pointCollectSound;


    // UUDET SÄÄDÖT PITCHILLE
    [Tooltip("Satunnainen minimi pitch-arvo miekan äänelle (esim. 0.9).")]
    [Range(0.1f, 3.0f)]
    public float minPitch = 0.9f;
    [Tooltip("Satunnainen maksimi pitch-arvo miekan äänelle (esim. 1.1).")]
    [Range(0.1f, 3.0f)]
    public float maxPitch = 1.1f;

    [Header("Movement Settings")]
    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float m_rollForce = 6.0f;

    [Header("Double Jump & Wall Jump")]
    [SerializeField] int maxJumps = 2;
    [SerializeField] float wallJumpForceX = 10f;
    [SerializeField] float wallJumpForceY = 20f;

    [Header("References")]
    [SerializeField] bool m_noBlood = false;
    [SerializeField] GameObject m_slideDust;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_HeroKnight m_groundSensor;
    private Sensor_HeroKnight m_wallSensorR1;
    private Sensor_HeroKnight m_wallSensorR2;
    private Sensor_HeroKnight m_wallSensorL1;
    private Sensor_HeroKnight m_wallSensorL2;

    // Points & respawn
    private int collectedPoints = 0;
    public TMP_Text caText;
    public Transform respawnPoint;

    // Boss damage
    [Header("Attack Settings")]
    public int attackDamage = 25;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask bossLayer;

    // AttackPoint mirroring
    private Vector3 attackPointBaseLocalPos;

    // State
    private bool m_isWallSliding = false;
    private bool m_grounded = false;
    private bool m_rolling = false;
    private int m_facingDirection = 1;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;
    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;

    private int jumpCount = 0;
    private float wallSlideLockTimer = 0f;
    private float wallSlideLockDuration = 0.4f;



    void Start()
    {



        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();

        if (attackPoint != null)
            attackPointBaseLocalPos = attackPoint.localPosition;
    }

    void Update()
    {
        m_timeSinceAttack += Time.deltaTime;

        if (m_rolling)
            m_rollCurrentTime += Time.deltaTime;
        if (m_rollCurrentTime > m_rollDuration)
            m_rolling = false;

        // Landing check
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            jumpCount = 0;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // Leaving ground
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // Input
        float inputX = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) inputX = -1f;
        if (Input.GetKey(KeyCode.RightArrow)) inputX = 1f;

        // Sprite flip + facing direction
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // Mirror attackPoint local position
        if (attackPoint != null)
        {
            attackPoint.localPosition = new Vector3(
                Mathf.Abs(attackPointBaseLocalPos.x) * m_facingDirection,
                attackPointBaseLocalPos.y,
                attackPointBaseLocalPos.z
            );
        }

        // Move
        if (!m_rolling)
            m_body2d.linearVelocity = new Vector2(inputX * m_speed, m_body2d.linearVelocity.y);

        // Animator air speed
        m_animator.SetFloat("AirSpeedY", m_body2d.linearVelocity.y);

        // Handle wall slide
        HandleWallSlide();
        // Jump
        if (Input.GetKeyDown(KeyCode.UpArrow) && !m_rolling)
        {
            if (m_isWallSliding)
            {
                m_animator.SetTrigger("Jump");
                float wallJumpDirection = -m_facingDirection;
                float pushForce = 4f;
                Vector2 wallJumpVelocity = new Vector2(wallJumpDirection * (wallJumpForceX + pushForce), wallJumpForceY);
                m_body2d.linearVelocity = wallJumpVelocity;

                wallSlideLockTimer = wallSlideLockDuration;
                jumpCount = 1;
                m_grounded = false;
                m_animator.SetBool("Grounded", m_grounded);
                m_groundSensor.Disable(0.2f);
            }
            else if (m_grounded || jumpCount < maxJumps)
            {
                if (jumpCount == 0)
                    m_animator.SetTrigger("Jump");
                else if (jumpCount == 1)
                {
                    m_animator.SetTrigger("DoubleJump");
                    m_animator.SetTrigger("Roll");
                    float rollBoost = 2f;
                    m_body2d.linearVelocity = new Vector2(m_body2d.linearVelocity.x + m_facingDirection * rollBoost, m_jumpForce);
                }

                m_grounded = false;
                m_animator.SetBool("Grounded", m_grounded);

                if (jumpCount == 0)
                    m_body2d.linearVelocity = new Vector2(m_body2d.linearVelocity.x, m_jumpForce);

                jumpCount++;
                m_groundSensor.Disable(0.2f);
            }
        }

        // Roll
        if (Input.GetKeyDown(KeyCode.DownArrow) && !m_rolling && !m_isWallSliding)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_body2d.linearVelocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.linearVelocity.y);
        }

        // Attacks
        if (Input.GetKeyDown(KeyCode.A) && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            m_currentAttack++;
            if (m_currentAttack > 3) m_currentAttack = 1;
            if (m_timeSinceAttack > 1.0f) m_currentAttack = 1;

            m_animator.SetTrigger("Attack" + m_currentAttack);
            m_timeSinceAttack = 0.0f;

            Attack(); // apply damage
        }

        // Block
        else if (Input.GetKeyDown(KeyCode.D) && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            m_animator.SetBool("IdleBlock", false);
        }

        // Death / Hurt
        if (Input.GetKeyDown(KeyCode.E) && !m_rolling)
        {
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
        }
        else if (Input.GetKeyDown(KeyCode.Q) && !m_rolling)
        {
            m_animator.SetTrigger("Hurt");
        }

        // Run / Idle Animations
        if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }
        else
        {
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }
    }
private void HandleWallSlide()
{
    if (wallSlideLockTimer > 0f)
    {
        wallSlideLockTimer -= Time.deltaTime;
        m_isWallSliding = false;
        m_animator.SetBool("WallSlide", false);
        return;
    }

    // Sensors already ignore BossHitbox via their LayerMask
    bool touchingRightWall = m_wallSensorR1.State() && m_wallSensorR2.State();
    bool touchingLeftWall  = m_wallSensorL1.State() && m_wallSensorL2.State();

    // Final wall slide condition
    m_isWallSliding = (touchingRightWall || touchingLeftWall) && !m_grounded;

    if (m_isWallSliding)
    {
        m_animator.SetBool("WallSlide", true);
        float slideSpeed = -2f;
        m_body2d.linearVelocity = new Vector2(m_body2d.linearVelocity.x, slideSpeed);
    }
    else
    {
        m_animator.SetBool("WallSlide", false);
    }
}



void AE_SlideDust()
{
    Vector3 spawnPosition = m_facingDirection == 1 
        ? m_wallSensorR2.transform.position 
        : m_wallSensorL2.transform.position;

    if (m_slideDust != null)
    {
        GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation);
        dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
    }
}
    // Respawn and point collection
    void OnCollisionEnter2D(Collision2D col)
    {
       
        if (col.gameObject.tag == "DeadZone")
        {
            transform.position = respawnPoint.position;
             // Reset animator triggers and set Respawn
        m_animator.ResetTrigger("Death");
        m_animator.SetTrigger("Respawn");

        // Force Idle immediately
    m_animator.SetInteger("AnimState", 0);
    

        }

        if (col.gameObject.tag == "Pisteet")
        {
            collectedPoints += 500;
            Destroy(col.gameObject);
            caText.text = collectedPoints.ToString();
            // Soitetaan ääni pisteen kohdalla
            if (pointCollectSound != null)
            {
                AudioSource.PlayClipAtPoint(pointCollectSound, col.transform.position);
            }
        }
    }




    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Checkpoint")
        {
            respawnPoint = col.gameObject.transform;
        }
    }
    // Boss damage application
    void Attack()
    {
        if (attackPoint == null) return;

        Collider2D[] hitBosses = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            bossLayer
        );

        
    }

// UUSI FUNKTIO LOPPULOGIIKKAAN
public int GetCollectedPoints()
{
    // Palauttaa pelaajan keräämät pisteet
    return collectedPoints;
}

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void PlaySwordSwingSound()
    {
        if (audioSource == null)
        {
            UnityEngine.Debug.LogWarning("AudioSource puuttuu pelaajalta, miekkaääntä ei voida toistaa.");
            return;
        }

        if (swordSwingSound == null)
        {
            UnityEngine.Debug.LogWarning("Sword Swing Sound -ääniklippi puuttuu Inspectorista.");
            return;
        }

        // 1. Aseta AudioSourcen pitch satunnaiselle arvolle
        audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);

        // 2. Toista ääni
        audioSource.PlayOneShot(swordSwingSound);

        // HUOM: Jos haluat palauttaa pitchin takaisin normaaliin (1.0) muiden äänien varalta, tee se tässä:
        // audioSource.pitch = 1.0f;
    }
   
        public void PlayDeathSound()
    {
        if (audioSource == null)
        {
            UnityEngine.Debug.LogWarning("AudioSource puuttuu, kuolinääntä ei voida toistaa.");
            return;
        }

        if (deathSound == null)
        {
            UnityEngine.Debug.LogWarning("Death Sound -ääniklippi puuttuu Inspectorista.");
            return;
        }

        // Pysäytä kaikki muut äänet, jotta kuolemaääni kuuluu selvästi
        audioSource.Stop();

        // Toista kuolinääni
        audioSource.PlayOneShot(deathSound);
    }




}





