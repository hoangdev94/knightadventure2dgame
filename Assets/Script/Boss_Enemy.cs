using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Boss_Enemy : Enemy
{
    public Slider HpBarBoss;
    public GameObject coinPrefab;
    protected override void Start()
    {
        base.Start();
        timeDelay = 2;
        dame = 20;
    }
    public override void Patrol()
    {
        if (leftPoint == null || rightPoint == null) return;

        float moveDir = movingRight ? 1 : -1;
        rgb2d.MovePosition(rgb2d.position + new Vector2(moveDir * patrolSpeed * Time.fixedDeltaTime, 0f));

        // Kiểm tra đổi hướng
        if (movingRight && transform.position.x >= rightPos.x)
        {
            movingRight = false;
        }
        else if (!movingRight && transform.position.x <= leftPos.x)
        {
            movingRight = true;
        }
        // Lật hướng sprite
        transform.localScale = new Vector3(movingRight ? -1 : 1, 1, 1);
        anim?.SetBool("isWalking", true);
    }
    public override void FollowPlayer()
    {
        if (target == null) return;

        distance = Vector2.Distance(target.position, transform.position);

        if (distance <= 3f)
        {
            if (Player.Instance.currentHp <= 0) return;
            Attack();
            anim?.SetBool("isWalking", false);
            return;
        }
        Vector2 targetPos = new Vector2(target.position.x, transform.position.y);
        Vector2 direction = (targetPos - rgb2d.position).normalized;
        rgb2d.MovePosition(rgb2d.position + direction * followSpeed * Time.fixedDeltaTime);

        // Lật hướng nhưng không lật hpBar
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        anim.SetBool("isWalking", true);
    }
    public override void EnemyHit(float _dameDone)
    {
        base.EnemyHit(_dameDone);

    }
    public override void UpdateHpBar()
    {
        HpBarBoss.value = currentHp;
    }

      public override IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (coinPrefab != null)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * 0.5f;
                Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            }
        }
        if(itemPrefab != null)
        {
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }


}
