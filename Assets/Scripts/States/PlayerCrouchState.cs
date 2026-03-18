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
        // 3. Skicka data till animationer (Använd Managerns sparade input!)
        player.anim.SetFloat("moveX", player.inputX);
        player.anim.SetFloat("moveY", player.inputZ);

        // 4. TAK-CHECK (Physics.CheckSphere)
        // Vi kollar om det är något ovanför gubbens huvud
        Vector3 headPosition = player.transform.position + Vector3.up * (player.originalColliderSize.y * 0.8f);
        bool isCeilingAbove = Physics.CheckSphere(headPosition, 0.3f, player.groundMask);

        // 5. LOGIK FÖR ATT SLUTA CROUCHA
        if (!Input.GetKey(KeyCode.C)) // Om vi släpper knappen...
        {
            if (!isCeilingAbove) // ...och inget är i vägen ovanför...
            {
                // Återställ collidern innan vi byter state
                player.col.height = player.originalColliderSize.y;
                player.col.center = player.originalColliderCenter;
                player.moveSpeed = player.originalSpeed;

                // Gå till Idle (Managern sköter om vi ska gå direkt till Move därifrån)
                player.SwitchState(player.IdleState);
            }
        }
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