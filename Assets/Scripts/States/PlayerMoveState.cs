using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player) { }

    public override void UpdateState(PlayerStateManager player)
    {
        // 1. SPRINT-LOGIK (Behålls i Update för snabb respons)
        player.moveSpeed = Input.GetKey(KeyCode.LeftShift) ? player.sprintSpeed : player.originalSpeed;

        // 2. SKICKA DATA TILL ANIMATIONERNA
        // Vi använder player.inputX som vi nu sparar i managern!
        player.anim.SetFloat("moveX", player.inputX);
        player.anim.SetFloat("moveY", player.inputZ);

        float animationMultiplier = (player.inputX == 0 && player.inputZ == 0) ? 0 : player.moveSpeed / player.originalSpeed;
        player.anim.SetFloat("Speed", animationMultiplier);

        // 3. BYT STATE
        if (player.inputX == 0 && player.inputZ == 0)
        {
            player.SwitchState(player.IdleState);
        }
    
        if (Input.GetButtonDown("Jump") && player.isGrounded)
        {
            player.SwitchState(player.JumpState);
        }
    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
        Vector3 direction = new Vector3(player.inputX, 0, player.inputZ).normalized;

        // ÄNDRING HÄR: Ta alltid nuvarande y-velocity från Rigidbody
        // På så sätt påverkas vi av gravitationen naturligt även när vi går.
        float yVel = player.rb.linearVelocity.y;

        player.rb.linearVelocity = new Vector3(
            direction.x * player.moveSpeed,
            yVel, 
            direction.z * player.moveSpeed
        );
    }
}