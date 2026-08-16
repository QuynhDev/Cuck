using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Windows;

public abstract class PlayerMovementState : IState
{
    protected PlayerMovementStateMachine stateMachine;
    protected CharacterController characterController; 

    protected readonly PlayerGroundedStateConfig groundedConfig;

    public PlayerMovementState (PlayerMovementStateMachine playerMovementStateMachine)
    {
        stateMachine = playerMovementStateMachine; 
        characterController = playerMovementStateMachine.playerBehavioralController.CharacterController; 

        groundedConfig = new PlayerGroundedStateConfig(); 
    }

    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void PhysicsUpdate()
    {
        Move(); 
    }

    private void Move()
    {
        if (InputManager.Instance.MoveInput == Vector2.zero) return; 

        float targetSpeed = stateMachine.TargetSpeed; 
        float currentHorizontalSpeed = new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;
        float speedOffset = 0.1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            stateMachine.CurrentSpeed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed,
                    Time.deltaTime * groundedConfig.SpeedChangeRate);

            // round speed to 3 decimal places
            stateMachine.CurrentSpeed = Mathf.Round(stateMachine.CurrentSpeed * 1000f) / 1000f;
        }
        else 
        { 
            stateMachine.CurrentSpeed = targetSpeed;
        }
    }
}
