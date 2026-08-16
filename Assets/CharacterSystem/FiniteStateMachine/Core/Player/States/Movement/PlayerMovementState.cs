using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Windows;

public abstract class PlayerMovementState : IState
{
    protected PlayerMovementStateMachine stateMachine;

    public PlayerMovementState (PlayerMovementStateMachine playerMovementStateMachine)
    {
        stateMachine = playerMovementStateMachine; 
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
    }
}
