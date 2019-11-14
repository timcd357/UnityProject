using System;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("组件")]
    Rigidbody2D rb;
    Animator am;
    Collision coll;

    [Header("地面移动参数")]
    public float speed = 300;
    float velicotyX;
    float y;
    public float wallJumpLerp = 10;
    [Header("其他移动参数")]
    public float jumpForce = 6f;
    float slideSpeed = 1;
    [Header("判断")]
    public bool isJump;
    public bool isDoubleJump;
    public bool isOnGround;
    public bool isTouchWall;
    public bool wallJumped;
    bool canMove=true;
    bool wallSlide;
    [Header("特效")]
    public ParticleSystem dashParticle;
    public ParticleSystem jumpParticle;
    public ParticleSystem wallJumpParticle;
    public ParticleSystem slideParticle;

    //动画参数
    int horizontal;
    int velocityY;
    int onGround;
    int touchWall;

    //按键
    bool jumpPress;
    void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        am = GetComponent<Animator>();
        horizontal = Animator.StringToHash("Horizontal");
        velocityY = Animator.StringToHash("VelocityY");
        onGround = Animator.StringToHash("IsOnGround");
        touchWall = Animator.StringToHash("IsTouchWall");
    }

    // Update is called once per frame
    void Update()
    {
        jumpPress = Input.GetButtonDown("Jump");
        velicotyX = Input.GetAxis("Horizontal");
        y = Input.GetAxis("Vertical");
        isOnGround = coll.onGround;
        isTouchWall = coll.onWall;
        if (isOnGround) {
            wallJumped = false;
            isJump = false;
        }
        AnimationManager();
        if(jumpPress && isOnGround)
            Jump(Vector2.up,false);
        else if(isTouchWall && jumpPress && !isOnGround)
        {
            WallJump();
        }
        if (coll.onWall && !coll.onGround)
        {
            if (velicotyX != 0)
            {
                wallSlide = true;
                WallSlide();
            }
        }
        if (!coll.onWall || coll.onGround)
            wallSlide = false;
        WallParticle(y);
    }
    void WallJump()
    {
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));
        wallJumped = true;       
        Jump(Vector2.up + Vector2.left * transform.localScale*2,true);
        transform.localScale = new Vector3(-transform.localScale.x, 1, 1);
    }
    private void Jump(Vector2 dir, bool wall)
    {
        slideParticle.transform.parent.localScale = new Vector3(1, 1, 1);
        ParticleSystem particle = wall ? wallJumpParticle : jumpParticle;       
        isJump = true;
        isOnGround = false;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity +=  dir * jumpForce;
        particle.Play();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (!canMove)
            return;
        if (velicotyX > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }else if (velicotyX < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }      
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(velicotyX * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);       
    }

    private void WallSlide()
    {
        if (!canMove)
            return;

        bool pushingWall = false;
        if ((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(push, -slideSpeed);
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }
    void WallParticle(float vertical)
    {
        var main = slideParticle.main;

        if (wallSlide || vertical < 0)
        {
            slideParticle.transform.parent.localScale = new Vector3(1, 1, 1);
            main.startColor = Color.white;
        }
        else
        {
            main.startColor = Color.clear;
        }
    }
    void AnimationManager()
    {
        am.SetFloat(horizontal, Mathf.Abs(velicotyX));
        am.SetFloat(velocityY, rb.velocity.y);
        am.SetBool(onGround, isOnGround);
        am.SetBool(touchWall, isTouchWall);
    }


}
