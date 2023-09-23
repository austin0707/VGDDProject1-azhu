using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Editor Variables
    [SerializeField]
    [Tooltip("How much health this enemy has")]
    private int m_MaxHealth;

    [SerializeField]
    [Tooltip("How fast the enemy can move")]
    private float m_Speed;

    [SerializeField]
    [Tooltip("How fast the enemy accelerates")]
    private float m_Acceleration;


    [SerializeField]
    [Tooltip("Approximate amount of damage per frame")]
    private float m_Damage;

    [SerializeField]
    [Tooltip("The explosion that occurs when this enemy dies")]
    private ParticleSystem m_DeathExplosion;

    [SerializeField]
    [Tooltip("The probability that this enemy drops a health pill")]
    private float m_HealthPillDropRate;

    [SerializeField]
    [Tooltip("The type of health pill this enemy drops")]
    private GameObject m_HealthPill;

    [SerializeField]
    [Tooltip("How many points killing this enemy provides")]
    private int m_Score;

    [SerializeField]
    [Tooltip("The type of movement algorithm the enemy follows")]
    private int m_MovementType;

    [SerializeField]
    [Tooltip("Cooldown time for movements")]
    private int m_MovementCooldownTime;

    [SerializeField]
    [Tooltip("The type of attack pattern of the enemy")]
    private int m_AttackType;

    [SerializeField]
    [Tooltip("Cooldown time of attacks")]
    private int m_AttackCooldownTime;

    [SerializeField]
    [Tooltip("Attack Game Object")]
    private GameObject m_AttackGO;
    #endregion

    #region Private Variables
    private float p_curHealth;
    private float p_movementTimer;
    private float p_attackTimer;
    private Color p_DefaultColor;
    #endregion

    #region Cached Components
    private Rigidbody cc_Rb;
    #endregion

    #region Cached References
    private Transform cr_Player;
    private Renderer cr_Renderer;
    #endregion

    #region Initialization
    private void Awake()
    {
        p_curHealth = m_MaxHealth;
        p_movementTimer = 0;
        p_attackTimer = 0;

        cc_Rb = GetComponent<Rigidbody>();
        cr_Renderer = GetComponentInChildren<Renderer>();
        p_DefaultColor = cr_Renderer.material.color;

    }
    private void Start()
    {
        cr_Player = FindObjectOfType<PlayerController>().transform;
    }
    #endregion

    #region Main Updates
    private void FixedUpdate()
    {
        if (m_MovementType == 1)
        {
            Vector3 dir = cr_Player.position - transform.position;
            dir.Normalize();
            cc_Rb.MovePosition(cc_Rb.position + dir * m_Speed * Time.fixedDeltaTime);
        } else if (m_MovementType == 2)
        {
            if (p_movementTimer < 0)
            {
                if (transform.position.y < 2)
                {
                    Vector3 dir = cr_Player.position - transform.position;
                    dir.Normalize();
                    dir += new Vector3(0, 3, 0);
                    cc_Rb.AddForce(dir * m_Acceleration);
                }
                else
                {
                    p_movementTimer = m_MovementCooldownTime;
                }
            } else
            {
                p_movementTimer -= Time.fixedDeltaTime;
                Vector3 dir = cr_Player.position - transform.position;
                dir.Normalize();
                cc_Rb.AddForce(dir * m_Speed);
            }
            
        }

        if (m_AttackType == 2)
        {
            if (p_attackTimer < 0)
            {
                GameObject go = Instantiate(m_AttackGO, transform.position, cc_Rb.rotation);
                go.GetComponent<Ability>().Use(transform.position);
                cr_Renderer.material.color = p_DefaultColor;

                p_attackTimer = m_AttackCooldownTime;
            } else
            {
                if (p_attackTimer > 1)
                {
                    Vector3 targetPosition = new Vector3(cr_Player.position.x,
                                       this.transform.position.y,
                                       cr_Player.position.z);
                    this.transform.LookAt(targetPosition);
                } else
                {
                    cr_Renderer.material.color = new Color(255, 0, 0);
                }
                p_attackTimer -= Time.fixedDeltaTime;
            }
        }
        
    }
    #endregion

    #region Collision Methods
    private void OnCollisionStay(Collision collision)
    {
        GameObject other = collision.collider.gameObject;
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().DecreaseHealth(m_Damage);
        }
    }
    #endregion

    #region Health Methods
    public void DecreaseHealth(float amount)
    {
        p_curHealth -= amount;
        if (p_curHealth <= 0)
        {
            ScoreManager.singleton.IncreaseScore(m_Score);
            if (Random.value < m_HealthPillDropRate)
            {
                Instantiate(m_HealthPill, transform.position, Quaternion.identity);
            }
            Instantiate(m_DeathExplosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
    #endregion
}
