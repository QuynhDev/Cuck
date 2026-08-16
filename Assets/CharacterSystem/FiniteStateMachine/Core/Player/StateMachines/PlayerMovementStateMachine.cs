using UnityEngine;

public class PlayerMovementStateMachine : StateMachine
{
    public PlayerBehavioralController playerBehavioralController { get; }

    public PlayerMovementStateMachine(PlayerBehavioralController playerBehavioralController)
    {
        this.playerBehavioralController = playerBehavioralController; 
    }
}


