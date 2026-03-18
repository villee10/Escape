using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    // Dessa gör hoppet "tungt" och skönt. 
    // Justera dem i Inspectorn tills det känns rätt!
    public float fallMultiplier = 3.5f;
    public float lowJumpMultiplier = 3f;

    public override void EnterState(PlayerStateManager player)
    {
        // Nollställ Y-farten precis vid hoppet för exakt höjd varje gång
        player.rb.linearVelocity = new Vector3(player.rb.linearVelocity.x, 0, player.rb.linearVelocity.z);
        player.rb.AddForce(Vector3.up * player.jumpForce, ForceMode.Impulse);
    }

    public override void UpdateState(PlayerStateManager player)
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        player.anim.SetFloat("moveX", x);
        player.anim.SetFloat("moveY", z);

        // Om vi faller neråt och nuddar marken -> Byt till Idle
        if (player.rb.linearVelocity.y < -0.1f && player.isGrounded)
        {
            player.SwitchState(player.IdleState);
        }
    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 moveDir = new Vector3(x, 0, z).normalized;

        // Här läser vi nu direkt från din PlayerStateManager (player)
        if (player.rb.linearVelocity.y < 0)
        {
            player.rb.linearVelocity +=
                Vector3.up * Physics.gravity.y * (player.fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (player.rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            player.rb.linearVelocity +=
                Vector3.up * Physics.gravity.y * (player.lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }

        player.rb.linearVelocity = new Vector3(moveDir.x * player.moveSpeed, player.rb.linearVelocity.y,
            moveDir.z * player.moveSpeed);
    }

}