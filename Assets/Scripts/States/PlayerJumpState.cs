using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    // Dessa gör hoppet "tungt" och skönt. 
    // Justera dem i Inspectorn tills det känns rätt!
    public float fallMultiplier = 3.5f;
    public float lowJumpMultiplier = 3f;

    public override void EnterState(PlayerStateManager player)
    {
        // 1. Starta animationen
        player.anim.SetBool("isJumping", true);

        // 2. Nollställ Y-farten och skjut iväg
        player.rb.linearVelocity = new Vector3(player.rb.linearVelocity.x, 0, player.rb.linearVelocity.z);
        player.rb.AddForce(Vector3.up * player.jumpForce, ForceMode.Impulse);
    }

    public override void UpdateState(PlayerStateManager player)
    {
        // Skicka input till JumpTree så Steve lutar sig åt rätt håll i luften
        player.anim.SetFloat("moveX", player.inputX);
        player.anim.SetFloat("moveY", player.inputZ);

        // Om vi faller neråt och nuddar marken -> Landat!
        if (player.rb.linearVelocity.y < -0.1f && player.isGrounded)
        {
            // Stäng av hopp-animationen
            player.anim.SetBool("isJumping", false);
        
            // Gå tillbaka till Idle (som i sin tur kollar om vi ska gå/springa direkt)
            player.SwitchState(player.IdleState);
        }
    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
        // Gör så att vi behåller sprint-farten även i luften om vi håller shift
        float currentAirSpeed = Input.GetKey(KeyCode.LeftShift) ? player.sprintSpeed : player.originalSpeed;

        Vector3 moveDir = new Vector3(player.inputX, 0, player.inputZ).normalized;

        // --- Din befintliga fall-multiplikator-logik här ---
        if (player.rb.linearVelocity.y < 0)
        {
            player.rb.linearVelocity += Vector3.up * Physics.gravity.y * (player.fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (player.rb.linearVelocity.y > 0 && !Input.GetButton("Jump"))
        {
            player.rb.linearVelocity += Vector3.up * Physics.gravity.y * (player.lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }

        // Använd currentAirSpeed istället för player.moveSpeed
        player.rb.linearVelocity = new Vector3(moveDir.x * currentAirSpeed, player.rb.linearVelocity.y, moveDir.z * currentAirSpeed);
    }

}