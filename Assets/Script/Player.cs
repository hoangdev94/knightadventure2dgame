using System.Collections;
using TMPro;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Rigidbody2D rgb2d;
    public float walkSpeed = 5f;
    public float jumpForce = 5f;
    private Animator anim;
    public float xAxis,yAxis;
    private bool isFacingRight;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    private int airJumpCounter = 0;
    [SerializeField] int maxAirJump;
    private float timedelay = 0.5f;
    private float timeattack;
    [SerializeField] private float timeSkill;
    [SerializeField] private float timeSkillDelay = 10f;
    private bool isGrounded;
    [SerializeField] Transform SideAttackTransform;
    [SerializeField] Vector2 SideAttackArea;
    [SerializeField] LayerMask attackableLayer;
    [SerializeField] float dame;
    [SerializeField] protected float maxHP = 100;
    [SerializeField] protected float maxMana = 100;
    public float currentHp;
    public float currentMana;
    [SerializeField] private Image hpBar;
    [SerializeField] private Image manaBar;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float TimeToDash;
    [SerializeField] private float dashCooldown;
    private bool canDash = true;
    private float gravity;
    PlayerState pState;
    [SerializeField] GameObject DashEffect;
    public GameObject SkillEffect;
    public Transform SkillPos;
    [SerializeField] private float SkillForce = 10f;
    [SerializeField] private Image CooldownSkill;
    [SerializeField] private Image CooldownDash;
    public static Player Instance;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI manaText;
    void Start()
    {
        rgb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pState = GetComponent<PlayerState>();
        currentHp = maxHP;
        currentMana = maxMana;
        gravity = rgb2d.gravityScale;
        InvokeRepeating(nameof(RegenerateMana), 1f, 1f);
        Instance = this;
        CooldownSkill.fillAmount = 1;
        hpText.text = currentHp.ToString() + "/" + maxHP;
        manaText.text = currentMana.ToString() + "/" + maxMana;

    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
    }
    // Update is called once per frame
    void Update()
    {
        if (pState.isDead) return;
        if (pState.dashing) return;
        if (GameManager.Instance.IsGameOver()) return;
        if (pState.isSkilling) return;
        GetInput();
        PlayerMove();
        HandleJump();
        UpdateAnimation();
        StartDashing();
        Attack();
        Skill();
        UpdateCoolDownSkill();
        if (!pState.dashing)
        {
            TimeToDash += Time.deltaTime;
            UpdateCoolDownDash();
        }
    }
    void GetInput()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
    }
    private void PlayerMove()
    {
        rgb2d.linearVelocity = new Vector2(walkSpeed * xAxis, rgb2d.linearVelocity.y);
        if (xAxis <0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            isFacingRight = false;

        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            isFacingRight = true;

        }

    }
    private void HandleJump()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rgb2d.linearVelocity = new Vector2(rgb2d.linearVelocity.x, jumpForce);
            AudioManager.Instance.JumpAudio();

        }
        else if (!isGrounded && Input.GetButtonDown("Jump") && airJumpCounter < maxAirJump)
        {
            rgb2d.linearVelocity = new Vector2(rgb2d.linearVelocity.x, jumpForce);
            AudioManager.Instance.JumpAudio();
            airJumpCounter++;
        }
        
        else if (isGrounded)
        {
            airJumpCounter = 0;
        }
    }
    void StartDashing()
    {
        if (Input.GetButtonDown("Dash") && canDash && isGrounded && currentMana >=10)
        {
            StartCoroutine(Dash());
            AudioManager.Instance.Dash();
            TimeToDash = 0;
        }
    }
    IEnumerator Dash()
    {
        
        canDash = false;
        anim.SetTrigger("Dashing");
        pState.dashing = true;
        rgb2d.gravityScale = 0;
        rgb2d.linearVelocity = new Vector2(transform.localScale.x*dashSpeed, 0);
        if (isGrounded) Instantiate(DashEffect,transform);
        yield return new WaitForSeconds(dashTime);
        rgb2d.gravityScale = gravity;
        pState.dashing = false;
        currentMana -= 10;
        UpdateManaBar();
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;  
    }
    public void UpdateCoolDownDash()
    {
        float ratio = TimeToDash / dashCooldown;
        ratio = Mathf.Clamp01(ratio);
        CooldownDash.fillAmount = 1 - ratio;
        if (ratio >= 1f && currentMana < 10)    
        {
            CooldownDash.fillAmount = 1f;
        }
        else
        {
            CooldownDash.fillAmount = 1 - ratio;
        }
    }
    private void UpdateAnimation()
    {
        bool isRunning = Mathf.Abs(rgb2d.linearVelocity.x) > 0.1f;
        bool isJumpping = !isGrounded;
        anim.SetBool("isWalk", isRunning);
        anim.SetBool("isJump", isJumpping);

    }
    private void Attack()
    {
        timeattack += Time.deltaTime;
        if (timeattack < timedelay) return;
        if (Input.GetKeyDown(KeyCode.J))
        {
            anim.SetTrigger("Attack");
            AudioManager.Instance.AttackAudio();
            timeattack = 0;
            Hit(SideAttackTransform, SideAttackArea);
           
        }


    }
    private void Skill()
    {
        timeSkill += Time.deltaTime;
        if (timeSkill < timeSkillDelay) return;
        if (Input.GetKeyDown(KeyCode.K) && pState.isSkilling == false && currentMana >=50)
        {
            pState.isSkilling = true;
            anim.SetTrigger("Skill");
            currentMana -= 50;
            UpdateManaBar();
            UpdateCoolDownSkill();
            timeSkill = 0;
            AudioManager.Instance.Skill();
        }
        pState.isSkilling = false;
    }
    public void SkillShotting()
    {
        GameObject SkillTmp = Instantiate(SkillEffect, SkillPos.position, Quaternion.identity);
        Vector3 skillScale = SkillTmp.transform.localScale;
        skillScale.x = transform.localScale.x > 0 ? Mathf.Abs(skillScale.x) : -Mathf.Abs(skillScale.x);
        SkillTmp.transform.localScale = skillScale;
        Rigidbody2D rb1 = SkillTmp.GetComponent<Rigidbody2D>();
        float inputMove = Input.GetAxis("Horizontal");
        float direction = transform.localScale.x;
        rb1.AddForce(new Vector2(direction * SkillForce, 0), ForceMode2D.Impulse);
    }
    private void Hit(Transform _attackTranfrom, Vector2 _attackArea) 
    {
        Collider2D[] ojectsToHit = Physics2D.OverlapBoxAll(_attackTranfrom.position, _attackArea, 0, attackableLayer);
        if(ojectsToHit.Length > 0)
        {
            Debug.Log("Hit");
        }
        for( int i =0; i < ojectsToHit.Length; i++)
        {
            if (ojectsToHit[i].GetComponent<Enemy>() != null)
            {
                ojectsToHit[i].GetComponent<Enemy>().EnemyHit(dame);
            }
        }

    }

    public void PlayerHit(float _damage)
    {
        if (pState.isDead) return;

        currentHp -= _damage;
        currentHp = Mathf.Max(currentHp, 0);
        UpdateHpBar();
        anim.SetBool("isHurt", true);
        AudioManager.Instance.HurtClip();
        CancelInvoke(nameof(ResetHurt));
        Invoke(nameof(ResetHurt), 0.3f);

        if (currentHp <= 0)
        {
            pState.isDead = true;
            anim.SetBool("isDead", true);
            StartCoroutine(DestroyAfterDelay(1.5f));
        }
    }

    private void ResetHurt()
    {
        anim.SetBool("isHurt", false);
    }

    IEnumerator DestroyAfterDelay(float delay)
    {
        anim.SetBool("isDead", true);
        yield return new WaitForSeconds(delay);
        GameManager.Instance.TriggerGameOverAfter(1f);
        Destroy(gameObject);
    }
    public void UpdateHpBar()
    {
        hpBar.fillAmount = currentHp / maxHP;
        hpText.text = currentHp.ToString() + "/" + maxHP;
    }
    public void UpdateManaBar()
    {
        manaBar.fillAmount = currentMana / maxMana;
        manaText.text = currentMana.ToString() + "/" + maxMana;
    }
    void RegenerateMana()
    {
        if (currentMana < maxMana)
        {
            if (pState.dashing == false)
            {
                currentMana += 3;
                currentMana = Mathf.Min(currentMana, maxMana);
                UpdateManaBar();
            }
         
        }
    }
    protected void UpdateCoolDownSkill()
    {
      
        float ratio = timeSkill / timeSkillDelay;
        ratio = Mathf.Clamp01(ratio); 
        CooldownSkill.fillAmount = 1 - ratio;
        if (ratio >= 1f && currentMana < 50)
        {
            CooldownSkill.fillAmount = 1f;
        }
        else
        {
            CooldownSkill.fillAmount = 1 - ratio;
        }
    }
    
}
