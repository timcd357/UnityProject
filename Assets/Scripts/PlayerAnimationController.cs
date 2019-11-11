using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    Animator am;
    Player player;
    public AnimatorStateInfo stateInfo;
    // Start is called before the first frame update
    void Start()
    {
        am = GetComponent<Animator>();
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        stateInfo = am.GetCurrentAnimatorStateInfo(0);
        am.SetFloat("Horizontal", Mathf.Abs(player.velocityX));
        am.SetFloat("VectorY", player.rb.velocity.y);
        am.SetBool("IsOnGround", player.isOnGround);
        am.SetBool("IsRoll", player.isRoll);
        am.SetBool("IsShield", player.isShield);
        am.SetBool("IsHit", player.isHit);
        am.SetBool("IsDead", player.isDead);
        AttackToIdle();
        EndRoll();
        EndShield();
        EndHit();
    }

    private void AttackToIdle()
    {
        if (stateInfo.normalizedTime < 0.01f)
        {
            player.canHit = false;
        }
        if (player.attack != 0 && stateInfo.normalizedTime > 1.0f&&!stateInfo.IsName("jumpTree"))
        {
            player.attack = 0;
        }
        if (player.attack != 2&&player.attack !=3)
        {
            //Debug.Log("当前状态是：" + player.attack);
            player.currentAttack = player.attack;
            am.SetInteger("Attack", player.attack);
        }
    }
    private void ChangeAttackAnimation()
    {
        am.SetInteger("Attack", player.attack);
    }

    private void EndRoll()
    {
        if (stateInfo.IsName("roll") && stateInfo.normalizedTime > 1.0f)
        {
            player.isRoll = false;
        }
    }
    private void EndShield()
    {
        if (stateInfo.IsName("shield") && stateInfo.normalizedTime > 1.0f)
        {
            player.isShield = false;
        }
    }

    private void EndHit()
    {
        if (stateInfo.IsName("hit") && stateInfo.normalizedTime > 1.0f)
        {
            player.isHit = false;
        }
    }

}
