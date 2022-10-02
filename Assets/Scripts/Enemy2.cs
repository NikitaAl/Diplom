using UnityEngine;
using UnityEngine.UI;

public class Enemy2 : MonoBehaviour, ITakeDamage
{
    private Transform player;

    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private float speed;
    [SerializeField] private int HP;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int damage;

    [SerializeField] private Transform attackPos;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private float radius;

    private float currentAttackTime = 0;
    [SerializeField] private float attackTime;

    [SerializeField] private GameObject normalCollider;
    [SerializeField] private GameObject deadCollider;

    private bool playerDetected = false;

    [SerializeField] float agroRange;
    [SerializeField] Transform castPoint;

    public HealthBar healthBar;  
    public GameObject healthBarset; 

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        HP = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        // расстояние до игрока
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer < agroRange)
        {
            // код для преследования игрока
            ChasePlayer();
            animator.SetBool("Run", true);
        }
        else
        {
            // прекратите преследовать игрока
            StopChasingPlayer();
            animator.SetBool("Run", false);
        }

        if (playerDetected)
        {
            currentAttackTime += Time.deltaTime;
            if (currentAttackTime >= attackTime )
            {
                currentAttackTime -= attackTime;

                Debug.Log("Start attack");
                OnAttack();
            }
            StopChasingPlayer();
        } 
    }
 
    void ChasePlayer()
    {
        if(transform.position.x < player.position.x)
        {
            
            rb.velocity = new Vector2(speed, 0);
            transform.localScale = new Vector2(1,1);

        }
        else 
        {
            
            rb.velocity = new Vector2(-speed, 0);
            transform.localScale = new Vector2(-1, 1);

        }
    }
   
        /*
        var dir = player.position - transform.position;
        dir.y = 0;
        dir = dir.normalized * speed * Time.deltaTime;
        rb.MovePosition(transform.position + dir);
        
        */    

    void StopChasingPlayer()
    {
        rb.velocity = new Vector2(0, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
        {
            playerDetected = true;
            animator.SetBool("Attack", true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
        {
            playerDetected = false;
            currentAttackTime = 0;
            animator.SetBool("Attack", false);
            animator.SetBool("Run", false);
            
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        if (attackPos != null)
        {
            Gizmos.DrawWireSphere(attackPos.position, radius);
        }
    }

    public void OnAttack()
    {
        Collider2D[] playerCollider = Physics2D.OverlapCircleAll(attackPos.position, radius, playerMask);
        ITakeDamage taker;
        foreach (Collider2D col in playerCollider)
        {
            
            if (col.attachedRigidbody.TryGetComponent<ITakeDamage>(out taker))
            {
                taker.TakeDamage(damage);
                Debug.Log("Attack");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
        healthBar.SetHealth(HP);
        animator.SetTrigger("Hurt");

        if (HP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died");

        animator.SetBool("IsDead", true);
        
        healthBarset.SetActive(false);
        normalCollider.SetActive(false);
        deadCollider.SetActive(true);

        Destroy(this);
    }
    
}
