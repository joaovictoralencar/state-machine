using System.Collections;
using System.Collections.Generic;
using StateMachine;
using UnityEngine;

public class JumpState : STM_State<PlayerAgent>
{
    private Movement _movement;
    private static readonly int Jump = Animator.StringToHash("Jump");

    protected override void OnInitialize()
    {
        _movement = Agent.GetComponent<Movement>();
    }

    protected override void OnEnterState()
    {
        Agent.Animator.SetBool(Jump, true);
        _movement.Jump();
    }

    protected override void OnExitState()
    {
        Agent.Animator.SetBool(Jump, false);
    }

    public override void SetSubStates()
    {
        if (Agent.WantsToMove && !Agent.RunPressed) SetSubState(typeof(WalkingState).ToString());
        if (Agent.WantsToMove && Agent.RunPressed) SetSubState(typeof(RunningState).ToString());
        else SetSubState(typeof(IdleState).ToString());
    }

    protected override void UpdateState()
    {
    }

    protected override STM_State<PlayerAgent> CheckSwitchState()
    {
        if (_movement.Velocity.y < 0 && !_movement.IsGrounded) return Agent.SwitchState(typeof(InAirState).ToString());
        return null;
    }
}