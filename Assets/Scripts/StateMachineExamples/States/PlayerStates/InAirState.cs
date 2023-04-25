using System.Collections;
using System.Collections.Generic;
using StateMachine;
using UnityEngine;

public class InAirState : STM_State<PlayerAgent>
{
    private Movement _movement;

    protected override void OnInitialize()
    {
        _movement = Agent.GetComponent<Movement>();
    }
    protected override void OnEnterState()
    {
    }

    protected override void OnExitState()
    {
    }

    public override void SetSubStates()
    {
        if (Agent.WantsToMove) SetSubState(typeof(WalkingState).ToString());
        else SetSubState(typeof(IdleState).ToString());
    }

    protected override void UpdateState()
    {
    }

    protected override STM_State<PlayerAgent> CheckSwitchState()
    {
        if (_movement.IsGrounded && Agent.JumpPressed) return Agent.SwitchState(typeof(JumpState).ToString());
        if (_movement.IsGrounded) Agent.SwitchState(typeof(GroundedState).ToString());
        return null;
    }
}