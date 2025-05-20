using Assets.FantasyMonsters.Common.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MonsterController : Enemy 
{
    private Vector3 originalScale;
    public override void Awake()
    {
        // Ghi nhớ scale gốc của prefab (ví dụ 0.3, 0.3, 0.3)
        base.Awake();
        originalScale = transform.localScale;
    }

    public override void Patrol()
    {
        base.Patrol();
        anim.SetBool("isWalking", true);
        // Giữ nguyên tỷ lệ scale, chỉ lật hướng X theo chiều hiện tại
        float facing = transform.localScale.x < 0 ? 1 : -1;
        if (facing == 1)
        {
            hpBar.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
        else
        {
            hpBar.fillOrigin = (int)Image.OriginHorizontal.Right;
        }
        transform.localScale = new Vector3(originalScale.x * facing, originalScale.y, originalScale.z);
    }

    public override void FollowPlayer()
    {
        base.FollowPlayer();
        anim.SetBool("isWalking", true);
        if (target == null) return;
        // Xác định hướng tới player
        float directionX = target.position.x - transform.position.x;
        float facing = directionX < 0 ? 1 : -1;

        if(facing == 1)
        {
            hpBar.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
        else
        {
            hpBar.fillOrigin = (int)Image.OriginHorizontal.Right;
        }

        transform.localScale = new Vector3(originalScale.x * facing, originalScale.y, originalScale.z);

        if (base.distance <= 2.5f)
        {
            base.Attack();
            return;
        }
    }

    public override void EnemyHit(float _dameDone)
    {
        base.EnemyHit(_dameDone);
    }
}
