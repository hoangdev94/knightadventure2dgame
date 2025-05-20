
using UnityEngine;

public class Skeleton_Enemy : Enemy

    
{
    protected override void Start()
    {
        base.Start();
        dame = 5;
    }
    public override void Patrol()
    {
        base.Patrol();

        anim.SetBool("isWalking", true);
    }
    public override void FollowPlayer()
    {
        base.FollowPlayer();
        anim.SetBool("isWalking", true);
    }
    public override void EnemyHit(float _dameDone)
    {
        base.EnemyHit(_dameDone);
   
    }
}
