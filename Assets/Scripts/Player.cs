using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    //自己
    public static Player _instance;
    [Header("组件")]
    public Rigidbody2D rb;
    public LayerMask groundCheck;
    PlayerAnimationController amCon;


    [Header("地面移动参数")]
    public float runSpeed = 500;
    public float walkSpeedDevior = 1.5f;

    public float velocityX;

    [Header("跳跃参数")]
    public float jumpForce = 8f;
    float jumpBonus = 1.5f;
    float jumpBonusTime = 0.1f;
    float jumpTime;

    [Header("攻击参数")]
    public int attack = 0;
    public int currentAttack;
    float airHeight = 13f;
    float downAttackForce = 2f;
    float rangeA = 3.77f;
    float rangeB = 4.05f;
    float rangeD = 3.77f;


    [Header("判断")]
    public bool isOnGround, isJump,isRoll,isShield,isHit,isDead;

    [Header("属性")]
    public float hp = 300;
    public float block = 10;
    public float damage = 20;

    //攻击判定bool值
    public bool canHit;

    //按键
    bool jumpPress, jumpHeld,attackPress,rollPress,shieldPress;
    float velocityY;    

    //被击中后的硬直
    float shards = 0.5f;
    float shardsTime;
    //存储血量
    public float saveHp;

    //传递参数
    object[] o = new object[4];

    private void Awake()
    {
        _instance = this;
    }
    void Start()
    {
        saveHp = hp;
        rb = GetComponent<Rigidbody2D>();
        amCon = GetComponent<PlayerAnimationController>();
    }

    void Update()
    {
        if (!isHit || !isDead) {
        isOnGround = rb.IsTouchingLayers(groundCheck);
        jumpPress = Input.GetButtonDown("Jump");
        jumpHeld = Input.GetButton("Jump");
        attackPress = Input.GetButtonDown("Fire1");
        if(!isRoll)
          rollPress = Input.GetButtonDown("Roll");
        if(!isShield)
          shieldPress = Input.GetButtonDown("Shield");

        velocityY = Input.GetAxis("Vertical");
        
        Jump();
        KnifeAttack();
        ROll();
        Shield();
        Dead();
        }
    }

    private void FixedUpdate()
    {
        if (!isHit || !isDead)
        {
            Move();
        }
    }

    private void Dead()
    {
        if (hp < 0.01)
        {
            isDead = true;
            //Destroy(gameObject, 10f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void ROll()
    {
        if (rollPress)
        {
            isRoll = true;
            rb.velocity = new Vector2(15*transform.localScale.x,rb.velocity.y);
            //todo 翻滚过程中无敌

        }
    }

    private void Shield()
    {
        if (shieldPress)
        {
            isShield = true;
            //todo shield期间为弹反时间

        }
    }
    private void Move()
    {
        if (attack == 1||attack==2||attack==3||isRoll||isShield||isHit)
        {
            velocityX = 0;
            return;
        }
        velocityX = Input.GetAxis("Horizontal");
        if (velocityX > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (velocityX < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        rb.velocity = new Vector2(velocityX*runSpeed*Time.fixedDeltaTime,rb.velocity.y);
    }

    private void Jump()
    {
        if (attack == 1 || attack == 2 || attack == 3||isHit)
            return;
        if (jumpPress && isOnGround && !isJump)
        {
            isJump = true;
            attack = 0;
            jumpTime = Time.time + jumpBonusTime;
            rb.AddForce(new Vector2(0,jumpForce), ForceMode2D.Impulse);            
        }else if (isJump && jumpHeld)
        {
            rb.AddForce(new Vector2(0, jumpBonus), ForceMode2D.Impulse);
            if (jumpTime < Time.time)
            {
                isJump = false;
            }
        }
    }

    /// <summary>
    /// 刀类攻击，共计有5种，其中第五种为空中攻击
    /// attack对应：1-A,2-B,3-D,4-C,5-Air,6-空中普攻,7-kick
    /// </summary>
    private void KnifeAttack()
    {
        if (attackPress&&isOnGround)
        {
            if (attack == 0 && amCon.stateInfo.IsName("idle"))
            {
                attack = 1;
            }
            else if (amCon.stateInfo.normalizedTime < 0.8f && attack == 1&&amCon.stateInfo.IsName("attackA"))
            {
                attack =2;               
            }
            else if (attack == 2 && amCon.stateInfo.IsName("attackB")&& amCon.stateInfo.normalizedTime<0.8f)
            {
                attack = 3;
            }
        }else if (velocityY > 0.01&&jumpHeld&&isOnGround)
        {
            attack = 5;
        }else if (attackPress && !isOnGround)
        {
            if (velocityY<-0.01)
            {
                attack = 4;
            }
            else
            {
                attack = 6;
            }
        }
 
        if (amCon.stateInfo.IsName("attackB"))
        {
            currentAttack = 2;
        }
        else if (amCon.stateInfo.IsName("attackD"))
        {
            currentAttack = 3;
        }

        AttackJudgment();
    }

    //可进行攻击判定
    private void CanHit()
    {
        canHit = true;
    }

    //攻击判定结束
    private void CantHit()
    {
       
       canHit = false;          
        
    }

    //攻击判定
    private void AttackJudgment()
    {
        if (canHit) {
        switch (currentAttack)
        {
            case 0:
                break;
            case 1:
                CircleAttackRange(rangeA,3,0,0);
                break;
            case 2:
                LineAttackRange(rangeB,7,0);
                break;
            case 3:
                CircleAttackRange(rangeD,3,0,0);
                break;
            case 4:
               CircleAttackRange(rangeD, 0, 0, downAttackForce);
               break;
            case 5:
                rb.velocity = new Vector2(0,airHeight);
                CircleAttackRange(rangeD,0,airHeight,0);
                break;
            case 6:
                CircleAttackRange(rangeA,3,0,0);
                break;
            case 7:
                break;
        }
        }
    }

    private void CircleAttackRange(float range,float forceX,float forceY,float downAttackForce)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range,LayerMask.GetMask("Enemy"));
        if (colliders != null)
        {
            foreach (Collider2D c in colliders)
            {               
                if ((c.transform.position - transform.position).x * transform.localScale.x > 0)
                {
                    rb.AddForce(new Vector2(0, downAttackForce), ForceMode2D.Impulse);
                    gameObject.GetComponent<Animator>().speed = 0.1f;
                    float direction = transform.localScale.x;
                    o[0] = damage;
                    o[1] = direction;
                    o[2] = forceX;
                    o[3] = forceY;
                    c.SendMessage("Hit",o);
                    Invoke("AnimPlay", 0.1f);
                }
            }
        }
    }

    private void LineAttackRange(float range, float forceX, float forceY)
    {
        Vector2 end = new Vector2(transform.position.x + range * transform.localScale.x, transform.position.y);
        RaycastHit2D[] colliders = Physics2D.LinecastAll(transform.position, end, LayerMask.GetMask("Enemy"));

        if (colliders != null)
        {
            foreach (RaycastHit2D c in colliders)
            {
                gameObject.GetComponent<Animator>().speed = 0.1f;
                float direction = transform.localScale.x;
                o[0] = damage;
                o[1] = direction;
                o[2] = forceX;
                o[3] = forceY;
                c.transform.SendMessage("Hit", o);
                Invoke("AnimPlay", 0.2f);
            }
        }
    }

    private void AnimPlay()
    {
        gameObject.GetComponent<Animator>().speed = 1;
    }
    public void IsHit(float d,float direction)
    {
        if (saveHp > hp)
        {
            saveHp = hp;
            isHit = true;
            shardsTime = Time.time + shards;
        }
        else if (isHit)
        {
            if (shardsTime < Time.time)
            {
                isHit = false;
            }
        }
        rb.velocity = new Vector2(10 * direction, rb.velocity.y);
        hp -= DataCalucalculator.Damage(d, block);
    }

}
