using UnityEngine;

public class PlayerMovementStateMachine : StateMachine
{
    public PlayerBehavioralController playerBehavioralController { get; }

    // Context Data
    public float TargetSpeed; 
    public float CurrentSpeed; 

    public PlayerMovementStateMachine(PlayerBehavioralController playerBehavioralController)
    {
        this.playerBehavioralController = playerBehavioralController; 
    }
}


