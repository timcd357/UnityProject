
using UnityEngine;

public class Skeleton : MonoBehaviour
{
    [Header("组件")]
    Rigidbody2D rb;
    Animator am;
    AnimatorStateInfo stateInfo;
    [Header("移动参数")]
    public float speed=200f;
    int direction;
    int turnTime;
    float turnTimeSpent = 0;

    [Header("攻击参数")]
    
    float attackRange = 4.41f;
    int attack;
    float attackIntervel = 3f;
    float attackSpent = 0;

    [Header("属性")]
    public float hp = 200;
    public float block = 3;
    public float damage = 10;
    [Header("判断")]
    bool isDead;
    bool isHit;
    public bool hit;

    //被击中后的硬直
    float shards = 0.5f;
    float shardsTime;
    //存储血量
    public float saveHp;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        am = GetComponent<Animator>();
        saveHp = hp;
    }
    
    void Update()
    {
        stateInfo = am.GetCurrentAnimatorStateInfo(0);
        Attack();
        IsHit();
        CircleAttackRange();
        Dead();
        AnimationController();
    }
    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (attack != 0||isDead||isHit||hit)
        {
            direction = 0;
            return;
        }
        turnTimeSpent += Time.fixedDeltaTime;
        if (turnTimeSpent > turnTime)
        {
            direction = Random.Range(-1, 2);
            turnTime = Random.Range(2, 8);
            turnTimeSpent = 0;
        }

        if (direction > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        
        rb.velocity = new Vector2(direction * speed * Time.fixedDeltaTime, rb.velocity.y);
    }

    private void Attack()
    {
        if (isDead||isHit)
        {
            attack = 0;
            return;
        }
        Vector2 direction = new Vector2(transform.localScale.x,0);
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position,direction,attackRange-2,LayerMask.GetMask("Player"));
        attackSpent += Time.deltaTime;
        if (hit2D.collider != null)
        {
            if (attackSpent > attackIntervel)
            {
                attack = 1;
                attackSpent = 0;
            }
            else
            {
                attack = 0;
            }
        }
        else
        {
            attack = 0;
        }
    }

    private void Dead()
    {
        if (hp < 0.01)
        {
            isDead = true;
            Destroy(gameObject, 10f);
        }
    }

    private void Hit(object[] o)
    {
        float damage = (float)o[0];
        float direction = (float)o[1];
        float forceX=(float)o[2];
        float forceY = (float)o[3];
        if (forceX == 0)
            forceX = rb.velocity.x;
        if (forceY == 0)
            forceY = rb.velocity.y;
        rb.velocity = new Vector2(forceX * direction, forceY);
        hp -= DataCalucalculator.Damage(damage, block);
    }

    private void IsHit()
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
    }
    private void CircleAttackRange()
    {
        Collider2D colliders = Physics2D.OverlapCircle(transform.position, attackRange, LayerMask.GetMask("Player"));
        if (colliders != null&&hit)
        {            
           if ((colliders.transform.position - transform.position).x * transform.localScale.x > 0)
            {
                //todo 攻击判定成功后的击退（可能是升龙）、伤害等逻辑
            float direction = -(transform.position - colliders.transform.position).normalized.x;
            Player._instance.IsHit(damage,direction);
            }
        }
    }

    private void canHit()
    {
        hit = true;
    }
    private void cantHit()
    {
        hit = false;
    }

    private void AnimationController()
    {
        am.SetInteger("Move", direction);
        am.SetInteger("Attack", attack);
        am.SetBool("IsDead", isDead);
        am.SetBool("IsHit", isHit);
    }
}
