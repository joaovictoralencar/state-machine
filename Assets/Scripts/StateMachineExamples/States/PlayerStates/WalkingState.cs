using System.Collections;
using System.Collections.Generic;
using StateMachine;
using UnityEngine;

public class WalkingState : STM_State<PlayerAgent>
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
    }

    protected override void UpdateState()
    {
        _movement.Move(_movement.WalkingSpeed);
    }

    protected override STM_State<PlayerAgent> CheckSwitchState()
    {
        if (Agent.WantsToMove && Agent.RunPressed && _movement.IsGrounded) return Agent.SwitchState(typeof(RunningState).ToString());
        if (Agent.MovementInput == 0 && !Agent.RunPressed) return Agent.SwitchState(typeof(IdleState).ToString());
        return null;
    }
}