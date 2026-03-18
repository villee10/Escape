using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        // Tvinga allt till 0 så han garanterat står i Idle-läge
        player.anim.SetFloat("moveX", 0);
        player.anim.SetFloat("moveY", 0);
        player.anim.SetFloat("Speed", 0);
    
        player.rb.linearVelocity = Vector3.zero;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public override void UpdateState(PlayerStateManager player)
    {
        // Vi kollar om Managerns sparade input inte är noll
        if (player.inputX != 0 || player.inputZ != 0)
        {
            player.SwitchState(player.MoveState);
        }
    }

    public override void FixedUpdateState(PlayerStateManager player) { }
}