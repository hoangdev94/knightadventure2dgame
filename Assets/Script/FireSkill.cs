using UnityEngine;

public class FireSkill : MonoBehaviour
{

    private bool hasCollided = false;

    void Start()
    {
        // Tự hủy sau 5 giây nếu không va chạm gì
        Invoke(nameof(SelfDestroy), 5f);
    }

    private void SelfDestroy()
    {
        if (!hasCollided)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hasCollided = true;

        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.EnemyHit(20);
            }
        }

        Destroy(gameObject);
    }

}
