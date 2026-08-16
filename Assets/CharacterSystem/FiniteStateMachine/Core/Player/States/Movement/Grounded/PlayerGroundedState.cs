using UnityEngine;

public abstract class PlayerGroundedState : PlayerMovementState
{
    public PlayerGroundedState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
    {
    }
}
