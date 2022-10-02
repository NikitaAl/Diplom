using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour, ITakeDamage
{

    Rigidbody2D rb;
    Animator anim;

    public Transform groundCheck;
    bool isGrounded;

    public float speed;
    public float jumpHeight;
    private float horizontal;

    public int maxHealth = 100;
    public int currentHealth;
    public bool heal;

    public HealthBar healthBar;

    [SerializeField] private GameObject normalCollider;
    [SerializeField] private GameObject deadCollider;

    public Main main;

    int coins;

    bool canHit = true;

    public GameObject gemBlue;
    int gemCount = 0;

    public Inventory inventory;
    public Soundeffector soundeffector;

    [SerializeField] private ItemData itemData;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        itemData.data.Find(item => item.name == "hp").onUse.AddListener(Heart);
        itemData.data.Find(item => item.name == "bg").onUse.AddListener(BlueGem);

    }

    private void OnDestroy()
    {
        itemData.data.Find(item => item.name == "hp").onUse.RemoveListener(Heart);
        itemData.data.Find(item => item.name == "bg").onUse.RemoveListener(BlueGem);
    }

    void Update()
    {
        CheckGround();

        if (isGrounded)
        {
            horizontal = Input.GetAxis("Horizontal");
        }
        else
        {
            horizontal = Mathf.Lerp(horizontal, 0, 1f * Time.deltaTime);
        }
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(transform.up * jumpHeight, ForceMode2D.Impulse);
            anim.SetInteger("State", 3);
        }

        if (Input.GetAxis("Horizontal") == 0)
        {
            anim.SetInteger("State", 1);
        }
        else
        {
            Flip();
            if (isGrounded)
                anim.SetInteger("State", 2);
        }
    }

    void Flip()
    {
        if (Input.GetAxis("Horizontal") > 0)
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        if (Input.GetAxis("Horizontal") < 0)
            transform.localRotation = Quaternion.Euler(0, 180, 0);
    }

    void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 0.2f);
        isGrounded = colliders.Length > 1;
        anim.SetBool("OnGround", isGrounded);
    }

    public void TakeDamage(int damage)
    {
        if (canHit)
        {
            currentHealth -= damage;
            canHit = true;
            healthBar.SetHealth(currentHealth);
            anim.SetTrigger("Hurt");
        }

        if (currentHealth <= 0 && canHit)
        {
            canHit = false;
            Die();
        }

    }

    void Die()
    {
        Debug.Log("Player died");

        anim.SetBool("IsDead", true);

        normalCollider.SetActive(false);
        deadCollider.SetActive(true);

        Invoke("Lose", 1f);
    }

    void Lose()
    {
        main.GetComponent<Main>().Lose();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Coin":
                Destroy(collision.gameObject);
                coins = coins + 5;
                break;
            case "Heart":
                if (inventory.TryPickItem("hp"))
                {
                    Destroy(collision.gameObject);
                }
                break;
            case "BlueGem":
                if (inventory.TryPickItem("bg"))
                {
                    Destroy(collision.gameObject);
                }
                break;
            case "RedGem":
                if (inventory.TryPickItem("rg"))
                {
                    Destroy(collision.gameObject);
                }
                break;
        }
    }

    public void Heart()
    {
        Debug.Log("heal");
        if ( currentHealth >= 100)
        {
            heal = false;

        } else if ( currentHealth >= 90)
        {
            currentHealth = currentHealth + 10;
            heal = true;
        }else 
        {
            currentHealth = currentHealth + 20;
            heal = true;
        }
        healthBar.SetHealth(currentHealth);
    }

    public void BlueGem()
    {
        StartCoroutine(NoHit());
    }

    IEnumerator NoHit()
    {
        gemCount++;
        gemBlue.SetActive(true);
        CheckGems(gemBlue);

        canHit = false;
        gemBlue.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        yield return new WaitForSeconds(5f);
        StartCoroutine(Invis(gemBlue.GetComponent<SpriteRenderer>(), 0.02f));
        yield return new WaitForSeconds(1f);
        canHit = true;

        gemCount--;
        gemBlue.SetActive(false);
        CheckGems(gemBlue);
    }

    void CheckGems(GameObject obj)
    {
        if (gemCount == 1)
            obj.transform.localPosition = new Vector3(-0.48f, 1.21f, obj.transform.localPosition.z);
    }

    IEnumerator Invis(SpriteRenderer spr, float time)
    {
        spr.color = new Color(1f, 1f, 1f, spr.color.a - time * 2);
        yield return new WaitForSeconds(time);
        if (spr.color.a > 0)
            StartCoroutine(Invis(spr, time));
    }

    public int GetCoins()
    {
        return coins;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "Ladder")
        {
            transform.Translate(Vector3.up * Input.GetAxis("Vertical") * speed * 0.3f * Time.deltaTime);
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ladder")
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    } 
}
