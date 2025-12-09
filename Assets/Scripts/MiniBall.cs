using UnityEngine;

public class MiniBall : MonoBehaviour
{
    public int damage = 10;
    public string enemyTag = "Enemy";

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.CompareTag(enemyTag))
        {
            EnemyController e = col.collider.GetComponent<EnemyController>();
            if (e) e.TakeDamage(damage);

            Destroy(gameObject);
        }

        if (col.collider.CompareTag("Barrier"))
        {
            Destroy(gameObject);
        }
    }
}
