using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;

    public Transform attackPoint;
    public LayerMask enemyLayers;

    public float attackRange = 0.5f;
    [SerializeField] private int attackDamage;

    public float attackRate = 2f;
    float nextAttackTime = 0f;

    public Soundeffector soundeffector;

    public GameObject gemRed;
    int gemCount = 0;

    [SerializeField] private ItemData itemData;

    private void Start()
    {
        itemData.data.Find(item => item.name == "rg").onUse.AddListener(RedGem);
    }

    private void OnDestroy()
    {
        itemData.data.Find(item => item.name == "rg").onUse.RemoveListener(RedGem);
    }

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                soundeffector.PlayAttackSound();
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void Attack()
    {
        // Воспроизвести анимацию атаки
        animator.SetTrigger("Attack");

        // Обнаружить врагов в зоне действия атаки
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Наносим им урон
        foreach (Collider2D enemy in hitEnemies)
        {
            var taker = enemy.attachedRigidbody.GetComponent<ITakeDamage>();
            if (taker == null) continue;
            taker.TakeDamage(attackDamage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void RedGem()
    {
        StartCoroutine(DableDamage());
    }

    IEnumerator DableDamage()
    {
        gemCount++;
        gemRed.SetActive(true);
        CheckGems(gemRed);

        attackDamage = attackDamage * 2;
        gemRed.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        yield return new WaitForSeconds(5f);
        StartCoroutine(Invis(gemRed.GetComponent<SpriteRenderer>(), 0.02f));
        yield return new WaitForSeconds(1f);
        attackDamage = attackDamage / 2;

        gemCount--;
        gemRed.SetActive(false);
        CheckGems(gemRed);
    }

    void CheckGems(GameObject obj)
    {
        if (gemCount == 1)
            obj.transform.localPosition = new Vector3(0.54f, 1.21f, obj.transform.localPosition.z);
    }
    IEnumerator Invis(SpriteRenderer spr, float time)
    {
        spr.color = new Color(1f, 1f, 1f, spr.color.a - time * 2);
        yield return new WaitForSeconds(time);
        if (spr.color.a > 0)
            StartCoroutine(Invis(spr, time));
    }
}
