using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [Header("Movement")]
    [SerializeField] public float patrolSpeed = 1f;
    [SerializeField] public float followSpeed = 2f;
    [Header("References")]
    [SerializeField] protected Rigidbody2D rgb2d;
    [SerializeField] protected Animator anim;
    [SerializeField] public Transform leftPoint;
    [SerializeField] public Transform rightPoint;
    [SerializeField] Transform SideAttackTransform;
    [SerializeField] Vector2 SideAttackArea;
    [SerializeField] LayerMask attackAble;
    [SerializeField] protected GameObject itemPrefab; 
    protected float timeDelay = 0.5f;
    private float timeAttack;
    protected Transform target;
    protected bool isPlayerInRange = false;
    protected bool movingRight = true;
    [SerializeField] public float dame;
    public Vector3 leftPos;
    public Vector3 rightPos;
    public float distance;
    private bool isDead = false;
    [SerializeField] protected float maxHP = 50;
    protected float currentHp;
    [SerializeField] protected Image hpBar;
    private bool isAggro = false;
    public virtual void Awake()
    {
        rgb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        target = GameObject.FindWithTag("Player")?.transform;
        // Ghi nhớ vị trí ban đầu của các mốc tuần tra
        if (leftPoint != null && rightPoint != null)
        {
            leftPos = leftPoint.position;
            rightPos = rightPoint.position;
        }
        currentHp = maxHP;
    }
    private void OnDrawGizmos()
    {
        if (leftPoint != null && rightPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(leftPoint.position, 0.1f);
            Gizmos.DrawSphere(rightPoint.position, 0.1f);
            Gizmos.DrawLine(leftPoint.position, rightPoint.position);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
    }

    private void Update()
    {
        EnemyDead();
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        if (isPlayerInRange && target != null)
        {
            FollowPlayer();
        }
        else if(isAggro)
        {
            FollowPlayer();
        }
        else
        {
            Patrol();
        }
    }

    public virtual void EnemyHit(float _damage)
    {
        currentHp -= _damage;
        currentHp = Mathf.Max(currentHp, 0);
        UpdateHpBar();
        isAggro = true;
    }

    public virtual void Patrol()
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
        transform.localScale = new Vector3(movingRight ? 1 : -1, 1, 1);
        hpBar.fillOrigin = movingRight ? (int)Image.OriginHorizontal.Left : (int)Image.OriginHorizontal.Right;
    }


    public virtual void FollowPlayer()
    {
        if (target == null) return;

         distance = Vector2.Distance(target.position, transform.position);

        if (distance <= 1.5f)
        {
            if (Player.Instance.currentHp <= 0) return;
            else
            {
                Attack();
                anim.SetBool("isWalking", false);
                return;
            }
        }

        Vector2 targetPos = new Vector2(target.position.x, transform.position.y);
        Vector2 direction = (targetPos - rgb2d.position).normalized;

        rgb2d.MovePosition(rgb2d.position + direction * followSpeed * Time.fixedDeltaTime);

        // Lật hướng
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            hpBar.fillOrigin = (int)Image.OriginHorizontal.Right;
        }
        
        else if (direction.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            hpBar.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
            
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
    //Attacking

    public virtual void Attack()
    {
        timeAttack += Time.deltaTime;
        if (timeAttack < timeDelay) return;
        else
        {
            anim.SetTrigger("Attack");
            timeAttack = 0;
        }

    }
    public void DoDamage()
    {
        Hit(SideAttackTransform, SideAttackArea);
    }
    private void Hit(Transform _attackTranfrom, Vector2 _attackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTranfrom.position, _attackArea, 0, attackAble);

        for (int i = 0; i < objectsToHit.Length; i++)
        {
            Player player = objectsToHit[i].GetComponent<Player>();
            if (player != null)
            {
                player.PlayerHit(dame);
               
            }
        }
    }
    public virtual void EnemyDead()
    {
        if(currentHp <=0 && !isDead)
        {
            isDead = true;
            anim.SetBool("isWalking", false);
            anim.SetBool("Action", true);
            anim.SetBool("isDead", true);
            rgb2d.linearVelocity = Vector2.zero;
            StartCoroutine(DestroyAfterDelay(1f));
        }
       
    }
    public virtual IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (itemPrefab!= null)
        {
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
    public virtual void UpdateHpBar()
    {
        hpBar.fillAmount = currentHp / maxHP;
    }


}
