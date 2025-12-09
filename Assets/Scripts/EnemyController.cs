using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("Move Settings")]
    public float moveSpeed = 3f;
    public bool moveNegativeZ = true;

    [Header("Stats")]
    public int maxHP = 20;
    private int currentHP;

    [Header("Health UI")]
    public Image healthFill;

    [Header("Attack")]
    public int damageToPlayer = 5;
    public float attackInterval = 2f;

    private bool isAttacking = false;
    private Coroutine attackCoroutine;

    private Animator anim;

    public float enemyMaxZ = -6;

    void Start()
    {
        currentHP = maxHP;
        anim = GetComponent<Animator>();
        UpdateHealthBar();
    }

    void Update()
    {
        if (!isAttacking)
            MoveAlongZ();
    }

    void MoveAlongZ()
    {
        float direction = moveNegativeZ ? -1f : 1f;

        Vector3 next = transform.position + new Vector3(0, 0, direction * moveSpeed * Time.deltaTime);

        if (next.z <= enemyMaxZ)
        {
            next.z = enemyMaxZ;
            moveSpeed = 0f;
            isAttacking = true;

            anim.SetBool("isMove", false);
            anim.SetBool("isAtack", true);

            if (attackCoroutine == null)
            {
                PlayerController player = FindAnyObjectByType<PlayerController>();
                attackCoroutine = StartCoroutine(AttackPlayer(player));
            }
        }
        else
        {
            anim.SetBool("isMove", true);
        }

        transform.position = next;
    }

    IEnumerator AttackPlayer(PlayerController player)
    {
        while (isAttacking)
        {
            if (player != null)
                GameManager.Instance.DamagePlayer(damageToPlayer);

            yield return new WaitForSeconds(attackInterval);
        }
    }

    public void TakeDamage(int dmg)
    {
        AudioManager.Instance.PlayEnemyHit();
        currentHP -= dmg;
        UpdateHealthBar();

        if (currentHP <= 0)
        {
            Die();
            FlyArcAndDie.Instance.FlyAndDie(this.transform, transform.forward);
        }
        else
        {
            anim.SetBool("isTakeDame", true);
            StartCoroutine(ResetTakeDamage());
        }
    }

    IEnumerator ResetTakeDamage()
    {
        yield return new WaitForSeconds(0.2f);
        anim.SetBool("isTakeDame", false);
    }

    void UpdateHealthBar()
    {
        if (healthFill != null)
            healthFill.fillAmount = (float)currentHP / maxHP;
    }

    void Die()
    {
        gameObject.tag = "Untagged";
        anim.SetBool("isDie", true);

        isAttacking = false;
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);

        Destroy(gameObject, 1f);
    }
}
