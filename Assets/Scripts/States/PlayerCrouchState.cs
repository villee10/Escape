using UnityEngine;

public class PlayerCrouchState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        // 1. Sätt farten till crouch-hastighet
        player.moveSpeed = player.crouchSpeed; 

        // 2. Sänk collidern (Vi använder CapsuleCollider som syns i din bild)
        if (player.col != null)
        {
            player.col.height = player.originalColliderSize.y * 0.5f; // originalColliderSize.y sparade vi som height i Managern
            player.col.center = new Vector3(player.originalColliderCenter.x, player.originalColliderCenter.y * 0.5f, player.originalColliderCenter.z);
        }
    }

    public override void UpdateState(PlayerStateManager player)
    {
        player.anim.SetFloat("moveX", player.inputX);
        player.anim.SetFloat("moveY", player.inputZ);

        // Enkel logik: Släpp C = Res dig upp direkt
        if (!Input.GetKey(KeyCode.C)) 
        {
            ResetCrouch(player);
        }
    }

    void ResetCrouch(PlayerStateManager player)
    {
        player.col.height = player.originalColliderSize.y;
        player.col.center = player.originalColliderCenter;
        player.moveSpeed = player.originalSpeed;
        
        // Vi tvingar animatorn att fatta att vi slutar croucha
        player.anim.SetBool("isCrouching", false);
        
        player.SwitchState(player.IdleState);
    }

    public override void FixedUpdateState(PlayerStateManager player)
    {
        // 1. Räkna ut riktningen baserat på Managerns input
        Vector3 direction = new Vector3(player.inputX, 0, player.inputZ).normalized;
    
        // 2. Applicera farten (som redan är satt till crouchSpeed i EnterState)
        Vector3 moveVector = direction * player.moveSpeed;

        // 3. --- FIXEN FÖR TERRÄNG-HACK (Samma som i MoveState) ---
        // Detta förhindrar att gubben "studsar" på terrängens polygoner
        float yVel = player.isGrounded ? 0f : player.rb.linearVelocity.y;

        player.rb.linearVelocity = new Vector3(
            moveVector.x, 
            yVel, 
            moveVector.z
        );
    }
}
