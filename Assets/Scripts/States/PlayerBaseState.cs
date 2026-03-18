using UnityEngine;

public abstract class PlayerBaseState
{
    // Dessa tre metoder är standard för alla states
    public abstract void EnterState(PlayerStateManager player);
    public abstract void UpdateState(PlayerStateManager player);
    public abstract void FixedUpdateState(PlayerStateManager player);
}